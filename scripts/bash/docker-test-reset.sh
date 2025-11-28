#!/bin/bash
# Test script to verify automatic database initialization with fresh volumes
# This script recreates all volumes and verifies that initialization runs automatically

set -e

echo "=========================================="
echo "Testing Docker Compose Volume Recreation"
echo "=========================================="
echo ""

echo "Step 1: Stopping containers and removing volumes..."
docker compose -f docker/docker-compose.yml down -v

if [ $? -ne 0 ]; then
    echo "✗ Failed to stop containers and remove volumes"
    exit 1
fi

echo "✓ Volumes removed successfully"
echo ""

echo "Step 2: Starting containers fresh..."
docker compose -f docker/docker-compose.yml up -d

if [ $? -ne 0 ]; then
    echo "✗ Failed to start containers"
    exit 1
fi

echo "✓ Containers started"
echo ""

echo "Step 3: Waiting for SQL Server to become healthy..."
MAX_WAIT=120
ELAPSED=0
while [ $ELAPSED -lt $MAX_WAIT ]; do
    if docker compose -f docker/docker-compose.yml ps sqlserver | grep -q "healthy"; then
        echo "✓ SQL Server is healthy!"
        break
    fi
    sleep 2
    ELAPSED=$((ELAPSED + 2))
done

if [ $ELAPSED -ge $MAX_WAIT ]; then
    echo "✗ SQL Server did not become healthy within ${MAX_WAIT} seconds"
    docker compose -f docker/docker-compose.yml ps
    exit 1
fi

echo ""
echo "Step 4: Waiting for database initialization to complete..."
MAX_WAIT=60
ELAPSED=0
INIT_SUCCESS=false

while [ $ELAPSED -lt $MAX_WAIT ]; do
    INIT_STATUS=$(docker compose -f docker/docker-compose.yml ps db-init --format json 2>/dev/null | grep -o '"State":"[^"]*"' | cut -d'"' -f4 || echo "")
    if [ "$INIT_STATUS" = "exited" ]; then
        INIT_EXIT_CODE=$(docker compose -f docker/docker-compose.yml ps db-init --format json 2>/dev/null | grep -o '"ExitCode":[0-9]*' | cut -d':' -f2 || echo "")
        if [ "$INIT_EXIT_CODE" = "0" ]; then
            echo "✓ Database initialization completed successfully!"
            INIT_SUCCESS=true
            break
        else
            echo "✗ Database initialization failed with exit code: $INIT_EXIT_CODE"
            echo "Check logs: docker compose -f docker/docker-compose.yml logs db-init"
            exit 1
        fi
    fi
    sleep 2
    ELAPSED=$((ELAPSED + 2))
done

if [ "$INIT_SUCCESS" = false ]; then
    echo "✗ Database initialization did not complete within ${MAX_WAIT} seconds"
    echo "Check logs: docker compose -f docker/docker-compose.yml logs db-init"
    exit 1
fi

echo ""
echo "Step 5: Verifying database users were created..."

# Test migration_user
if docker compose -f docker/docker-compose.yml exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost \
    -U migration_user \
    -P "Migration@Pass123" \
    -C \
    -Q "SELECT 1" \
    > /dev/null 2>&1; then
    echo "✓ migration_user exists and can connect"
else
    echo "✗ migration_user verification failed"
    exit 1
fi

# Test app_user
if docker compose -f docker/docker-compose.yml exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost \
    -U app_user \
    -P "YourStrong@Passw0rd" \
    -C \
    -Q "SELECT 1" \
    > /dev/null 2>&1; then
    echo "✓ app_user exists and can connect"
else
    echo "✗ app_user verification failed"
    exit 1
fi

# Verify GloboTicket database exists
if docker compose -f docker/docker-compose.yml exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd \
    -S localhost \
    -U sa \
    -P "YourStrong!Passw0rd" \
    -C \
    -Q "IF EXISTS (SELECT * FROM sys.databases WHERE name = 'GloboTicket') SELECT 'EXISTS' ELSE SELECT 'NOT_EXISTS'" \
    | grep -q "EXISTS"; then
    echo "✓ GloboTicket database exists"
else
    echo "✗ GloboTicket database does not exist"
    exit 1
fi

echo ""
echo "=========================================="
echo "✓ All tests passed!"
echo "=========================================="
echo ""
echo "Database initialization is working correctly."
echo "Users created:"
echo "  • migration_user - For EF Core migrations"
echo "  • app_user       - For API runtime operations"
echo ""
echo "Next step: Run migrations with ./scripts/bash/db-update.sh"

