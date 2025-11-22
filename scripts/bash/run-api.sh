#!/bin/bash
# Run the API server

set -e

echo "Starting API server..."
echo "Press Ctrl+C to stop"

cd src/GloboTicket.API
dotnet run

