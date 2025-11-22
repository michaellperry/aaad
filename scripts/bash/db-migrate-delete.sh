#!/bin/bash
# Delete a migration file after confirming it's not applied to database

set -e

if [ -z "$1" ]; then
    echo "Error: Migration name is required" >&2
    echo "Usage: $0 <MigrationName>" >&2
    exit 1
fi

MIGRATION_NAME="$1"

echo "Checking if migration '$MIGRATION_NAME' is applied to database..."

# Get list of applied migrations
MIGRATION_LIST=$(dotnet ef migrations list \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API 2>&1)

# Check if migration is in the applied list (marked with *)
if echo "$MIGRATION_LIST" | grep -q "^\*.*$MIGRATION_NAME"; then
    echo "Error: Migration '$MIGRATION_NAME' has been applied to the database. Cannot delete." >&2
    echo "You must rollback the migration first using db-migrate-rollback.sh" >&2
    exit 1
fi

# Check if migration file exists
MIGRATION_PATH="src/GloboTicket.Infrastructure/Data/Migrations"
if ! ls "$MIGRATION_PATH"/*_"$MIGRATION_NAME".cs 1> /dev/null 2>&1; then
    echo "Error: Migration file for '$MIGRATION_NAME' not found in $MIGRATION_PATH" >&2
    exit 1
fi

echo "Migration '$MIGRATION_NAME' is not applied. Proceeding with removal..."

# Remove the migration using EF Core (this handles multiple files)
dotnet ef migrations remove \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API

echo "Migration '$MIGRATION_NAME' removed successfully!"

