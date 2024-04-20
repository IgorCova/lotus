## Работа с Helm-ом

### Обнулим minikube если нужно
minikube delete

### стартуем minikube
minikube start

### запускаем дашборд кубера
minikube dashboard

### для удобства создаем алиас для работы с kubectl
alias k="minikube kubectl --"

### включаем ingress аддон от NGINX
minikube addons enable ingress 

### проверяем что ingress NGINX запустился
kubectl get pods -n ingress-nginx

### стартуем тунель чтобы подключаться к нашему сервису локально
minikube tunnel  

### Команда для запуска сервиса
helm upgrade --install lotus ./lotus --values lotus/values.yaml

### БД устанавливается из helm, вместе с файлом values.yaml

### Postman коллекция в этом папке
### Запустить коллекцию командой
newman run Otus.postman_collection.json

### Результат запуска коллекции
→ Create user
POST http://arch.homework/user [200 OK, 156B, 66ms]
✓  Status code is 200

→ Get all users
GET http://arch.homework/user/all [200 OK, 1.4kB, 22ms]
✓  Status code is 200
✓  set userId

→ Get user by ID
GET http://arch.homework/user/fe110b52-c56f-4438-8f8b-d361c573c603 [200 OK, 279B, 11ms]
✓  Status code is 200

→ Update user by ID
PUT http://arch.homework/user/fe110b52-c56f-4438-8f8b-d361c573c603 [200 OK, 156B, 12ms]
✓  Status code is 200

→ Delete user by ID
DELETE http://arch.homework/user/fe110b52-c56f-4438-8f8b-d361c573c603 [200 OK, 156B, 52ms]
✓  Status code is 200

┌─────────────────────────┬───────────────────┬──────────────────┐
│                         │          executed │           failed │
├─────────────────────────┼───────────────────┼──────────────────┤
│              iterations │                 1 │                0 │
├─────────────────────────┼───────────────────┼──────────────────┤
│                requests │                 5 │                0 │
├─────────────────────────┼───────────────────┼──────────────────┤
│            test-scripts │                 5 │                0 │
├─────────────────────────┼───────────────────┼──────────────────┤
│      prerequest-scripts │                 0 │                0 │
├─────────────────────────┼───────────────────┼──────────────────┤
│              assertions │                 6 │                0 │
├─────────────────────────┴───────────────────┴──────────────────┤
│ total run duration: 272ms                                      │
├────────────────────────────────────────────────────────────────┤
│ total data received: 1.38kB (approx)                           │
├────────────────────────────────────────────────────────────────┤
│ average response time: 32ms [min: 11ms, max: 66ms, s.d.: 22ms] │
└────────────────────────────────────────────────────────────────┘