#!/usr/bin/env pwsh
# Delete a migration file after confirming it's not applied to database

param(
    [Parameter(Mandatory=$true)]
    [string]$MigrationName
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($MigrationName)) {
    Write-Error "Migration name cannot be empty"
    exit 1
}

Write-Host "Checking if migration '$MigrationName' is applied to database..." -ForegroundColor Cyan

# Get list of applied migrations
$migrationList = dotnet ef migrations list `
  --project src/GloboTicket.Infrastructure `
  --startup-project src/GloboTicket.API 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to list migrations. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}

# Check if migration is in the applied list (marked with *)
$appliedMigrations = $migrationList | Select-String -Pattern "^\*" | ForEach-Object { $_.Line }

if ($appliedMigrations -match [regex]::Escape($MigrationName)) {
    Write-Error "Migration '$MigrationName' has been applied to the database. Cannot delete."
    Write-Host "You must rollback the migration first using db-migrate-rollback.ps1" -ForegroundColor Yellow
    exit 1
}

# Check if migration file exists
$migrationPath = "src/GloboTicket.Infrastructure/Data/Migrations"
$migrationFiles = Get-ChildItem -Path $migrationPath -Filter "*_$MigrationName.cs" -ErrorAction SilentlyContinue

if ($migrationFiles.Count -eq 0) {
    Write-Error "Migration file for '$MigrationName' not found in $migrationPath"
    exit 1
}

Write-Host "Migration '$MigrationName' is not applied. Proceeding with removal..." -ForegroundColor Yellow

# Remove the migration using EF Core (this handles multiple files)
dotnet ef migrations remove `
  --project src/GloboTicket.Infrastructure `
  --startup-project src/GloboTicket.API

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migration '$MigrationName' removed successfully!" -ForegroundColor Green
} else {
    Write-Error "Failed to remove migration. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}

