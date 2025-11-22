#!/bin/bash
# View Docker logs (takes optional service name)

set -e

if [ -n "$1" ]; then
    echo "Viewing logs for service: $1"
    docker compose -f docker/docker-compose.yml logs "$1"
else
    echo "Viewing all Docker logs..."
    docker compose -f docker/docker-compose.yml logs
fi

