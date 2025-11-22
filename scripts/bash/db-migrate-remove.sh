#!/bin/bash
# Remove the last migration

set -e

echo "Removing last migration..."
echo "Note: This will only work if the migration hasn't been applied to any database."

dotnet ef migrations remove \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API

echo "Last migration removed successfully!"

