#!/usr/bin/env pwsh
# Run tests (takes optional filter: "unit", "integration", or "all")

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("unit", "integration", "all")]
    [string]$Filter = "all"
)

$ErrorActionPreference = "Stop"

switch ($Filter) {
    "unit" {
        Write-Host "Running unit tests..." -ForegroundColor Cyan
        dotnet test tests/GloboTicket.UnitTests --verbosity normal
    }
    "integration" {
        Write-Host "Running integration tests..." -ForegroundColor Cyan
        dotnet test tests/GloboTicket.IntegrationTests --verbosity normal
    }
    default {
        Write-Host "Running all tests..." -ForegroundColor Cyan
        dotnet test --verbosity normal
    }
}

if ($LASTEXITCODE -eq 0) {
    Write-Host "Tests completed successfully!" -ForegroundColor Green
} else {
    Write-Error "Tests failed. Exit code: $LASTEXITCODE"
    exit $LASTEXITCODE
}

