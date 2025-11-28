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
INIT_COMPLETED=false

while [ $ELAPSED -lt $MAX_WAIT ]; do
    # Check container status using docker inspect (more reliable than docker compose ps)
    CONTAINER_STATE=$(docker inspect globoticket-db-init --format='{{.State.Status}}' 2>/dev/null || echo "")
    
    if [ "$CONTAINER_STATE" = "exited" ]; then
        INIT_EXIT_CODE=$(docker inspect globoticket-db-init --format='{{.State.ExitCode}}' 2>/dev/null || echo "")
        if [ "$INIT_EXIT_CODE" = "0" ]; then
            echo "✓ Database initialization completed successfully!"
            INIT_COMPLETED=true
            break
        else
            echo "✗ Database initialization failed with exit code: $INIT_EXIT_CODE"
            echo "Check logs with: docker compose -f docker/docker-compose.yml logs db-init"
            exit 1
        fi
    elif [ "$CONTAINER_STATE" = "running" ]; then
        # Container is still running, keep waiting
        sleep 2
        ELAPSED=$((ELAPSED + 2))
        echo "  Waiting... (${ELAPSED}s)"
    elif [ -z "$CONTAINER_STATE" ]; then
        # Container doesn't exist yet, keep waiting
        sleep 2
        ELAPSED=$((ELAPSED + 2))
        echo "  Waiting... (${ELAPSED}s)"
    else
        # Unexpected state
        echo "✗ Unexpected container state: $CONTAINER_STATE"
        docker compose -f docker/docker-compose.yml ps
        exit 1
    fi
done

if [ "$INIT_COMPLETED" = false ]; then
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

