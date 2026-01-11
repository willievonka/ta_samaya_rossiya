# Frontend — TaSamayaRossiya

### О проекте
- Фреймворк: Angular 21
- UI: Taiga UI
- Карты: Leaflet + GeoJSON

---

### Требования
- Node.js 24.x
- npm
- Angular CLI (можно использовать локально или глобально)

---

### Установка зависимостей
- cd frontend
- npm install

---

### Запуск в режиме разработки
- Локально (hot-reload): `ng serve`
- Для запуска в Docker dev (из Dockerfile.dev): `docker compose -f docker-compose.dev.yml up --build`
  - При запуске через Docker frontend доступен по `http://localhost:4200`

---

### Продакшн-сборка и Docker
- Сборка прод: `npm run build` — результат в `dist/ta_samaya_rossiya`
- Docker (prod) собирает приложение и кладёт в nginx:
  - docker compose up --build -d  (использует Dockerfile и `nginx`)
  - В прод-режиме фронт доступен на `http://localhost` (порт 80)

---

### Прокси & API
- В dev-режиме используется proxy.conf.json для проксирования запросов к backend.
- Проверьте, что backend доступен и настроен корректно (см. README.md).

