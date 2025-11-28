#!/usr/bin/env pwsh
# Start Docker infrastructure

$ErrorActionPreference = "Stop"

Write-Host "Starting Docker infrastructure..." -ForegroundColor Cyan

docker compose -f docker/docker-compose.yml up -d

if ($LASTEXITCODE -ne 0) {
    Write-Error "Failed to start Docker infrastructure. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "Waiting for SQL Server to become healthy..." -ForegroundColor Yellow
Write-Host "This may take 30-60 seconds..." -ForegroundColor Gray

# Wait for SQL Server to be healthy
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
    Write-Host "  Waiting... (${elapsed}s)" -ForegroundColor Gray
}

if (-not $healthy) {
    Write-Error "SQL Server did not become healthy within $maxWait seconds"
    docker compose -f docker/docker-compose.yml ps
    exit 1
}

Write-Host "Waiting for database initialization to complete..." -ForegroundColor Yellow
Write-Host "This may take a few seconds..." -ForegroundColor Gray

# Wait for init container to complete
$maxWait = 60
$elapsed = 0
$initComplete = $false

while ($elapsed -lt $maxWait) {
    # Check container status using docker inspect (more reliable than docker compose ps)
    $containerState = docker inspect globoticket-db-init --format='{{.State.Status}}' 2>$null
    
    if ($containerState -eq "exited") {
        $exitCode = docker inspect globoticket-db-init --format='{{.State.ExitCode}}' 2>$null
        if ($exitCode -eq "0") {
            Write-Host "✓ Database initialization completed successfully!" -ForegroundColor Green
            $initComplete = $true
            break
        } else {
            Write-Error "Database initialization failed with exit code: $exitCode"
            Write-Host "Check logs with: docker compose -f docker/docker-compose.yml logs db-init" -ForegroundColor Yellow
            exit 1
        }
    } elseif ($containerState -eq "running") {
        # Container is still running, keep waiting
        Start-Sleep -Seconds 2
        $elapsed += 2
        Write-Host "  Waiting... (${elapsed}s)" -ForegroundColor Gray
    } elseif ([string]::IsNullOrEmpty($containerState)) {
        # Container doesn't exist yet, keep waiting
        Start-Sleep -Seconds 2
        $elapsed += 2
        Write-Host "  Waiting... (${elapsed}s)" -ForegroundColor Gray
    } else {
        # Unexpected state
        Write-Error "Unexpected container state: $containerState"
        docker compose -f docker/docker-compose.yml ps
        exit 1
    }
}

if (-not $initComplete) {
    Write-Error "Database initialization did not complete within $maxWait seconds"
    Write-Host "Check logs with: docker compose -f docker/docker-compose.yml logs db-init" -ForegroundColor Yellow
    docker compose -f docker/docker-compose.yml ps
    exit 1
}

Write-Host ""
Write-Host "Docker infrastructure started successfully!" -ForegroundColor Green
Write-Host ""
docker compose -f docker/docker-compose.yml ps
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Run database migrations: pwsh scripts/powershell/db-update.ps1" -ForegroundColor White
Write-Host "  2. Start the API: pwsh scripts/powershell/run-api.ps1" -ForegroundColor White

