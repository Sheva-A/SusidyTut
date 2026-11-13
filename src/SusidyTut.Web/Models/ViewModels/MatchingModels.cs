using SusidyTut.Web.Models.Enums;

namespace SusidyTut.Web.Models.ViewModels;

/// <summary>Стан запиту між поточним користувачем і кандидатом (табл. 4.2).</summary>
public enum RequestState
{
    /// <summary>Запиту немає.</summary>
    None = 1,

    /// <summary>Я надіслав, статус «Очікує».</summary>
    SentPending = 2,

    /// <summary>Я отримав, статус «Очікує».</summary>
    ReceivedPending = 3,

    /// <summary>Статус «Прийнято».</summary>
    Accepted = 4,

    /// <summary>Статус «Відхилено».</summary>
    Declined = 5
}

/// <summary>Допоміжні методи для RequestState.</summary>
public static class RequestStates
{
    /// <summary>Визначає стан запиту для поточного користувача.</summary>
    /// <param name="request">Запит між парою або null.</param>
    /// <param name="currentUserId">Id поточного користувача.</param>
    /// <returns>Стан запиту.</returns>
    public static RequestState From(ContactRequest? request, string currentUserId) => request switch
    {
        null => RequestState.None,
        { Status: RequestStatus.Accepted } => RequestState.Accepted,
        { Status: RequestStatus.Declined } => RequestState.Declined,
        _ when request.SenderId == currentUserId => RequestState.SentPending,
        _ => RequestState.ReceivedPending
    };

    /// <summary>Текст стану для картки Е-05.</summary>
    /// <param name="state">Стан запиту.</param>
    /// <returns>Текст або null, якщо запиту немає.</returns>
    public static string? CardText(RequestState state) => state switch
    {
        RequestState.SentPending => "Запит надіслано",
        RequestState.ReceivedPending => "Отримано запит",
        RequestState.Accepted => "Знайомство підтверджено",
        RequestState.Declined => "Запит відхилено",
        _ => null
    };
}

/// <summary>Бали за одним критерієм сумісності.</summary>
/// <param name="Criterion">Назва критерію.</param>
/// <param name="MyValue">Значення поточного користувача.</param>
/// <param name="CandidateValue">Значення кандидата.</param>
/// <param name="Points">Нараховані бали.</param>
public record CriterionScore(string Criterion, string MyValue, string CandidateValue, int Points);

/// <summary>Картка кандидата на Е-05.</summary>
public class CandidateCard
{
    /// <summary>Id анкети кандидата.</summary>
    public int ProfileId { get; init; }

    /// <summary>Ім'я.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Вік.</summary>
    public int Age { get; init; }

    /// <summary>Навчальний заклад.</summary>
    public string? University { get; init; }

    /// <summary>Бюджет від.</summary>
    public int BudgetMin { get; init; }

    /// <summary>Бюджет до.</summary>
    public int BudgetMax { get; init; }

    /// <summary>Житловий статус.</summary>
    public HousingStatus Housing { get; init; }

    /// <summary>Відсоток сумісності.</summary>
    public int Compatibility { get; init; }

    /// <summary>Стан запиту.</summary>
    public RequestState RequestState { get; init; }
}

/// <summary>Результат підбору для Е-05.</summary>
public class SearchResult
{
    /// <summary>Загальна кількість кандидатів.</summary>
    public int TotalCount { get; init; }

    /// <summary>Перші 50 карток.</summary>
    public IReadOnlyList<CandidateCard> Cards { get; init; } = [];
}
