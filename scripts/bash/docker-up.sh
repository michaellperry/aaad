#!/bin/bash
# Start Docker infrastructure

set -e

echo "Starting Docker infrastructure..."

docker compose -f docker/docker-compose.yml up -d

echo "Waiting for SQL Server to become healthy..."
echo "This may take 30-60 seconds..."

# Wait for SQL Server to be healthy
MAX_WAIT=120
ELAPSED=0
while [ $ELAPSED -lt $MAX_WAIT ]; do
    if docker compose -f docker/docker-compose.yml ps sqlserver | grep -q "healthy"; then
        echo "✓ SQL Server is healthy!"
        break
    fi
    sleep 2
    ELAPSED=$((ELAPSED + 2))
    echo "  Waiting... (${ELAPSED}s)"
done

if [ $ELAPSED -ge $MAX_WAIT ]; then
    echo "✗ SQL Server did not become healthy within ${MAX_WAIT} seconds"
    docker compose -f docker/docker-compose.yml ps
    exit 1
fi

echo "Waiting for database initialization to complete..."
echo "This may take a few seconds..."

# Wait for init container to complete
MAX_WAIT=60
ELAPSED=0
while [ $ELAPSED -lt $MAX_WAIT ]; do
    INIT_STATUS=$(docker compose -f docker/docker-compose.yml ps db-init --format json 2>/dev/null | grep -o '"State":"[^"]*"' | cut -d'"' -f4 || echo "")
    if [ "$INIT_STATUS" = "exited" ]; then
        INIT_EXIT_CODE=$(docker compose -f docker/docker-compose.yml ps db-init --format json 2>/dev/null | grep -o '"ExitCode":[0-9]*' | cut -d':' -f2 || echo "")
        if [ "$INIT_EXIT_CODE" = "0" ]; then
            echo "✓ Database initialization completed successfully!"
            break
        else
            echo "✗ Database initialization failed with exit code: $INIT_EXIT_CODE"
            echo "Check logs with: docker compose -f docker/docker-compose.yml logs db-init"
            exit 1
        fi
    fi
    sleep 2
    ELAPSED=$((ELAPSED + 2))
    echo "  Waiting... (${ELAPSED}s)"
done

if [ $ELAPSED -ge $MAX_WAIT ]; then
    echo "✗ Database initialization did not complete within ${MAX_WAIT} seconds"
    echo "Check logs with: docker compose -f docker/docker-compose.yml logs db-init"
    docker compose -f docker/docker-compose.yml ps
    exit 1
fi

echo ""
echo "Docker infrastructure started successfully!"
echo ""
docker compose -f docker/docker-compose.yml ps
echo ""
echo "Next steps:"
echo "  1. Run database migrations: ./scripts/bash/db-update.sh"
echo "  2. Start the API: ./scripts/bash/run-api.sh"

