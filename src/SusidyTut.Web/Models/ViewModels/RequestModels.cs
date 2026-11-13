using SusidyTut.Web.Models.Enums;

namespace SusidyTut.Web.Models.ViewModels;

/// <summary>Результат надсилання запиту (ФК-10).</summary>
public enum SendResult
{
    /// <summary>Запит створено.</summary>
    Sent = 1,

    /// <summary>Вичерпано ліміт 10 запитів за 24 год.</summary>
    LimitReached = 2,

    /// <summary>Між парою вже є запит.</summary>
    AlreadyExists = 3,

    /// <summary>Анкета відправника неактивна.</summary>
    ProfileInactive = 4,

    /// <summary>Одержувач не є кандидатом.</summary>
    NotCandidate = 5
}

/// <summary>Результат відповіді на запит (ФК-11, ФК-12).</summary>
public enum RespondResult
{
    /// <summary>Статус змінено.</summary>
    Success = 1,

    /// <summary>Запит не знайдено.</summary>
    NotFound = 2,

    /// <summary>Користувач не є одержувачем.</summary>
    Forbidden = 3,

    /// <summary>Запит уже не в статусі «Очікує».</summary>
    NotPending = 4
}

/// <summary>Рядок таблиці «Вхідні» / «Надіслані» на Е-07.</summary>
public class RequestRow
{
    /// <summary>Id запиту.</summary>
    public int RequestId { get; init; }

    /// <summary>Id анкети іншого учасника.</summary>
    public int OtherProfileId { get; init; }

    /// <summary>Ім'я іншого учасника.</summary>
    public string OtherName { get; init; } = string.Empty;

    /// <summary>Відсоток сумісності.</summary>
    public int Compatibility { get; init; }

    /// <summary>Повідомлення.</summary>
    public string? Message { get; init; }

    /// <summary>Дата надсилання (UTC).</summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>Статус.</summary>
    public RequestStatus Status { get; init; }
}

/// <summary>Рядок вкладки «Контакти» (лише прийняті запити).</summary>
public class ContactRow
{
    /// <summary>Id анкети.</summary>
    public int ProfileId { get; init; }

    /// <summary>Ім'я.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Telegram.</summary>
    public string? Telegram { get; init; }

    /// <summary>Телефон.</summary>
    public string? Phone { get; init; }
}

/// <summary>Дані сторінки Е-07.</summary>
public class RequestsOverview
{
    /// <summary>Вхідні запити.</summary>
    public IReadOnlyList<RequestRow> Incoming { get; init; } = [];

    /// <summary>Надіслані запити.</summary>
    public IReadOnlyList<RequestRow> Sent { get; init; } = [];

    /// <summary>Відкриті контакти.</summary>
    public IReadOnlyList<ContactRow> Contacts { get; init; } = [];
}

/// <summary>Дані сторінки Е-06 «Анкета кандидата».</summary>
public class CandidateDetails
{
    /// <summary>Id анкети.</summary>
    public int ProfileId { get; init; }

    /// <summary>Id користувача-кандидата.</summary>
    public string UserId { get; init; } = string.Empty;

    /// <summary>Ім'я.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Вік.</summary>
    public int Age { get; init; }

    /// <summary>Стать.</summary>
    public Gender Gender { get; init; }

    /// <summary>Бажана стать сусіда.</summary>
    public PreferredGender PreferredGender { get; init; }

    /// <summary>Місто.</summary>
    public string City { get; init; } = string.Empty;

    /// <summary>Навчальний заклад.</summary>
    public string? University { get; init; }

    /// <summary>Житло.</summary>
    public HousingStatus Housing { get; init; }

    /// <summary>Бюджет від.</summary>
    public int BudgetMin { get; init; }

    /// <summary>Бюджет до.</summary>
    public int BudgetMax { get; init; }

    /// <summary>Режим сну.</summary>
    public SleepSchedule SleepSchedule { get; init; }

    /// <summary>Чистота.</summary>
    public Cleanliness Cleanliness { get; init; }

    /// <summary>Гості.</summary>
    public GuestsFrequency Guests { get; init; }

    /// <summary>Шум.</summary>
    public NoiseTolerance Noise { get; init; }

    /// <summary>Куріння.</summary>
    public SmokingAttitude Smoking { get; init; }

    /// <summary>Тварини.</summary>
    public PetsAttitude Pets { get; init; }

    /// <summary>Про себе.</summary>
    public string? About { get; init; }

    /// <summary>Бали за критеріями.</summary>
    public IReadOnlyList<CriterionScore> Criteria { get; init; } = [];

    /// <summary>Підсумковий відсоток.</summary>
    public int Compatibility { get; init; }

    /// <summary>Стан запиту.</summary>
    public RequestState RequestState { get; init; }

    /// <summary>Id запиту, якщо є.</summary>
    public int? RequestId { get; init; }

    /// <summary>Повідомлення вхідного запиту.</summary>
    public string? IncomingMessage { get; init; }

    /// <summary>Telegram — лише при прийнятому запиті (ФК-15).</summary>
    public string? Telegram { get; init; }

    /// <summary>Телефон — лише при прийнятому запиті (ФК-15).</summary>
    public string? Phone { get; init; }
}
