$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$webProject = Join-Path $repositoryRoot 'MyShop'
$applicationDll = Join-Path $webProject 'bin\Release\net10.0\MyShop.dll'
$databaseName = 'MyShopSiteAudit'
$baseUrl = 'http://127.0.0.1:5099'
$artifactDirectory = Join-Path $repositoryRoot 'artifacts\responsive-audit'
$chromeProfile = Join-Path ([System.IO.Path]::GetTempPath()) 'myshop-responsive-chrome'
$stdoutPath = Join-Path ([System.IO.Path]::GetTempPath()) 'myshop-site-audit.stdout.log'
$stderrPath = Join-Path ([System.IO.Path]::GetTempPath()) 'myshop-site-audit.stderr.log'
$chromePath = 'C:\Program Files\Google\Chrome\Application\chrome.exe'
$appProcess = $null
$chromeProcess = $null

try {
    if (-not (Test-Path -LiteralPath $chromePath)) {
        throw "Google Chrome was not found at $chromePath."
    }

    dotnet build (Join-Path $repositoryRoot 'MyShopClean.sln') -c Release
    if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }

    sqllocaldb start MSSQLLocalDB | Out-Null
    $env:ConnectionStrings__ConnectToDataBase = "Server=(localdb)\MSSQLLocalDB;Database=$databaseName;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
    $env:SeedAdmin__Password = 'SiteAudit_Admin#2026'
    $env:Database__ApplyMigrationsOnStartup = 'true'
    $env:SeedDemoData__Enabled = 'true'
    $env:ASPNETCORE_URLS = $baseUrl
    $env:ASPNETCORE_ENVIRONMENT = 'Development'

    Remove-Item -LiteralPath $stdoutPath, $stderrPath -Force -ErrorAction SilentlyContinue
    Remove-Item -LiteralPath $artifactDirectory, $chromeProfile -Recurse -Force -ErrorAction SilentlyContinue
    New-Item -ItemType Directory -Path $artifactDirectory | Out-Null

    $appProcess = Start-Process `
        -FilePath 'dotnet' `
        -ArgumentList $applicationDll `
        -WorkingDirectory $webProject `
        -WindowStyle Hidden `
        -RedirectStandardOutput $stdoutPath `
        -RedirectStandardError $stderrPath `
        -PassThru

    $homeResponse = $null
    for ($attempt = 1; $attempt -le 60; $attempt++) {
        Start-Sleep -Seconds 1
        try {
            $homeResponse = Invoke-WebRequest -Uri "$baseUrl/" -UseBasicParsing -TimeoutSec 5
            break
        }
        catch {
            if ($appProcess.HasExited) { break }
        }
    }

    if ($null -eq $homeResponse) {
        throw "The application did not become available.`n$(Get-Content -LiteralPath $stderrPath -Raw -ErrorAction SilentlyContinue)"
    }

    $settingsSql = @"
UPDATE [SiteSettings]
SET [SiteName] = N'Responsive Test Shop',
    [SiteTagline] = N'Responsive storefront',
    [PrimaryColor] = '#123456',
    [SecondaryColor] = '#345678',
    [AccentColor] = '#f97316',
    [FooterTitle] = N'Dynamic Footer',
    [FooterDescription] = N'Database-backed footer content',
    [FooterCopyright] = N'Responsive Test Shop',
    [Address] = N'Test Address',
    [Phone] = N'021-12345678',
    [SupportEmail] = N'test@example.com',
    [PublicBaseUrl] = N'$baseUrl',
    [TorobEnabled] = 1,
    [TorobAccessToken] = N'torob-audit-token-12345678901234567890',
    [EmallsEnabled] = 1,
    [EmallsAccessToken] = N'emalls-audit-token-1234567890123456789';
