#!/usr/bin/env pwsh
# Clean build artifacts

$ErrorActionPreference = "Stop"

Write-Host "Cleaning build artifacts..." -ForegroundColor Cyan

dotnet clean

if ($LASTEXITCODE -eq 0) {
    Write-Host "Clean completed successfully!" -ForegroundColor Green
} else {
    Write-Error "Clean failed. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}

