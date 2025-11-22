#!/usr/bin/env pwsh
# Run the web frontend

$ErrorActionPreference = "Stop"

Push-Location src/GloboTicket.Web

try {
    if (-not (Test-Path "node_modules")) {
        Write-Host "node_modules not found. Installing dependencies..." -ForegroundColor Yellow
        npm install
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Failed to install npm dependencies. Exit code: $LASTEXITCODE"
            exit $LASTEXITCODE
        }
    }

    Write-Host "Starting web frontend..." -ForegroundColor Cyan
    Write-Host "Press Ctrl+C to stop" -ForegroundColor Yellow

    npm run dev
} finally {
    Pop-Location
}

