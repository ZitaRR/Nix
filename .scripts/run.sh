#!/usr/bin/env sh
set -e

TAG="${1:-latest}"
IMAGE_NAME="zitarr/nix:$TAG"
CONTAINER_NAME="nix"

echo "Pulling image ($IMAGE_NAME) from Docker Hub..."
docker pull "$IMAGE_NAME"

docker rm -f "$CONTAINER_NAME" 2>/dev/null || true

echo "Starting Nix..."
docker run \
    -e DISCORD_TOKEN \
    --name "$CONTAINER_NAME" \
    "$IMAGE_NAME" \
    env

echo "Nix exited."
