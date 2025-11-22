#!/usr/bin/env pwsh
# View Docker logs (takes optional service name)

param(
    [Parameter(Mandatory=$false)]
    [string]$ServiceName
)

$ErrorActionPreference = "Stop"

if ($ServiceName) {
    Write-Host "Viewing logs for service: $ServiceName" -ForegroundColor Cyan
    docker compose -f docker/docker-compose.yml logs $ServiceName
} else {
    Write-Host "Viewing all Docker logs..." -ForegroundColor Cyan
    docker compose -f docker/docker-compose.yml logs
}

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to view Docker logs. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}

