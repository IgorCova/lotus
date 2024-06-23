## BUILD
```
docker build --platform linux/amd64 -t emptyparam/lotus:latest .
docker build -t emptyparam/lotus:arm64-latest .
```
## PUSH
```
docker push emptyparam/lotus:latest
docker push emptyparam/lotus:arm64-latest
```

arm64 для запуска на маке m1

## CI / CD
Добавил ```.github/workflows/docker-hub.yml```

В нем идет автоматическая сборка и публикация дистрибутива в [dockerhub](https://hub.docker.com/repository/docker/emptyparam/lotus)