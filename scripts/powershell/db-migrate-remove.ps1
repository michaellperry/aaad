#!/usr/bin/env pwsh
# Remove the last migration (unapplies if applied, deletes if not)

$ErrorActionPreference = "Stop"

Write-Host "Removing last migration..." -ForegroundColor Yellow
Write-Host "Note: This will only work if the migration hasn't been applied to any database." -ForegroundColor Yellow

dotnet ef migrations remove `
  --project src/GloboTicket.Infrastructure `
  --startup-project src/GloboTicket.API

if ($LASTEXITCODE -eq 0) {
    Write-Host "Last migration removed successfully!" -ForegroundColor Green
} else {
    Write-Error "Failed to remove migration. Exit code: $LASTEXITCODE"
    Write-Host "If the migration has been applied to a database, you must rollback first using db-migrate-rollback.ps1" -ForegroundColor Yellow
    exit $LASTEXITCODE
}

