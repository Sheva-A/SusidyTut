# CLAUDE.md — правила проєкту «СусідиТут»

MVP веб-сервісу підбору сусідів для спільної оренди житла. Джерело вимог — ТЗ (`ФІ-NN`, `ФК-NN`, `ФА-NN`, `ДВ-NN`) і прототип Figma (`Е-01`…`Е-09`).

## Стек (не змінювати)
- .NET 10, C# 14, ASP.NET Core Razor Pages (без вбудованого Identity UI — свої сторінки `Account/` українською через `UserManager`/`SignInManager`).
- PostgreSQL 17, EF Core 10 (`Npgsql.EntityFrameworkCore.PostgreSQL`), Code-First, `Database.Migrate()` під час старту.
- Bootstrap 5, jQuery, jQuery Validation (+Unobtrusive) — лише локально з `wwwroot/lib`, без CDN.
- **Власний JavaScript заборонено.** `Html.Raw` для даних користувачів заборонено. SQL вручну не пишемо — лише LINQ.

## Архітектура (ДВ-06)
- `Pages/` — `PageModel` лише приймає дані, викликає сервіс і повертає сторінку. **Жодного `ApplicationDbContext` у Pages.**
- `Services/` — `IProfileService`, `IMatchingService`, `IContactRequestService`, `IAdminService` (+ реалізації), реєстрація `Scoped`.
- `Data/` — `ApplicationDbContext : IdentityDbContext<ApplicationUser>`, `DbInitializer`, `Migrations/`.
- `MatchingService.IsCandidate(a, b)` (В4–В8) і `CalculateCompatibility(a, b)` (ДВ-02) — чисті методи без БД.

## Ключові бізнес-правила
- Відбір В1–В8: не сам, роль User і не заблокований, анкета активна, те саме місто, бюджети перетинаються включно, стать узгоджена в обидва боки, куріння (Smoker × NonSmokerAgainst — ні), тварини (HasPet × NoPetAgainst — ні).
- Сумісність: 4 критерії (сон, чистота, гості, шум); |різниця| 0 → 25, 1 → 10, 2 → 0; сума 0–100.
- Сортування Е-05: % ↓, `UpdatedAt` ↓, `Profile.Id` ↑; показуємо перші 50.
- Запити: 1 запит між парою в будь-якому напрямку/статусі; ≤ 10 надісланих за ковзні 24 год; переходи лише Pending → Accepted/Declined одержувачем.
- **ФК-15:** Telegram/телефон іншого користувача потрапляють у HTML лише при запиті `Accepted`. Не ховати CSS/JS — просто не передавати у ViewModel.
- Заблоковані (`IsBlocked`) не входять, не є кандидатами, їхні запити не показуються; `UpdateSecurityStampAsync` + `ValidationInterval = 1 хв`.
- Identity: lockout 5 спроб / 5 хв; cookie 8 год зі sliding expiration.

## Дані та формати
- Enum зберігаються як int від 1, з `[Display(Name = "...")]` українською.
- Дати в БД — UTC (`timestamptz`); в UI — `Europe/Kyiv`, `ДД.ММ.РРРР ГГ:ХХ` (на Е-08 — `ДД.ММ.РРРР`).
- Бюджет: `від 4 500 до 7 000 грн/міс`.
- Бейдж сумісності: 75–100 «Висока» `bg-success`; 40–74 «Середня» `bg-warning`; 0–39 «Низька» `bg-secondary`.
- Статус запиту: «Очікує» `bg-secondary`, «Прийнято» `bg-success`, «Відхилено» `bg-danger`.
- Тексти помилок — дослівно з табл. 4.5 і 4.6 ТЗ. Пробіли обрізаються, порожні необов'язкові поля → `NULL`.

## Початкові дані (ДВ-08)
- Ролі `User`, `Admin`; міста Київ, Львів, Харків, Одеса, Дніпро; адмін з `Admin__Email`/`Admin__Password`.
- Демо: `DemoData__Enabled`, `DemoData__ProfilesCount` (60), `new Random(42)`, `demoN@susidytut.local` / `Demo12345`.

## Стиль коду
- Ідентифікатори англійською, тексти UI українською.
- XML-коментарі `///` (коротко, українською) для публічних класів/методів у `Models/` і `Services/`.
- У `MatchingService` кожна умова/бал — коментар з кодом вимоги (`// В5: перетин бюджетів`).
- Інші коментарі — лише для неочевидної логіки, 3–6 слів.

## Git
- Гілки `feature/<назва>`, злиття в `main`.
- Коміти та заголовки PR — один короткий рядок англійською (`Add matching service`).
- `bin/`, `obj/` не комітимо.

## Команди
- Збірка: `dotnet build`
- Міграція: `dotnet ef migrations add <Name> -p src/SusidyTut.Web -o Data/Migrations`
- Запуск: `docker compose up -d --build` → http://localhost:8080
