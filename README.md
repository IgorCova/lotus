## BUILD

docker build --platform linux/amd64 -t emptyparam/lotus:latest .

## PUSH
docker push emptyparam/lotus:latest

## CI / CD

Добавил .github/workflows/docker-hub.yml
В нем идет автоматическая сборка и публикация дистрибутива в [dockerhub](https://hub.docker.com/repository/docker/emptyparam/lotus)

Добавил .github/workflows/worker-docker-hub.yml
В нем идет автоматическая сборка и публикация дистрибутива в [dockerhub](https://hub.docker.com/repository/docker/emptyparam/lotus-worker)