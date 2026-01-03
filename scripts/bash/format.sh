#!/bin/bash
# Format the solution to remove unnecessary imports and apply code style

set -e

echo "Running dotnet format on solution..."

dotnet format GloboTicket.sln

echo "Format completed successfully!"
