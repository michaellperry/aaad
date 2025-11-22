#!/bin/bash
# Apply all pending migrations to the database

set -e

CONNECTION_STRING="Server=localhost,1433;Database=GloboTicket;User Id=migration_user;Password=Migration@Pass123;TrustServerCertificate=True;Encrypt=True"

echo "Applying database migrations..."

dotnet ef database update \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API \
  --connection "$CONNECTION_STRING"

echo "Migrations applied successfully!"

