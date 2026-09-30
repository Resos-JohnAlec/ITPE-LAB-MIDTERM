param([switch]$SkipBrowser)
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
Set-Location -LiteralPath $projectRoot
$localDotnet = Join-Path $projectRoot '.tools/dotnet/dotnet.exe'
$dotnetCommand = if (Test-Path -LiteralPath $localDotnet) { $localDotnet } else { (Get-Command dotnet).Source }
$env:DOTNET_CLI_HOME = Join-Path $projectRoot '.local/dotnet-home'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_GENERATE_ASPNET_CERTIFICATE = 'false'
$oldConnection = $env:ConnectionStrings__CampusEvents
$oldAdminKey = $env:CAMPUS_ADMIN_KEY
$serverProcess = $null
$runSuffix = [DateTime]::Now.ToString('yyyyMMdd_HHmmss')
$evidencePath = Join-Path $projectRoot 'docs/evidence'
$localPath = Join-Path $projectRoot '.local'
New-Item -ItemType Directory -Force -Path $evidencePath, $localPath | Out-Null
$backendProject = Join-Path $projectRoot 'backend/CampusEvents.csproj'

function Assert-Exit([string]$step) {
    if ($LASTEXITCODE -ne 0) { throw "$step failed (exit $LASTEXITCODE)." }
}

function Start-VerificationServer([string]$databaseSuffix) {
    $env:ConnectionStrings__CampusEvents = "Server=(localdb)\MSSQLLocalDB;Database=CampusEventsVerification_$databaseSuffix;Integrated Security=true;Encrypt=true;TrustServerCertificate=true"
    & $dotnetCommand run --no-build --project $backendProject -- --init-db
    Assert-Exit 'Database initialization'
    $process = Start-Process -FilePath $dotnetCommand -ArgumentList @('run','--no-build','--project',('"' + $backendProject + '"')) -WorkingDirectory $projectRoot -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $localPath "server-$databaseSuffix.log") -RedirectStandardError (Join-Path $localPath "server-$databaseSuffix.error.log")
    $script:serverProcess = $process
    $ready = $false
    for ($attempt = 0; $attempt -lt 40; $attempt++) {
        if ($process.HasExited) { throw 'Verification server exited before becoming ready. See .local logs.' }
        try {
            $response = Invoke-WebRequest -UseBasicParsing -Uri 'http://127.0.0.1:5080/api/events' -TimeoutSec 2
            if ($response.StatusCode -eq 200) { $ready = $true; break }
        } catch { Start-Sleep -Milliseconds 250 }
    }
    if (-not $ready) { throw 'Verification server did not become ready.' }
}

function Stop-VerificationServer {
    if ($script:serverProcess -and -not $script:serverProcess.HasExited) {
        # Stop the known dotnet run child first, then its launcher. Never kill by name.
        $launcherId = $script:serverProcess.Id
        Get-CimInstance Win32_Process -Filter "ParentProcessId=$launcherId" | ForEach-Object {
            Stop-Process -Id $_.ProcessId -ErrorAction SilentlyContinue
        }
        Stop-Process -Id $launcherId -ErrorAction SilentlyContinue
        $script:serverProcess.WaitForExit(5000) | Out-Null
    }
    $script:serverProcess = $null
}

try {
    $listener = [System.Net.NetworkInformation.IPGlobalProperties]::GetIPGlobalProperties().GetActiveTcpListeners() | Where-Object Port -eq 5080
    if ($listener) { throw 'Port 5080 is in use. Stop your existing app before running verification.' }
    & $dotnetCommand test tests/CampusEvents.Tests/CampusEvents.Tests.csproj --logger 'trx;LogFileName=unit-tests.trx' --results-directory $evidencePath
    Assert-Exit 'Unit tests'
    & $dotnetCommand build tests/CampusEvents.Integration/CampusEvents.Integration.csproj
    Assert-Exit 'Integration build'
    $env:CAMPUS_ADMIN_KEY = [Guid]::NewGuid().ToString('N')
    Start-VerificationServer ($runSuffix + '_api')
    & $dotnetCommand run --no-build --project tests/CampusEvents.Integration/CampusEvents.Integration.csproj | Tee-Object -Variable integrationOutput
    $integrationOutput | Set-Content -LiteralPath (Join-Path $evidencePath 'integration-results.txt') -Encoding UTF8
    Assert-Exit 'Integration checks'
    Stop-VerificationServer
    if (-not $SkipBrowser) {
        Start-VerificationServer ($runSuffix + '_browser')
        node tests/browser.mjs
        Assert-Exit 'Browser checks'
    }
    Write-Host 'Verification complete. Evidence is in docs/evidence; dedicated test databases were retained for inspection.'
} finally {
    Stop-VerificationServer
    $env:ConnectionStrings__CampusEvents = $oldConnection
    $env:CAMPUS_ADMIN_KEY = $oldAdminKey
}
