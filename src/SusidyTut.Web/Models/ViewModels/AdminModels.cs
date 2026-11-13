namespace SusidyTut.Web.Models.ViewModels;

/// <summary>Статистика для Е-08 (ФА-05).</summary>
public class AdminStats
{
    /// <summary>Усього користувачів.</summary>
    public int UsersTotal { get; init; }

    /// <summary>Заблоковані користувачі.</summary>
    public int UsersBlocked { get; init; }

    /// <summary>Активні анкети.</summary>
    public int ActiveProfiles { get; init; }

    /// <summary>Запити «Очікує».</summary>
    public int RequestsPending { get; init; }

    /// <summary>Запити «Прийнято».</summary>
    public int RequestsAccepted { get; init; }

    /// <summary>Запити «Відхилено».</summary>
    public int RequestsDeclined { get; init; }
}

/// <summary>Рядок таблиці користувачів на Е-08.</summary>
public class AdminUserRow
{
    /// <summary>Id користувача.</summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>Email.</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>Ім'я з анкети або null.</summary>
    public string? Name { get; init; }

    /// <summary>Дата реєстрації (UTC).</summary>
    public DateTime RegisteredAt { get; init; }

    /// <summary>Ознака блокування.</summary>
    public bool IsBlocked { get; init; }
}

/// <summary>Сторінка списку з пагінацією.</summary>
/// <typeparam name="T">Тип рядка.</typeparam>
public class PagedList<T>
{
    /// <summary>Рядки поточної сторінки.</summary>
    public IReadOnlyList<T> Items { get; init; } = [];

    /// <summary>Номер сторінки (з 1).</summary>
    public int Page { get; init; } = 1;

    /// <summary>Кількість сторінок.</summary>
    public int TotalPages { get; init; } = 1;

    /// <summary>Чи є попередня сторінка.</summary>
    public bool HasPrevious => Page > 1;

    /// <summary>Чи є наступна сторінка.</summary>
    public bool HasNext => Page < TotalPages;
}
