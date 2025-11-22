#!/bin/bash
# Run tests (takes optional filter: "unit", "integration", or "all")

set -e

FILTER="${1:-all}"

case "$FILTER" in
    unit)
        echo "Running unit tests..."
        dotnet test tests/GloboTicket.UnitTests --verbosity normal
        ;;
    integration)
        echo "Running integration tests..."
        dotnet test tests/GloboTicket.IntegrationTests --verbosity normal
        ;;
    all|*)
        echo "Running all tests..."
        dotnet test --verbosity normal
        ;;
esac

echo "Tests completed successfully!"

