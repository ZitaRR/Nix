#!/usr/bin/env sh
set -e

IMAGE_NAME="zitarr/nix:latest"
CONTAINER_NAME="nix"

echo "Pulling latest image from Docker Hub..."
docker pull "$IMAGE_NAME"

docker rm -f "$CONTAINER_NAME" 2>/dev/null || true

echo "Starting Nix container..."
docker run -it --rm \
    --name "$CONTAINER_NAME" \
    "$IMAGE_NAME"

echo "Nix exited."
