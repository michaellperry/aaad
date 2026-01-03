#!/usr/bin/env pwsh
# Format the solution to remove unnecessary imports and apply code style

$ErrorActionPreference = "Stop"

Write-Host "Running dotnet format on solution..." -ForegroundColor Cyan

dotnet format GloboTicket.sln

if ($LASTEXITCODE -eq 0) {
    Write-Host "Format completed successfully!" -ForegroundColor Green
} else {
    Write-Error "Format failed. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}
