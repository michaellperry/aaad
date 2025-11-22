#!/usr/bin/env pwsh
# List all migrations and their applied status

$ErrorActionPreference = "Stop"

Write-Host "Listing migrations and their status..." -ForegroundColor Cyan

dotnet ef migrations list `
  --project src/GloboTicket.Infrastructure `
  --startup-project src/GloboTicket.API

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to list migrations. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}

