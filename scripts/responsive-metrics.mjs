import { mkdir, writeFile } from 'node:fs/promises';
import path from 'node:path';

const [debugPort, baseUrl, outputDirectory] = process.argv.slice(2);
if (!debugPort || !baseUrl || !outputDirectory) {
    throw new Error('Usage: node responsive-metrics.mjs <debug-port> <base-url> <output-directory>');
}

const pages = [
    { name: 'home', url: '/' },
    { name: 'products', url: '/Product' },
    { name: 'product-detail', url: '/Product/Description?productId=1' },
    { name: 'login', url: '/Login' },
    { name: 'register', url: '/Register' },
    { name: 'contact', url: '/ContactUs' },
    { name: 'about', url: '/AboutUs' }
];
const viewports = [
    { name: 'mobile', width: 390, height: 844 },
    { name: 'tablet', width: 768, height: 1024 },
    { name: 'desktop', width: 1440, height: 900 }
];

const targets = await fetch(`http://127.0.0.1:${debugPort}/json/list`).then(response => response.json());
const target = targets.find(item => item.type === 'page');
if (!target) throw new Error('Chrome did not expose a page target.');

const socket = new WebSocket(target.webSocketDebuggerUrl);
await new Promise((resolve, reject) => {
    socket.addEventListener('open', resolve, { once: true });
    socket.addEventListener('error', reject, { once: true });
});

let requestId = 0;
const pending = new Map();
const eventWaiters = new Map();

socket.addEventListener('message', event => {
    const message = JSON.parse(event.data);
    if (message.id && pending.has(message.id)) {
        const { resolve, reject } = pending.get(message.id);
        pending.delete(message.id);
        if (message.error) reject(new Error(message.error.message));
        else resolve(message.result);
        return;
    }

    const waiters = eventWaiters.get(message.method);
    if (!waiters) return;
    eventWaiters.delete(message.method);
    waiters.forEach(resolve => resolve(message.params));
});

function send(method, params = {}) {
    const id = ++requestId;
    return new Promise((resolve, reject) => {
        pending.set(id, { resolve, reject });
        socket.send(JSON.stringify({ id, method, params }));
    });
}

function waitForEvent(method, timeoutMs = 15000) {
    return new Promise((resolve, reject) => {
        const timeout = setTimeout(() => reject(new Error(`Timed out waiting for ${method}.`)), timeoutMs);
        const resolveWithCleanup = value => {
            clearTimeout(timeout);
            resolve(value);
        };
        const waiters = eventWaiters.get(method) ?? [];
        waiters.push(resolveWithCleanup);
        eventWaiters.set(method, waiters);
    });
}

await mkdir(outputDirectory, { recursive: true });
await send('Page.enable');

const failures = [];
for (const page of pages) {
    for (const viewport of viewports) {
        await send('Emulation.setDeviceMetricsOverride', {
            width: viewport.width,
            height: viewport.height,
            deviceScaleFactor: 1,
            mobile: viewport.name === 'mobile'
        });

        const loaded = waitForEvent('Page.loadEventFired');
        await send('Page.navigate', { url: `${baseUrl}${page.url}` });
        await loaded;
        await new Promise(resolve => setTimeout(resolve, 750));
        await send('Runtime.evaluate', { expression: 'window.scrollTo(0, 0)' });

        const evaluation = await send('Runtime.evaluate', {
            returnByValue: true,
            expression: `(() => {
                const width = document.documentElement.clientWidth;
                const elements = [...document.body.querySelectorAll('*')];
                const offenders = elements.map(element => {
                    const rect = element.getBoundingClientRect();
                    return {
                        tag: element.tagName.toLowerCase(),
                        id: element.id,
                        className: typeof element.className === 'string' ? element.className.slice(0, 120) : '',
                        left: Math.round(rect.left),
                        right: Math.round(rect.right),
                        width: Math.round(rect.width)
                    };
                }).filter(item => item.width > 0 && (item.left < -2 || item.right > width + 2)).slice(0, 12);

                return {
                    viewportWidth: width,
                    documentWidth: document.documentElement.scrollWidth,
                    bodyWidth: document.body.scrollWidth,
                    anchors: ['.site-header', '.site-header > .container-main', '.site-header-row', '.site-main', '.site-footer']
                        .map(selector => {
                            const element = document.querySelector(selector);
                            if (!element) return { selector, missing: true };
                            const rect = element.getBoundingClientRect();
                            const style = getComputedStyle(element);
                            return {
                                selector,
                                left: Math.round(rect.left),
                                right: Math.round(rect.right),
                                width: Math.round(rect.width),
                                display: style.display,
                                direction: style.direction
                            };
                        }),
                    offenders
                };
            })()`
        });

        const metrics = evaluation.result.value;
        if (metrics.documentWidth > metrics.viewportWidth + 2 || metrics.bodyWidth > metrics.viewportWidth + 2) {
            failures.push({ page: page.name, viewport: viewport.name, ...metrics });
        }

        const screenshot = await send('Page.captureScreenshot', {
            format: 'png',
            captureBeyondViewport: false,
            fromSurface: true
        });
        await writeFile(
            path.join(outputDirectory, `${page.name}-${viewport.name}.png`),
            Buffer.from(screenshot.data, 'base64')
        );
    }
}

socket.close();
if (failures.length) {
    console.error(JSON.stringify(failures.slice(0, 3), null, 2));
    console.error(`Horizontal overflow found in ${failures.length} page/viewport combinations.`);
    process.exitCode = 1;
} else {
    console.log(`Responsive metrics passed: ${pages.length * viewports.length} page/viewport combinations without horizontal overflow.`);
}
