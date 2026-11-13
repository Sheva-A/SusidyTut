# Як запустити демо «СусідиТут» через Rider

Проєкт лежить у WSL: `\\wsl.localhost\Ubuntu\home\sheva_a\projects\SusidyTut`.

Є два способи:

- **Спосіб А — Docker Compose.** Для показу демо. Усе піднімається однією кнопкою, .NET SDK не потрібен.
- **Спосіб Б — застосунок з Rider, база в Docker.** Для налагодження: точки зупину, гаряче перезавантаження.

---

## Перед першим запуском (один раз)

1. **Docker Desktop** → *Settings → Resources → WSL integration* → увімкніть перемикач для **Ubuntu** → *Apply & restart*.
2. **Rider** → *File → Open* → вставте шлях
   `\\wsl.localhost\Ubuntu\home\sheva_a\projects\SusidyTut\SusidyTut.sln`.
3. **Підключення Docker у Rider:** *Settings → Build, Execution, Deployment → Docker* → **+** → *Docker for Windows* → *OK*. Унизу має з'явитися «Connection successful».

---

## Спосіб А: демо через Docker Compose (рекомендовано)

### Запуск

1. Запустіть **Docker Desktop** і дочекайтеся зеленого статусу «Engine running».
2. У Rider відкрийте файл `docker-compose.yml`.
3. Натисніть зелену подвійну стрілку **▶▶** на полях біля рядка `services:` → **Run 'docker-compose.yml: Compose Deployment'**.
4. Відкриється вікно **Services** (`Alt+8`): там видно контейнери `db` і `web` та їхні журнали.
5. Через 10–30 секунд відкрийте в браузері **http://localhost:8080**.

> Перше збирання триває кілька хвилин (завантажуються образи .NET і PostgreSQL). Наступні запуски — секунди.

Щоб образ перезбирався після змін у коді: *Run → Edit Configurations…* → ваша конфігурація Compose → у полі **Build** оберіть **Always** (те саме, що `--build`).

### Зупинка

У вікні **Services** натисніть правою кнопкою на `docker-compose.yml` → **Down**.
Дані (користувачі, анкети, запити) зберігаються до наступного запуску.

### Те саме з терміналу Rider (`Alt+F12`)

```bash
docker compose up -d --build   # запуск
docker compose down            # зупинка, дані зберігаються
docker compose down -v         # зупинка з повним очищенням бази
docker compose logs web        # журнал застосунку
```

---

## Спосіб Б: застосунок з Rider (для налагодження)

Потрібен .NET 10 SDK. Найпростіше відкрити проєкт через **WSL-середовище Rider**: *File → Remote Development → WSL → Ubuntu* → оберіть `SusidyTut.sln`. Тоді Rider використовує SDK, вже встановлений у WSL.

> **Важливо:** у WSL є два .NET — системний `/usr/bin/dotnet` (лише SDK 8) і `~/.dotnet` (SDK 10). Перевірте, що Rider використовує правильний: *Settings → Build, Execution, Deployment → Toolset and Build* → **.NET CLI executable path** = `/home/sheva_a/.dotnet/dotnet`.

1. Запустіть лише базу:
   ```bash
   docker compose up -d db
   ```
2. Угорі Rider оберіть конфігурацію **SusidyTut.Web: http** і натисніть **▶ Run** (або **🐞 Debug**).
3. Сайт відкриється на **http://localhost:5153**.

Рядок підключення для цього режиму — у `src/SusidyTut.Web/appsettings.json` (`Host=localhost;Port=5432`). Міграції та демо-дані застосовуються автоматично під час старту.

> Якщо контейнер `web` теж запущений — це не заважає: обидва працюють з тією самою базою (порти 8080 і 5153).

---

## Облікові записи для демо

| Роль | Email | Пароль |
|---|---|---|
| Адміністратор | `admin@susidytut.local` | `Admin12345` |
| Користувачі | `demo1@susidytut.local` … `demo60@susidytut.local` | `Demo12345` |

Демо-користувачі з номером, кратним 5 (`demo5`, `demo10`, …), живуть у Львові, решта — у Києві.

## Сценарій показу (з розділу 7 ТЗ)

1. Відкрийте http://localhost:8080 і зареєструйте користувача А. В анкеті вкажіть місто «Київ» і бюджет від 4000 до 7000.
2. На сторінці «Пошук сусідів» покажіть, що кандидати впорядковані за відсотком сумісності. Відкрийте одну анкету й перевірте бали в таблиці.
3. У вікні інкогніто (`Ctrl+Shift+N`) зареєструйте користувача Б із сумісною анкетою.
4. А надсилає запит Б, Б приймає його на сторінці «Запити».
5. Обидва бачать контакти одне одного; контакти інших кандидатів приховані.
6. Увійдіть як адміністратор, покажіть статистику й заблокуйте Б. Б зникає з пошуку А.

Можна також увійти як `demo1@susidytut.local`: у нього вже є кандидати.

## Чистий старт (скинути всі дані)

```bash
docker compose down -v
docker compose up -d --build
```

Будуть створені заново лише адміністратор і 60 демо-анкет.

## Як видалити окремого користувача

Через інтерфейс видалити обліковий запис не можна (ДВ-07). Заблокувати користувача можна на сторінці «Адміністрування». Якщо його треба саме видалити, виконайте в терміналі (замініть email):

```bash
docker compose exec -T db psql -U postgres susidytut -c "
BEGIN;
DELETE FROM \"ContactRequests\" WHERE \"SenderId\" IN (SELECT \"Id\" FROM \"AspNetUsers\" WHERE \"NormalizedEmail\" = UPPER('max@test.com'))
                               OR \"ReceiverId\" IN (SELECT \"Id\" FROM \"AspNetUsers\" WHERE \"NormalizedEmail\" = UPPER('max@test.com'));
DELETE FROM \"AspNetUsers\" WHERE \"NormalizedEmail\" = UPPER('max@test.com');
COMMIT;"
```

Анкета та роль видаляються автоматично (каскадно). **Дія незворотна.**

## Якщо щось не працює

| Проблема | Що зробити |
|---|---|
| У Rider порожня панель файлів («Nothing to show») | Збій ініціалізації панелі в режимі WSL (проєкт при цьому завантажений). Двічі натисніть `Alt+1` (закрити й відкрити панель) і у випадному списку вгорі панелі оберіть **Solution**. Не допомогло — закрийте вікно Rider і відкрийте проєкт знову через *File → Recent Projects* |
| `docker: command not found` у терміналі | Docker Desktop не запущено або вимкнено WSL integration для Ubuntu |
| `port is already allocated` (8080 або 5432) | Порт зайнятий іншою програмою (наприклад, локальним PostgreSQL). Зупиніть її або змініть лівий номер порту в `docker-compose.yml` |
| Сторінка не відкривається одразу після запуску | Зачекайте 20–30 с і перегляньте журнал: `docker compose logs web` |
| Після зміни коду нічого не змінилося | Запускайте з `--build` (або **Build: Always** у Rider) |
