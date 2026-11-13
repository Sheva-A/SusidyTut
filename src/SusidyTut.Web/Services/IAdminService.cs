using SusidyTut.Web.Models.ViewModels;

namespace SusidyTut.Web.Services;

/// <summary>Адміністрування користувачів (ФА-02–ФА-05).</summary>
public interface IAdminService
{
    /// <summary>Шість чисел статистики.</summary>
    /// <returns>Статистика на поточний момент.</returns>
    Task<AdminStats> GetStatsAsync();

    /// <summary>Користувачі з роллю User, по 25 на сторінку.</summary>
    /// <param name="emailFilter">Частина email (без регістру) або null.</param>
    /// <param name="page">Номер сторінки з 1.</param>
    /// <returns>Сторінка списку.</returns>
    Task<PagedList<AdminUserRow>> GetUsersAsync(string? emailFilter, int page);

    /// <summary>Блокує або розблоковує користувача.</summary>
    /// <param name="userId">Id користувача.</param>
    /// <param name="blocked">true — заблокувати.</param>
    /// <returns>false, якщо користувача з роллю User не знайдено.</returns>
    Task<bool> SetBlockedAsync(string userId, bool blocked);
}
