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

dotnet run --project $project.FullName
