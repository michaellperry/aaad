#!/usr/bin/env pwsh
# Check Docker container status

$ErrorActionPreference = "Stop"

Write-Host "Checking Docker container status..." -ForegroundColor Cyan

docker compose -f docker/docker-compose.yml ps

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to check Docker status. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}

