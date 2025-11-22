#!/usr/bin/env pwsh
# Add a new migration

param(
    [Parameter(Mandatory=$true)]
    [string]$MigrationName
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($MigrationName)) {
    Write-Error "Migration name cannot be empty"
    exit 1
}

Write-Host "Adding migration: $MigrationName" -ForegroundColor Cyan

dotnet ef migrations add $MigrationName `
  --project src/GloboTicket.Infrastructure `
  --startup-project src/GloboTicket.API `
  --output-dir Data/Migrations

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migration '$MigrationName' created successfully!" -ForegroundColor Green
} else {
    Write-Error "Failed to create migration. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}

