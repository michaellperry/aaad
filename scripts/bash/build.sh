#!/bin/bash
# Build the solution

set -e

echo "Building solution..."

dotnet build GloboTicket.sln

echo "Build completed successfully!"

