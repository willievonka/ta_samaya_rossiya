# Backend — TaSamayaRossiya

### О проекте ℹ️
- Ядро: ASP.NET Core на .NET 8
- ORM: Entity Framework Core + Npgsql (с поддержкой NetTopologySuite для геоданных)
- DB: PostgreSQL + PostGIS
- Миграции хранятся в папке `src/Infrastructure/Migrations`. При старте приложение пытается автоматически выполнить миграции (см. `InfrastructureStartup`).

---

### Требования (локально)
- .NET SDK 8.x (https://dotnet.microsoft.com/)
- Docker & Docker Compose (если вы используете контейнеры)
- (Опционально) PostgreSQL с PostGIS, если не используете Docker

---

### Переменные окружения
Файл конфигурации: .env (используется docker-compose). Не храните секреты в публичных репозиториях.

Основные переменные:
- CONNECTIONSTRINGS__MapDBConnection — строка подключения (например: Host=db;Port=5432;Database=ta-samaya-rossiya;Username=postgres;Password=...)
- POSTGRES_DB, POSTGRES_USER, POSTGRES_PASSWORD, POSTGRES_HOST, POSTGRES_PORT
- Jwt__Key, Jwt__Issuer, Jwt__Audience, Jwt__AccessTokenExpirationMinutes, Jwt__RefreshTokenExpirationHours
- В .env могут быть добавлены seed-администраторы (ADMIN__0__EMAIL и т.д.)

---

### Запуск в режиме разработки (локально)
- Если вы хотите запускать бэкенд локально (на машине):
  - cd backend
  - dotnet restore
  - dotnet run --project WebApi.csproj
- Или используйте Docker Compose (рекомендуется):
  - docker compose -f docker-compose.dev.yml up --build
  - Backend будет доступен на `http://localhost:8080`

---

### Сборка и запуск в прод (Docker)
- docker compose up --build -d (использует docker-compose.yml)
- В прод-режиме backend не экспортирует порт на хост — он доступен внутри сети Docker (frontend взаимодействует с backend внутри Docker).

---

### Миграции и инициализация БД
- Миграции хранятся в `src/Infrastructure/Migrations`
- Приложение автоматически пытается применить миграции при старте. Если вы хотите применить вручную:
  - dotnet tool install --global dotnet-ef
  - dotnet ef database update --project src/Infrastructure --startup-project src/WebApi

---

### Тесты
- unit & integration tests:
  - cd backend
  - dotnet test Tests.csproj

---

### Отладка и логирование
- Конфигурация логирования находится в `src/WebApi` (Serilog используется для логирования).
- В случае проблем с миграциями убедитесь, что Postgres запущен и доступен по строке подключения.