"@
    sqlcmd -S '(localdb)\MSSQLLocalDB' -d $databaseName -b -Q $settingsSql | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'Could not update the temporary site settings.' }

    Stop-Process -Id $appProcess.Id -Force
    $appProcess.WaitForExit()
    $appProcess = Start-Process `
        -FilePath 'dotnet' `
        -ArgumentList $applicationDll `
        -WorkingDirectory $webProject `
        -WindowStyle Hidden `
        -RedirectStandardOutput $stdoutPath `
        -RedirectStandardError $stderrPath `
        -PassThru

    $homeResponse = $null
    for ($attempt = 1; $attempt -le 30; $attempt++) {
        Start-Sleep -Milliseconds 500
        try {
            $homeResponse = Invoke-WebRequest -Uri "$baseUrl/" -UseBasicParsing -TimeoutSec 5
            break
        }
        catch {
            if ($appProcess.HasExited) { break }
        }
    }
    if ($null -eq $homeResponse) { throw 'The application did not restart after updating settings.' }

    $homeResponse = Invoke-WebRequest -Uri "$baseUrl/" -UseBasicParsing -TimeoutSec 10
    foreach ($expectedValue in @('Responsive Test Shop', 'Dynamic Footer', '021-12345678', '#123456')) {
        if (-not $homeResponse.Content.Contains($expectedValue)) {
            throw "Dynamic setting '$expectedValue' was not rendered on the home page."
        }
    }

    $torob = Invoke-RestMethod -Uri "$baseUrl/integrations/torob/products" -Headers @{
        'X-Integration-Token' = 'torob-audit-token-12345678901234567890'
    } -TimeoutSec 10
    $emalls = Invoke-RestMethod -Uri "$baseUrl/integrations/emalls/products" -Headers @{
        'X-Integration-Token' = 'emalls-audit-token-1234567890123456789'
    } -TimeoutSec 10
    $emallsXml = Invoke-WebRequest -Uri "$baseUrl/integrations/emalls/products.xml" -Headers @{
        'X-Integration-Token' = 'emalls-audit-token-1234567890123456789'
    } -UseBasicParsing -TimeoutSec 10

    if ($torob.count -lt 1 -or $emalls.count -lt 1) { throw 'A marketplace feed contained no products.' }
    if ($torob.products[0].currency -ne 'TOMAN') { throw 'The Torob feed currency was not declared.' }
    if (-not $emallsXml.Content.Contains('<currency>TOMAN</currency>')) { throw 'The E-malls XML feed currency was not declared.' }

    $pages = @(
        @{ Name = 'home'; Url = '/' },
        @{ Name = 'products'; Url = '/Product' },
        @{ Name = 'product-detail'; Url = '/Product/Description?productId=1' },
        @{ Name = 'login'; Url = '/Login' },
        @{ Name = 'register'; Url = '/Register' },
        @{ Name = 'contact'; Url = '/ContactUs' },
        @{ Name = 'about'; Url = '/AboutUs' }
    )
    foreach ($page in $pages) {
        $response = Invoke-WebRequest -Uri "$baseUrl$($page.Url)" -UseBasicParsing -TimeoutSec 10
        if ($response.StatusCode -ne 200) {
            throw "Page '$($page.Name)' returned HTTP $($response.StatusCode)."
        }
    }

    $chromeProcess = Start-Process `
        -FilePath $chromePath `
        -ArgumentList '--headless=new', '--disable-gpu', '--disable-extensions', '--hide-scrollbars', '--no-first-run', '--remote-debugging-port=9223', "--user-data-dir=$chromeProfile", 'about:blank' `
        -WindowStyle Hidden `
        -PassThru

    $chromeReady = $false
    for ($attempt = 1; $attempt -le 40; $attempt++) {
        Start-Sleep -Milliseconds 250
        try {
            Invoke-RestMethod -Uri 'http://127.0.0.1:9223/json/version' -TimeoutSec 2 | Out-Null
            $chromeReady = $true
            break
        }
        catch {
            if ($chromeProcess.HasExited) { break }
        }
    }
    if (-not $chromeReady) { throw 'Chrome DevTools did not become available.' }

    node (Join-Path $PSScriptRoot 'responsive-metrics.mjs') '9223' $baseUrl $artifactDirectory
    if ($LASTEXITCODE -ne 0) { throw 'Responsive layout metrics failed.' }

    Write-Output "Site audit passed: $($pages.Count) pages at 3 viewport sizes."
    Write-Output "Marketplace feeds passed: Torob JSON, E-malls JSON and E-malls XML."
    Write-Output "Screenshots: $artifactDirectory"
}
finally {
    if ($null -ne $chromeProcess -and -not $chromeProcess.HasExited) {
        Stop-Process -Id $chromeProcess.Id -Force
        $chromeProcess.WaitForExit()
    }

    if ($null -ne $appProcess -and -not $appProcess.HasExited) {
        Stop-Process -Id $appProcess.Id -Force
        $appProcess.WaitForExit()
    }

    sqlcmd -S '(localdb)\MSSQLLocalDB' -Q "IF DB_ID('$databaseName') IS NOT NULL BEGIN ALTER DATABASE [$databaseName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$databaseName]; END" 2>$null
    Remove-Item -LiteralPath $stdoutPath, $stderrPath, $chromeProfile -Recurse -Force -ErrorAction SilentlyContinue
}
