$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$webProject = Join-Path $repositoryRoot 'MyShop'
$applicationDll = Join-Path $webProject 'bin\Release\net10.0\MyShop.dll'
$databaseName = 'MyShopSmoke'
$stdoutPath = Join-Path ([System.IO.Path]::GetTempPath()) 'myshop-smoke.stdout.log'
$stderrPath = Join-Path ([System.IO.Path]::GetTempPath()) 'myshop-smoke.stderr.log'
$appProcess = $null

try {
    dotnet build (Join-Path $repositoryRoot 'MyShopClean.sln') -c Release
    if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }

    sqllocaldb start MSSQLLocalDB | Out-Null
    $env:ConnectionStrings__ConnectToDataBase = "Server=(localdb)\MSSQLLocalDB;Database=$databaseName;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
    $env:SeedAdmin__Password = 'SmokeTest_Admin#2026'
    $env:ASPNETCORE_URLS = 'http://127.0.0.1:5099'
    $env:ASPNETCORE_ENVIRONMENT = 'Development'
    $env:Database__ApplyMigrationsOnStartup = 'true'
    $env:SeedDemoData__Enabled = 'true'

    Remove-Item -LiteralPath $stdoutPath, $stderrPath -Force -ErrorAction SilentlyContinue
    $appProcess = Start-Process `
        -FilePath 'dotnet' `
        -ArgumentList $applicationDll `
        -WorkingDirectory $webProject `
        -WindowStyle Hidden `
        -RedirectStandardOutput $stdoutPath `
        -RedirectStandardError $stderrPath `
        -PassThru

    $response = $null
    for ($attempt = 1; $attempt -le 60; $attempt++) {
        Start-Sleep -Seconds 1
        try {
            $response = Invoke-WebRequest -Uri 'http://127.0.0.1:5099/' -UseBasicParsing -TimeoutSec 5
            break
        }
        catch {
            if ($appProcess.HasExited) { break }
        }
    }

    if ($null -eq $response) {
        throw "The home page did not become available.`n$(Get-Content -LiteralPath $stderrPath -Raw -ErrorAction SilentlyContinue)"
    }

    if ($response.StatusCode -ne 200) { throw "Unexpected HTTP status $($response.StatusCode)." }
    if (-not $response.Content.Contains('placeholder.svg')) { throw 'The seeded demo content was not rendered.' }

    Write-Output "Smoke test passed: HTTP $($response.StatusCode), $($response.Content.Length) bytes."
}
finally {
    if ($null -ne $appProcess -and -not $appProcess.HasExited) {
        Stop-Process -Id $appProcess.Id -Force
        $appProcess.WaitForExit()
    }

    sqlcmd -S '(localdb)\MSSQLLocalDB' -Q "IF DB_ID('$databaseName') IS NOT NULL BEGIN ALTER DATABASE [$databaseName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$databaseName]; END" 2>$null
    Remove-Item -LiteralPath $stdoutPath, $stderrPath -Force -ErrorAction SilentlyContinue
}
