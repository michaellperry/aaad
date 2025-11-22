#!/usr/bin/env pwsh
# Start Docker infrastructure

$ErrorActionPreference = "Stop"

Write-Host "Starting Docker infrastructure..." -ForegroundColor Cyan

docker compose -f docker/docker-compose.yml up -d

if ($LASTEXITCODE -eq 0) {
    Write-Host "Docker infrastructure started successfully!" -ForegroundColor Green
    Write-Host "Waiting for containers to become healthy..." -ForegroundColor Yellow
    Start-Sleep -Seconds 5
    docker compose -f docker/docker-compose.yml ps
} else {
    Write-Error "Failed to start Docker infrastructure. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}

