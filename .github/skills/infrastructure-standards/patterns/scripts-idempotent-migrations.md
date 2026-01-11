# Idempotent Migration Script

Run migrations only when the database container is running; safe to execute repeatedly.

```bash
#!/bin/bash
DB_CONTAINER="globoticket-db"
DB_NAME="GloboTicket"

if ! docker ps | grep -q $DB_CONTAINER; then
    echo "Database container $DB_CONTAINER is not running"
    exit 1
fi

CURRENT_MIGRATION=$(docker exec $DB_CONTAINER psql -U globoticket -d $DB_NAME -t -c "SELECT MigrationId FROM __EFMigrationsHistory ORDER BY MigrationId DESC LIMIT 1;" 2>/dev/null | xargs)

if [ -z "$CURRENT_MIGRATION" ]; then
    echo "No migrations found, running initial migration..."
    dotnet ef database update --project src/GloboTicket.Infrastructure --startup-project src/GloboTicket.API
else
    echo "Current migration: $CURRENT_MIGRATION"
    echo "Checking for pending migrations..."
    dotnet ef database update --project src/GloboTicket.Infrastructure --startup-project src/GloboTicket.API
fi

echo "Database migration completed"
```
