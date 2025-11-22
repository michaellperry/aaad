#!/usr/bin/env pwsh
# Build the solution

$ErrorActionPreference = "Stop"

Write-Host "Building solution..." -ForegroundColor Cyan

dotnet build GloboTicket.sln

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build completed successfully!" -ForegroundColor Green
} else {
    Write-Error "Build failed. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}

