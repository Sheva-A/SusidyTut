using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using SusidyTut.Web.Data;
using SusidyTut.Web.Models.Enums;

namespace SusidyTut.Web.Infrastructure;

/// <summary>Форматування даних для сторінок (п. 4.5.2).</summary>
public static class Format
{
    private static readonly ConcurrentDictionary<Enum, string> EnumNames = new();
    private static readonly TimeZoneInfo KyivZone = FindKyivZone();

    /// <summary>Текст [Display(Name)] для значення enum.</summary>
    public static string EnumName(Enum value) => EnumNames.GetOrAdd(value, v =>
        v.GetType().GetField(v.ToString())?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? v.ToString());

    /// <summary>Число з пробілом між тисячами: 4 500.</summary>
    public static string Money(int value) =>
        value.ToString("#,0", CultureInfo.InvariantCulture).Replace(',', ' ');

    /// <summary>Бюджет: «від 4 500 до 7 000 грн/міс».</summary>
    public static string Budget(int min, int max) => $"від {Money(min)} до {Money(max)} грн/міс";

    /// <summary>Дата й час за Києвом: ДД.ММ.РРРР ГГ:ХХ.</summary>
    public static string KyivDateTime(DateTime utc) =>
        ToKyiv(utc).ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);

    /// <summary>Дата за Києвом: ДД.ММ.РРРР.</summary>
    public static string KyivDate(DateTime utc) =>
        ToKyiv(utc).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);

    /// <summary>Мітка рівня сумісності.</summary>
    public static string CompatibilityLabel(int percent) => percent switch
    {
        >= 75 => "Висока",
        >= 40 => "Середня",
        _ => "Низька"
    };

    /// <summary>CSS-клас бейджа сумісності.</summary>
    public static string CompatibilityCss(int percent) => percent switch
    {
        >= 75 => "bg-success",
        >= 40 => "bg-warning text-dark",
        _ => "bg-secondary"
    };

    /// <summary>CSS-клас бейджа статусу запиту.</summary>
    public static string StatusCss(RequestStatus status) => status switch
    {
        RequestStatus.Accepted => "bg-success",
        RequestStatus.Declined => "bg-danger",
        _ => "bg-secondary"
    };

    /// <summary>Посилання на Telegram без «@».</summary>
    public static string TelegramUrl(string telegram) => "https://t.me/" + telegram.TrimStart('@');

    /// <summary>Стартова сторінка залежно від ролі.</summary>
    public static string HomePage(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
            return "/Index";
        return user.IsInRole(DbInitializer.AdminRole) ? "/Admin/Index" : "/Search/Index";
    }

    private static DateTime ToKyiv(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), KyivZone);

    private static TimeZoneInfo FindKyivZone()
    {
        foreach (var id in new[] { "Europe/Kyiv", "Europe/Kiev", "FLE Standard Time" })
        {
            if (TimeZoneInfo.TryFindSystemTimeZoneById(id, out var zone))
                return zone;
        }
        return TimeZoneInfo.Utc;
    }
}
