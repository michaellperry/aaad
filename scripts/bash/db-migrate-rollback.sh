#!/bin/bash
# Rollback to a specific migration (use "0" to rollback all migrations)

set -e

if [ -z "$1" ]; then
    echo "Error: Target migration is required" >&2
    echo "Usage: $0 <TargetMigration> (use '0' to rollback all migrations)" >&2
    exit 1
fi

TARGET_MIGRATION="$1"

CONNECTION_STRING="Server=localhost,1433;Database=GloboTicket;User Id=migration_user;Password=Migration@Pass123;TrustServerCertificate=True;Encrypt=True"

if [ "$TARGET_MIGRATION" = "0" ]; then
    echo "Rolling back all migrations..."
else
    echo "Rolling back to migration: $TARGET_MIGRATION"
fi

dotnet ef database update "$TARGET_MIGRATION" \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API \
  --connection "$CONNECTION_STRING"

echo "Rollback completed successfully!"

