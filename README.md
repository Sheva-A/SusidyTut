# СусідиТут

Веб-сервіс підбору сусідів для спільної оренди житла.

**Вимоги:** Docker Desktop (Docker Compose v2).

## Запуск і зупинка

```bash
docker compose up -d --build
```

Сервіс: http://localhost:8080

```bash
docker compose down
```

## Тестові облікові записи

| Роль | Email | Пароль |
|---|---|---|
| Адміністратор | `admin@susidytut.local` | `Admin12345` |
| Користувачі | `demo1@susidytut.local` … `demo60@susidytut.local` | `Demo12345` |

## Резервне копіювання

```bash
docker compose exec db pg_dump -U postgres susidytut > backup.sql
```

## Команда

Анна Шевченко, Андрій Швальов, Віолетта Пархоменко (ТЦР-31, ДУІКТ).
