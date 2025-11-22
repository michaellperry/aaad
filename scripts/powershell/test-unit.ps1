#!/usr/bin/env pwsh
# Run unit tests only

$ErrorActionPreference = "Stop"

Write-Host "Running unit tests..." -ForegroundColor Cyan

dotnet test tests/GloboTicket.UnitTests --verbosity normal

if ($LASTEXITCODE -eq 0) {
    Write-Host "Unit tests completed successfully!" -ForegroundColor Green
} else {
    Write-Error "Unit tests failed. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}

