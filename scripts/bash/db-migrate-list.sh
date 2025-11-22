#!/bin/bash
# List all migrations and their applied status

set -e

echo "Listing migrations and their status..."

dotnet ef migrations list \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API

