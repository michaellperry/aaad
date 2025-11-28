#!/usr/bin/env pwsh
# Drop and recreate database with all migrations

$ErrorActionPreference = "Stop"

$connectionString = "Server=localhost,1433;Database=GloboTicket;User Id=migration_user;Password=Migration@Pass123;TrustServerCertificate=True;Encrypt=True"

Write-Host "Dropping database..." -ForegroundColor Yellow

dotnet ef database drop --force `
  --project src/GloboTicket.Infrastructure `
  --startup-project src/GloboTicket.API

if ($LASTEXITCODE -ne 0) {
    Write-Warning "Database drop may have failed or database didn't exist. Continuing..."
}

Write-Host "Recreating database with all migrations..." -ForegroundColor Cyan

dotnet ef database update `
  --project src/GloboTicket.Infrastructure `
  --startup-project src/GloboTicket.API `
  --connection $connectionString

if ($LASTEXITCODE -eq 0) {
    Write-Host "Database reset completed successfully!" -ForegroundColor Green
} else {
    Write-Error "Failed to recreate database. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}

