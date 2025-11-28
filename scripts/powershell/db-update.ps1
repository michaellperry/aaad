#!/usr/bin/env pwsh
# Apply all pending migrations to the database

$ErrorActionPreference = "Stop"

$connectionString = "Server=localhost,1433;Database=GloboTicket;User Id=migration_user;Password=Migration@Pass123;TrustServerCertificate=True;Encrypt=True"

Write-Host "Restoring NuGet packages..." -ForegroundColor Cyan
dotnet restore

Write-Host "Applying database migrations..." -ForegroundColor Cyan

dotnet ef database update `
  --project src/GloboTicket.Infrastructure `
  --startup-project src/GloboTicket.API `
  --connection $connectionString

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migrations applied successfully!" -ForegroundColor Green
} else {
    Write-Error "Failed to apply migrations. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}

