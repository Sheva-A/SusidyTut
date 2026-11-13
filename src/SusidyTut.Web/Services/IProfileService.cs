using SusidyTut.Web.Models;
using SusidyTut.Web.Models.ViewModels;

namespace SusidyTut.Web.Services;

/// <summary>Робота з анкетами (ФК-04–ФК-06).</summary>
public interface IProfileService
{
    /// <summary>Анкета користувача з містом.</summary>
    /// <param name="userId">Id користувача.</param>
    /// <returns>Анкета або null.</returns>
    Task<Profile?> GetByUserIdAsync(string userId);

    /// <summary>Чи має користувач анкету.</summary>
    /// <param name="userId">Id користувача.</param>
    /// <returns>true, якщо анкета існує.</returns>
    Task<bool> HasProfileAsync(string userId);

    /// <summary>Дані анкети для форми.</summary>
    /// <param name="userId">Id користувача.</param>
    /// <returns>Заповнена форма або нова з типовими значеннями.</returns>
    Task<ProfileInput> GetInputAsync(string userId);

    /// <summary>Довідник міст за назвою.</summary>
    /// <returns>Список міст.</returns>
    Task<IReadOnlyList<City>> GetCitiesAsync();

    /// <summary>Бізнес-перевірка анкети (бюджет, контакти, місто).</summary>
    /// <param name="input">Дані форми.</param>
    /// <returns>Список помилок полів.</returns>
    Task<IReadOnlyList<FieldError>> ValidateAsync(ProfileInput input);

    /// <summary>Створює або оновлює анкету.</summary>
    /// <param name="userId">Id користувача.</param>
    /// <param name="input">Перевірені дані форми.</param>
    /// <returns>true, якщо анкету створено вперше.</returns>
    Task<bool> SaveAsync(string userId, ProfileInput input);
}
