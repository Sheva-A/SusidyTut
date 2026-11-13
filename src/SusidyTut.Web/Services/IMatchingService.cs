using SusidyTut.Web.Models;
using SusidyTut.Web.Models.ViewModels;

namespace SusidyTut.Web.Services;

/// <summary>Підбір кандидатів і сумісність (ДВ-01–ДВ-03).</summary>
public interface IMatchingService
{
    /// <summary>Умови В4–В8 без звернення до БД.</summary>
    /// <param name="a">Анкета поточного користувача.</param>
    /// <param name="b">Анкета іншого користувача.</param>
    /// <returns>true, якщо всі умови виконано.</returns>
    bool IsCandidate(Profile a, Profile b);

    /// <summary>Відсоток сумісності 0–100 (ДВ-02).</summary>
    /// <param name="a">Анкета поточного користувача.</param>
    /// <param name="b">Анкета іншого користувача.</param>
    /// <returns>Сума балів за 4 критеріями.</returns>
    int CalculateCompatibility(Profile a, Profile b);

    /// <summary>Бали за кожним критерієм для таблиці Е-06.</summary>
    /// <param name="a">Анкета поточного користувача.</param>
    /// <param name="b">Анкета кандидата.</param>
    /// <returns>Чотири рядки: сон, чистота, гості, шум.</returns>
    IReadOnlyList<CriterionScore> GetCriteria(Profile a, Profile b);

    /// <summary>Список кандидатів для Е-05 (до 50).</summary>
    /// <param name="me">Анкета поточного користувача.</param>
    /// <returns>Кількість і картки кандидатів.</returns>
    Task<SearchResult> FindCandidatesAsync(Profile me);

    /// <summary>Повна перевірка В1–В8 для пари.</summary>
    /// <param name="me">Анкета поточного користувача.</param>
    /// <param name="otherUserId">Id іншого користувача.</param>
    /// <returns>true, якщо інший — кандидат.</returns>
    Task<bool> IsCandidateAsync(Profile me, string otherUserId);

    /// <summary>Анкета, яку можна відкрити на Е-06 (ФК-08).</summary>
    /// <param name="me">Анкета поточного користувача.</param>
    /// <param name="profileId">Id анкети.</param>
    /// <returns>Анкета або null, якщо доступу немає.</returns>
    Task<Profile?> GetViewableProfileAsync(Profile me, int profileId);
}
