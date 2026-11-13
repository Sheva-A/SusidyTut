using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SusidyTut.Web.Data;
using SusidyTut.Web.Infrastructure;
using SusidyTut.Web.Models;
using SusidyTut.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.User.AllowedUserNameCharacters = string.Empty;
        // Складність пароля перевіряє regex у формі
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredUniqueChars = 1;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager<AppSignInManager>();

// ФА-03: сесія заблокованого діє не довше 1 хв
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
    options.ValidationInterval = TimeSpan.FromMinutes(1));

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Error/403";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services
    .AddRazorPages()
    .AddMvcOptions(options =>
    {
        options.ModelBinderProviders.Insert(0, new TrimStringModelBinderProvider());
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

        var messages = options.ModelBindingMessageProvider;
        messages.SetValueMustNotBeNullAccessor(_ => "Заповніть це поле");
        messages.SetMissingBindRequiredValueAccessor(_ => "Заповніть це поле");
        messages.SetMissingKeyOrValueAccessor(() => "Заповніть це поле");
        messages.SetAttemptedValueIsInvalidAccessor((_, _) => "Некоректне значення");
        messages.SetUnknownValueIsInvalidAccessor(_ => "Некоректне значення");
        messages.SetValueIsInvalidAccessor(_ => "Некоректне значення");
        messages.SetValueMustBeANumberAccessor(_ => "Введіть ціле число");
        messages.SetNonPropertyAttemptedValueIsInvalidAccessor(_ => "Некоректне значення");
        messages.SetNonPropertyUnknownValueIsInvalidAccessor(() => "Некоректне значення");
        messages.SetNonPropertyValueMustBeANumberAccessor(() => "Введіть ціле число");
    });

builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IMatchingService, MatchingService>();
builder.Services.AddScoped<IContactRequestService, ContactRequestService>();
builder.Services.AddScoped<IAdminService, AdminService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await DbInitializer.InitializeAsync(scope.ServiceProvider);
}

// Е-09 для 500, 404, 403
app.UseExceptionHandler("/Error/500");
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
