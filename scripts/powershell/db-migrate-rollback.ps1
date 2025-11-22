#!/usr/bin/env pwsh
# Rollback to a specific migration (use "0" to rollback all migrations)

param(
    [Parameter(Mandatory=$true)]
    [string]$TargetMigration
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($TargetMigration)) {
    Write-Error "Target migration cannot be empty. Use '0' to rollback all migrations."
    exit 1
}

$connectionString = "Server=localhost,1433;Database=GloboTicket;User Id=migration_user;Password=Migration@Pass123;TrustServerCertificate=True;Encrypt=True"

if ($TargetMigration -eq "0") {
    Write-Host "Rolling back all migrations..." -ForegroundColor Yellow
} else {
    Write-Host "Rolling back to migration: $TargetMigration" -ForegroundColor Yellow
}

dotnet ef database update $TargetMigration `
  --project src/GloboTicket.Infrastructure `
  --startup-project src/GloboTicket.API `
  --connection $connectionString

if ($LASTEXITCODE -eq 0) {
    Write-Host "Rollback completed successfully!" -ForegroundColor Green
} else {
    Write-Error "Failed to rollback migration. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}

