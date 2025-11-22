#!/bin/bash
# Drop and recreate database with all migrations

set -e

CONNECTION_STRING="Server=localhost,1433;Database=GloboTicket;User Id=migration_user;Password=Migration@Pass123;TrustServerCertificate=True;Encrypt=True"

echo "Dropping database..."

# Ignore errors from drop (database might not exist)
dotnet ef database drop --force \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API \
  --connection "$CONNECTION_STRING" || echo "Database drop may have failed or database didn't exist. Continuing..."

echo "Recreating database with all migrations..."

dotnet ef database update \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API \
  --connection "$CONNECTION_STRING"

echo "Database reset completed successfully!"

