param([switch]$InitializeOnly)
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$localDotnet = Join-Path $projectRoot '.tools/dotnet/dotnet.exe'
$dotnetCommand = if (Test-Path -LiteralPath $localDotnet) { $localDotnet } else { 'dotnet' }
$env:DOTNET_CLI_HOME = Join-Path $projectRoot '.local/dotnet-home'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_GENERATE_ASPNET_CERTIFICATE = 'false'
$projectFile = Join-Path $projectRoot 'backend/CampusEvents.csproj'
& $dotnetCommand run --project $projectFile -- --init-db
if ($LASTEXITCODE -ne 0) { throw 'Database initialization failed. Check SQL Server LocalDB and the connection string.' }
if ($InitializeOnly) { exit 0 }
if (-not $env:CAMPUS_ADMIN_KEY) {
    $keyBytes = New-Object byte[] 24
    $generator = [System.Security.Cryptography.RandomNumberGenerator]::Create()
    try { $generator.GetBytes($keyBytes) } finally { $generator.Dispose() }
    $env:CAMPUS_ADMIN_KEY = [Convert]::ToBase64String($keyBytes)
    Write-Host 'A temporary organizer key was generated for this run:'
    Write-Host $env:CAMPUS_ADMIN_KEY
}
Write-Host 'Campus Gather: http://127.0.0.1:5080'
Write-Host 'Press Ctrl+C to stop. Registrations stay in the database.'
& $dotnetCommand run --no-build --project $projectFile
exit $LASTEXITCODE
