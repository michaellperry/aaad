#!/bin/bash
# Run integration tests only

set -e

echo "Running integration tests..."

dotnet test tests/GloboTicket.IntegrationTests --verbosity normal

echo "Integration tests completed successfully!"

