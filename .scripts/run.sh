IMAGE_NAME="zitarr/nix:latest"
CONTAINER_NAME="nix"

echo "Pulling latest image from Docker Hub..."
docker pull $IMAGE_NAME

if [$(docker ps -aq -f name=$CONTAINER_NAME)]; then
    echo "Stopping existing container..."
    docker stop $CONTAINER_NAME
    docker rm $CONTAINER_NAME
    echo "Stopped existing container."
fi

echo "Starting Nix container..."
docker run -it \
    --name $CONTAINER_NAME \
    $IMAGE_NAME

echo "Nix started!"