#!/bin/bash
# Add a new migration

set -e

if [ -z "$1" ]; then
    echo "Error: Migration name is required" >&2
    echo "Usage: $0 <MigrationName>" >&2
    exit 1
fi

MIGRATION_NAME="$1"

echo "Adding migration: $MIGRATION_NAME"

dotnet ef migrations add "$MIGRATION_NAME" \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API \
  --output-dir Data/Migrations

echo "Migration '$MIGRATION_NAME' created successfully!"

