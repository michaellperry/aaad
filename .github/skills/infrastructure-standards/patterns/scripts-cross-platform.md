# Cross-Platform Script Parity

Bash and PowerShell versions should offer the same behavior and checks.

## Bash: docker-up.sh
```bash
#!/bin/bash
set -e

if ! command -v docker >/dev/null; then
    echo "Docker is not installed or not in PATH"
    exit 1
fi

if ! docker info >/dev/null 2>&1; then
    echo "Docker daemon is not running"
    exit 1
fi

echo "Starting GloboTicket services..."
docker-compose up -d --build

timeout=120
counter=0
while [ $counter -lt $timeout ]; do
    if docker-compose ps | grep -q "Up (healthy)"; then
        echo "All services are healthy"
        docker-compose ps
        exit 0
    fi
    sleep 5
    counter=$((counter + 5))
    echo "Waiting... (${counter}s/${timeout}s)"
done

echo "Services failed to start within ${timeout}s"
docker-compose logs
exit 1
```

## PowerShell: docker-up.ps1
```powershell
param([switch]$Force)

$ErrorActionPreference = "Stop"

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "Docker is not installed or not in PATH" -ForegroundColor Red
    exit 1
}

try {
    docker info | Out-Null
} catch {
    Write-Host "Docker daemon is not running" -ForegroundColor Red
    exit 1
}

Write-Host "Starting GloboTicket services..." -ForegroundColor Green

$composeArgs = @("up", "-d")
if ($Force) { $composeArgs += "--build" }

docker-compose @composeArgs

$timeout = 120
$counter = 0
while ($counter -lt $timeout) {
    $status = docker-compose ps --format json | ConvertFrom-Json
    $healthy = $status | Where-Object { $_.State -eq "Up (healthy)" }
    if ($healthy.Count -eq $status.Count) {
        Write-Host "All services are healthy" -ForegroundColor Green
        docker-compose ps
        exit 0
    }
    Start-Sleep 5
    $counter += 5
    Write-Host "Waiting... (${counter}s/${timeout}s)" -ForegroundColor Yellow
}

Write-Host "Services failed to start within ${timeout}s" -ForegroundColor Red
docker-compose logs
exit 1
```
