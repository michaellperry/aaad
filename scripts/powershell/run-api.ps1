#!/usr/bin/env pwsh
# Run the API server

$ErrorActionPreference = "Stop"

Write-Host "Starting API server..." -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop" -ForegroundColor Yellow

Push-Location src/GloboTicket.API

try {
    dotnet run
} finally {
    Pop-Location
}

