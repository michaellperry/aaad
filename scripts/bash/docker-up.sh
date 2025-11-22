#!/bin/bash
# Start Docker infrastructure

set -e

echo "Starting Docker infrastructure..."

docker compose -f docker/docker-compose.yml up -d

echo "Docker infrastructure started successfully!"
echo "Waiting for containers to become healthy..."
sleep 5
docker compose -f docker/docker-compose.yml ps

