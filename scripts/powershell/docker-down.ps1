#!/usr/bin/env pwsh
# Stop Docker infrastructure

$ErrorActionPreference = "Stop"

Write-Host "Stopping Docker infrastructure..." -ForegroundColor Cyan

docker compose -f docker/docker-compose.yml down

if ($LASTEXITCODE -eq 0) {
    Write-Host "Docker infrastructure stopped successfully!" -ForegroundColor Green
} else {
    Write-Error "Failed to stop Docker infrastructure. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}

