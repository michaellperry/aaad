#!/bin/bash
# Drop and recreate database with all migrations

set -e

MIGRATION_CONNECTION_STRING="Server=localhost,1433;Database=GloboTicket;User Id=migration_user;Password=Migration@Pass123;TrustServerCertificate=True;Encrypt=True"
SA_CONNECTION_STRING="Server=localhost,1433;Database=master;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;Encrypt=True"

echo "=========================================="
echo "Database Reset Script"
echo "=========================================="
echo ""

# Step 1: Ensure migration_user has dbcreator role
echo "Step 1: Ensuring migration_user has dbcreator server role..."

# Try to grant permission, but don't fail if docker command doesn't work
GRANT_RESULT=$(docker compose -f docker/docker-compose.yml exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost \
  -U sa \
  -P 'YourStrong!Passw0rd' \
  -C \
  -Q "IF IS_SRVROLEMEMBER('dbcreator', 'migration_user') = 0 BEGIN ALTER SERVER ROLE dbcreator ADD MEMBER migration_user; SELECT 'GRANTED' AS Result; END ELSE BEGIN SELECT 'ALREADY_HAS' AS Result; END" \
  -h -1 \
  -W 2>/dev/null | tr -d '[:space:]' || echo "FAILED")

if echo "$GRANT_RESULT" | grep -q "GRANTED\|ALREADY_HAS"; then
  echo "✓ migration_user permissions verified"
elif [ "$GRANT_RESULT" = "FAILED" ]; then
  echo ""
  echo "⚠ Could not automatically grant permissions via Docker"
  echo ""
  echo "MANUAL FIX REQUIRED:"
  echo "Run one of these commands:"
  echo ""
  echo "Option 1 (Docker command):"
  echo "  docker compose -f docker/docker-compose.yml exec sqlserver /opt/mssql-tools18/bin/sqlcmd \\"
  echo "    -S localhost -U sa -P 'YourStrong!Passw0rd' -C \\"
  echo "    -Q \"ALTER SERVER ROLE dbcreator ADD MEMBER migration_user;\""
  echo ""
  echo "Option 2 (SQL script):"
  echo "  docker compose -f docker/docker-compose.yml exec sqlserver /opt/mssql-tools18/bin/sqlcmd \\"
  echo "    -S localhost -U sa -P 'YourStrong!Passw0rd' -C \\"
  echo "    -i /init-db/fix-permissions.sql"
  echo ""
  echo "Then retry this script."
  exit 1
fi

echo ""
echo "Step 2: Dropping database..."

# Use sa connection to drop database (sa always has permission)
dotnet ef database drop --force \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API \
  --connection "$SA_CONNECTION_STRING" 2>&1 | grep -v "Build started\|Build succeeded" || echo "Database drop completed (may not have existed)"

echo ""
echo "Step 3: Recreating database with all migrations..."

# Run migrations with migration_user
dotnet ef database update \
  --project src/GloboTicket.Infrastructure \
  --startup-project src/GloboTicket.API \
  --connection "$MIGRATION_CONNECTION_STRING"

echo ""
echo "=========================================="
echo "✓ Database reset completed successfully!"
echo "=========================================="

