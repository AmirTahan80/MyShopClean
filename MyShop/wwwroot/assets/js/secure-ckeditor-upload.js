(function (global) {
    "use strict";

    class SecureUploadAdapter {
        constructor(loader, uploadUrl, antiforgeryToken) {
            this.loader = loader;
            this.uploadUrl = uploadUrl;
            this.antiforgeryToken = antiforgeryToken;
            this.abortController = new AbortController();
        }

        upload() {
            return this.loader.file.then(file => {
                const data = new FormData();
                data.append("upload", file);

                return fetch(this.uploadUrl, {
                    method: "POST",
                    credentials: "same-origin",
                    headers: {
                        "RequestVerificationToken": this.antiforgeryToken
                    },
                    body: data,
                    signal: this.abortController.signal
                });
            }).then(async response => {
                const result = await response.json();
                if (!response.ok || result.uploaded !== 1 || !result.url) {
                    throw new Error(result.error?.message || "بارگذاری تصویر انجام نشد.");
                }

                return { default: result.url };
            });
        }

        abort() {
            this.abortController.abort();
        }
    }

    global.enableSecureCkEditorUpload = function (editor, uploadUrl, antiforgeryToken) {
        editor.plugins.get("FileRepository").createUploadAdapter =
            loader => new SecureUploadAdapter(loader, uploadUrl, antiforgeryToken);
        return editor;
    };
})(window);
