using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SusidyTut.Web.Models;
using SusidyTut.Web.Models.Enums;

namespace SusidyTut.Web.Data;

/// <summary>Міграції та початкові дані (ДВ-08).</summary>
public static class DbInitializer
{
    /// <summary>Назва ролі користувача.</summary>
    public const string UserRole = "User";

    /// <summary>Назва ролі адміністратора.</summary>
    public const string AdminRole = "Admin";

    /// <summary>Пароль демо-акаунтів.</summary>
    public const string DemoPassword = "Demo12345";

    private static readonly string[] CityNames = ["Київ", "Львів", "Харків", "Одеса", "Дніпро"];

    private static readonly string[] FemaleNames =
    [
        "Анна", "Олена", "Марія", "Ірина", "Катерина", "Софія", "Дарина", "Вікторія", "Юлія", "Наталія",
        "Оксана", "Христина", "Тетяна", "Валерія", "Анастасія", "Аліна", "Богдана", "Соломія", "Злата", "Ярослава"
    ];

    private static readonly string[] MaleNames =
    [
        "Андрій", "Олександр", "Дмитро", "Максим", "Іван", "Богдан", "Тарас", "Микола", "Остап", "Юрій",
        "Василь", "Сергій", "Артем", "Назар", "Денис", "Владислав", "Ярослав", "Роман", "Степан", "Олег"
    ];

    private static readonly string[] Universities =
        ["ДУІКТ", "КНУ імені Тараса Шевченка", "КПІ ім. Ігоря Сікорського", "НаУКМА", "КНЕУ"];

    /// <summary>Застосовує міграції та створює початкові дані.</summary>
    /// <param name="services">Провайдер сервісів (scope).</param>
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var config = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DbInitializer));

        await MigrateWithRetryAsync(db, logger);

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { UserRole, AdminRole })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        foreach (var name in CityNames)
        {
            if (!await db.Cities.AnyAsync(c => c.Name == name))
                db.Cities.Add(new City { Name = name });
        }
        await db.SaveChangesAsync();

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        await EnsureAdminAsync(userManager, config, logger);

        if (config.GetValue("DemoData:Enabled", false) && !await db.Profiles.AnyAsync())
        {
            var count = config.GetValue("DemoData:ProfilesCount", 60);
            await CreateDemoDataAsync(db, userManager, count);
            logger.LogInformation("Створено демо-анкет: {Count}", count);
        }
    }

    private static async Task MigrateWithRetryAsync(ApplicationDbContext db, ILogger logger)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                await db.Database.MigrateAsync();
                return;
            }
            catch (Exception ex) when (attempt < 10)
            {
                logger.LogWarning(ex, "БД недоступна, спроба {Attempt}", attempt);
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
        }
    }

    private static async Task EnsureAdminAsync(UserManager<ApplicationUser> userManager, IConfiguration config, ILogger logger)
    {
        var email = config["Admin:Email"] ?? "admin@susidytut.local";
        var password = config["Admin:Password"] ?? "Admin12345";

        if (await userManager.FindByEmailAsync(email) is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            RegisteredAt = DateTime.UtcNow
        };
        var result = await userManager.CreateAsync(admin, password);
        if (!result.Succeeded)
        {
            logger.LogError("Не вдалося створити адміністратора: {Errors}",
                string.Join("; ", result.Errors.Select(e => e.Description)));
            return;
        }
        await userManager.AddToRoleAsync(admin, AdminRole);
    }

    private static async Task CreateDemoDataAsync(ApplicationDbContext db, UserManager<ApplicationUser> userManager, int count)
    {
        var random = new Random(42);
        var kyivId = await db.Cities.Where(c => c.Name == "Київ").Select(c => c.Id).SingleAsync();
        var lvivId = await db.Cities.Where(c => c.Name == "Львів").Select(c => c.Id).SingleAsync();
        var userRoleId = await db.Roles.Where(r => r.Name == UserRole).Select(r => r.Id).SingleAsync();

        // Один хеш на всіх: швидше для 500 анкет
        var passwordHash = userManager.PasswordHasher.HashPassword(new ApplicationUser(), DemoPassword);
        var now = DateTime.UtcNow;

        for (var i = 1; i <= count; i++)
        {
            var email = $"demo{i}@susidytut.local";
            if (await userManager.FindByEmailAsync(email) is not null)
                continue;

            var gender = (Gender)random.Next(1, 3);
            var names = gender == Gender.Male ? MaleNames : FemaleNames;
            var budgetMin = 3000 + 500 * random.Next(0, 9);
            var created = now.AddMinutes(-i);

            var user = new ApplicationUser
            {
                UserName = email,
                NormalizedUserName = userManager.NormalizeName(email),
                Email = email,
                NormalizedEmail = userManager.NormalizeEmail(email),
                EmailConfirmed = true,
                PasswordHash = passwordHash,
                SecurityStamp = Guid.NewGuid().ToString("N"),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                LockoutEnabled = true,
                RegisteredAt = created
            };

            var profile = new Profile
            {
                User = user,
                Name = names[random.Next(names.Length)],
                Age = random.Next(17, 26),
                Gender = gender,
                PreferredGender = (PreferredGender)random.Next(1, 4),
                CityId = i % 5 == 0 ? lvivId : kyivId,
                University = Universities[random.Next(Universities.Length)],
                Housing = (HousingStatus)random.Next(1, 3),
                BudgetMin = budgetMin,
                BudgetMax = budgetMin + 500 * random.Next(2, 9),
                SleepSchedule = (SleepSchedule)random.Next(1, 4),
                Cleanliness = (Cleanliness)random.Next(1, 4),
                Guests = (GuestsFrequency)random.Next(1, 4),
                Noise = (NoiseTolerance)random.Next(1, 4),
                Smoking = (SmokingAttitude)random.Next(1, 4),
                Pets = (PetsAttitude)random.Next(1, 4),
                Telegram = $"@demo_user_{i}",
                IsActive = true,
                CreatedAt = created,
                UpdatedAt = created
            };

            db.Users.Add(user);
            db.Profiles.Add(profile);
            db.UserRoles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = userRoleId });

            if (i % 100 == 0)
                await db.SaveChangesAsync();
        }

        await db.SaveChangesAsync();
    }
}
