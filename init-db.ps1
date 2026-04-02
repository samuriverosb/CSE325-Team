$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $repoRoot

$env:DOTNET_CLI_HOME = Join-Path $repoRoot ".dotnet"
if (-not (Test-Path $env:DOTNET_CLI_HOME)) {
    New-Item -ItemType Directory -Path $env:DOTNET_CLI_HOME | Out-Null
}

$project = Get-ChildItem -Path "src/FinanceTracker.Web" -Filter "*.csproj" | Select-Object -First 1
if (-not $project) {
    throw "Could not find a .csproj file in src/FinanceTracker.Web."
}

$dbPath = "src/FinanceTracker.Web/Data/app.db"

if (Test-Path $dbPath) {
    Remove-Item -LiteralPath $dbPath -Force
}

dotnet dotnet-ef database update --project $project.FullName --startup-project $project.FullName --context ApplicationDbContext
