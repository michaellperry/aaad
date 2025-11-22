#!/usr/bin/env pwsh
# Run integration tests only

$ErrorActionPreference = "Stop"

Write-Host "Running integration tests..." -ForegroundColor Cyan

dotnet test tests/GloboTicket.IntegrationTests --verbosity normal

if ($LASTEXITCODE -eq 0) {
    Write-Host "Integration tests completed successfully!" -ForegroundColor Green
} else {
    Write-Error "Integration tests failed. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}

