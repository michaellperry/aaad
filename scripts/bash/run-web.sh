#!/bin/bash
# Run the web frontend

set -e

cd src/GloboTicket.Web

if [ ! -d "node_modules" ]; then
    echo "node_modules not found. Installing dependencies..."
    npm install
fi

echo "Starting web frontend..."
echo "Press Ctrl+C to stop"

npm run dev

