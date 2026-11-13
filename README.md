# СусідиТут

Веб-сервіс підбору сусідів для спільної оренди житла. Студенти заповнюють анкету з побутовими звичками та бюджетом, сервіс автоматично знаходить сумісних кандидатів, рахує відсоток сумісності, а після прийняття запиту на знайомство відкриває контакти (Telegram, телефон).

## Можливості

- реєстрація, вхід і вихід;
- створення та редагування анкети;
- автоматичний підбір кандидатів із відсотком сумісності (до 50 найкращих);
- надсилання, прийняття та відхилення запитів, обмін контактами;
- адміністрування: статистика, пошук, блокування та розблокування користувачів.

## Вимоги до програмного забезпечення

- Docker Desktop 4.x або Docker Engine 24+ з Docker Compose v2;
- браузер Google Chrome, Microsoft Edge, Mozilla Firefox 120+ або Safari 17+;
- для розробки без Docker: .NET 10 SDK, PostgreSQL 17, інструмент `dotnet-ef` 10.0.x.

## Технології

.NET 10, ASP.NET Core Razor Pages, ASP.NET Core Identity, Entity Framework Core 10, PostgreSQL 17, Bootstrap 5.

## Запуск і зупинка

Запуск (з кореневої папки репозиторію):

```bash
docker compose up -d --build
```

Після запуску сервіс доступний за адресою http://localhost:8080. Міграції та початкові дані застосовуються автоматично.

Зупинка:

```bash
docker compose down
```

Зупинка з видаленням бази даних (чистий старт):

```bash
docker compose down -v
```

Журнал застосунку:

```bash
docker compose logs web
```

Покрокова інструкція для Rider (Docker Compose і налагодження): [docs/DEMO.md](docs/DEMO.md).

## Налаштування

Параметри задаються змінними середовища в `docker-compose.yml` (без перекомпіляції):

| Змінна | Значення за замовчуванням | Опис |
|---|---|---|
| `ConnectionStrings__Default` | `Host=db;Port=5432;Database=susidytut;Username=postgres;Password=postgres` | рядок підключення до БД |
| `Admin__Email` | `admin@susidytut.local` | email адміністратора |
| `Admin__Password` | `Admin12345` | пароль адміністратора |
| `DemoData__Enabled` | `true` | чи створювати демонстраційні анкети |
| `DemoData__ProfilesCount` | `60` | кількість демонстраційних анкет (для перевірки продуктивності — `500`) |

Демонстраційні дані створюються лише тоді, коли таблиця `Profiles` порожня.

## Тестові облікові записи

| Роль | Email | Пароль |
|---|---|---|
| Адміністратор | `admin@susidytut.local` | `Admin12345` |
| Користувачі (демо) | `demo1@susidytut.local` … `demo60@susidytut.local` | `Demo12345` |

Демо-користувачі з номером, кратним 5, мешкають у Львові, решта — у Києві.

## Резервне копіювання

Створення резервної копії:

```bash
docker compose exec db pg_dump -U postgres susidytut > backup.sql
```

Відновлення з резервної копії (у чисту базу):

```bash
docker compose exec -T db psql -U postgres susidytut < backup.sql
```

## Запуск без Docker (для розробки)

1. Запустіть PostgreSQL 17 (наприклад, лише контейнер БД із портом 5432 або локальний сервер).
2. Перевірте рядок підключення в `src/SusidyTut.Web/appsettings.json`.
3. Виконайте:

```bash
dotnet run --project src/SusidyTut.Web
```

Створення нової міграції:

```bash
dotnet ef migrations add <Назва> --project src/SusidyTut.Web --output-dir Data/Migrations
```

## Структура проєкту

```
SusidyTut/
  SusidyTut.sln
  docker-compose.yml
  README.md
  src/SusidyTut.Web/
    Data/       ApplicationDbContext, DbInitializer, Migrations/
    Models/     сутності, переліки, моделі представлення
    Services/   бізнес-логіка та інтерфейси сервісів
    Pages/      Account/, Profile/, Search/, Requests/, Admin/, Shared/
    wwwroot/    стилі та бібліотеки шаблону (Bootstrap, jQuery)
    Program.cs
    Dockerfile
```

## Склад команди

- Анна Шевченко
- Андрій Швальов
- Віолетта Пархоменко

Група ТЦР-31, ДУІКТ, 2026.
