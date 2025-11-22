#!/bin/bash
# Stop Docker infrastructure

set -e

echo "Stopping Docker infrastructure..."

docker compose -f docker/docker-compose.yml down

echo "Docker infrastructure stopped successfully!"

