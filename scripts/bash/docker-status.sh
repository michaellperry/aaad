#!/bin/bash
# Check Docker container status

set -e

echo "Checking Docker container status..."

docker compose -f docker/docker-compose.yml ps

