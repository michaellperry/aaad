#!/bin/bash
# Run unit tests only

set -e

echo "Running unit tests..."

dotnet test tests/GloboTicket.UnitTests --verbosity normal

echo "Unit tests completed successfully!"

