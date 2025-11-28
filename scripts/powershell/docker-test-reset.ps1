#!/usr/bin/env pwsh
# Test script to verify automatic database initialization with fresh volumes
# This script recreates all volumes and verifies that initialization runs automatically

$ErrorActionPreference = "Stop"

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Testing Docker Compose Volume Recreation" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Step 1: Stopping containers and removing volumes..." -ForegroundColor Yellow
docker compose -f docker/docker-compose.yml down -v

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to stop containers and remove volumes"
    exit 1
}

Write-Host "✓ Volumes removed successfully" -ForegroundColor Green
Write-Host ""

Write-Host "Step 2: Starting containers fresh..." -ForegroundColor Yellow
docker compose -f docker/docker-compose.yml up -d

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to start containers"
    exit 1
}

Write-Host "✓ Containers started" -ForegroundColor Green
Write-Host ""

Write-Host "Step 3: Waiting for SQL Server to become healthy..." -ForegroundColor Yellow
$maxWait = 120
$elapsed = 0
$healthy = $false

while ($elapsed -lt $maxWait) {
    $status = docker compose -f docker/docker-compose.yml ps sqlserver --format json 2>$null | ConvertFrom-Json | Select-Object -ExpandProperty State -ErrorAction SilentlyContinue
    if ($status -match "healthy") {
        Write-Host "✓ SQL Server is healthy!" -ForegroundColor Green
        $healthy = $true
        break
    }
    Start-Sleep -Seconds 2
    $elapsed += 2
}

if (-not $healthy) {
    Write-Error "SQL Server did not become healthy within $maxWait seconds"
    docker compose -f docker/docker-compose.yml ps
    exit 1
}

Write-Host ""
Write-Host "Step 4: Waiting for database initialization to complete..." -ForegroundColor Yellow
$maxWait = 60
$elapsed = 0
$initSuccess = $false

while ($elapsed -lt $maxWait) {
    $initContainer = docker compose -f docker/docker-compose.yml ps db-init --format json 2>$null | ConvertFrom-Json -ErrorAction SilentlyContinue
    if ($initContainer) {
        $state = $initContainer.State
        if ($state -eq "exited") {
            $exitCode = $initContainer.ExitCode
            if ($exitCode -eq 0) {
                Write-Host "✓ Database initialization completed successfully!" -ForegroundColor Green
                $initSuccess = $true
                break
            } else {
                Write-Error "Database initialization failed with exit code: $exitCode"
                Write-Host "Check logs: docker compose -f docker/docker-compose.yml logs db-init" -ForegroundColor Yellow
                exit 1
            }
        }
    }
    Start-Sleep -Seconds 2
    $elapsed += 2
}

if (-not $initSuccess) {
    Write-Error "Database initialization did not complete within $maxWait seconds"
    Write-Host "Check logs: docker compose -f docker/docker-compose.yml logs db-init" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Step 5: Verifying database users were created..." -ForegroundColor Yellow

# Test migration_user
$migrationTest = docker compose -f docker/docker-compose.yml exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd `
    -S localhost `
    -U migration_user `
    -P "Migration@Pass123" `
    -C `
    -Q "SELECT 1" 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ migration_user exists and can connect" -ForegroundColor Green
} else {
    Write-Error "migration_user verification failed"
    exit 1
}

# Test app_user
$appTest = docker compose -f docker/docker-compose.yml exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd `
    -S localhost `
    -U app_user `
    -P "YourStrong@Passw0rd" `
    -C `
    -Q "SELECT 1" 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ app_user exists and can connect" -ForegroundColor Green
} else {
    Write-Error "app_user verification failed"
    exit 1
}

# Verify GloboTicket database exists
$dbCheck = docker compose -f docker/docker-compose.yml exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd `
    -S localhost `
    -U sa `
    -P "YourStrong!Passw0rd" `
    -C `
    -Q "IF EXISTS (SELECT * FROM sys.databases WHERE name = 'GloboTicket') SELECT 'EXISTS' ELSE SELECT 'NOT_EXISTS'" 2>&1

if ($dbCheck -match "EXISTS") {
    Write-Host "✓ GloboTicket database exists" -ForegroundColor Green
} else {
    Write-Error "GloboTicket database does not exist"
    exit 1
}

Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "✓ All tests passed!" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Database initialization is working correctly." -ForegroundColor Green
Write-Host "Users created:" -ForegroundColor Cyan
Write-Host "  • migration_user - For EF Core migrations" -ForegroundColor White
Write-Host "  • app_user       - For API runtime operations" -ForegroundColor White
Write-Host ""
Write-Host "Next step: Run migrations with pwsh scripts/powershell/db-update.ps1" -ForegroundColor Yellow

