using System.ComponentModel.DataAnnotations;
using SusidyTut.Web.Models.Enums;

namespace SusidyTut.Web.Models;

/// <summary>Анкета користувача (табл. 4.4, 4.6).</summary>
public class Profile
{
    /// <summary>Первинний ключ.</summary>
    public int Id { get; set; }

    /// <summary>Власник анкети.</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>Обліковий запис власника.</summary>
    public ApplicationUser? User { get; set; }

    /// <summary>Ім'я.</summary>
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Вік.</summary>
    public int Age { get; set; }

    /// <summary>Стать.</summary>
    public Gender Gender { get; set; }

    /// <summary>Бажана стать сусіда.</summary>
    public PreferredGender PreferredGender { get; set; }

    /// <summary>Місто.</summary>
    public int CityId { get; set; }

    /// <summary>Довідник міста.</summary>
    public City? City { get; set; }

    /// <summary>Навчальний заклад.</summary>
    [MaxLength(100)]
    public string? University { get; set; }

    /// <summary>Житловий статус.</summary>
    public HousingStatus Housing { get; set; }

    /// <summary>Бюджет від, грн/міс.</summary>
    public int BudgetMin { get; set; }

    /// <summary>Бюджет до, грн/міс.</summary>
    public int BudgetMax { get; set; }

    /// <summary>Режим сну.</summary>
    public SleepSchedule SleepSchedule { get; set; }

    /// <summary>Чистота.</summary>
    public Cleanliness Cleanliness { get; set; }

    /// <summary>Гості.</summary>
    public GuestsFrequency Guests { get; set; }

    /// <summary>Шум.</summary>
    public NoiseTolerance Noise { get; set; }

    /// <summary>Куріння.</summary>
    public SmokingAttitude Smoking { get; set; }

    /// <summary>Домашні тварини.</summary>
    public PetsAttitude Pets { get; set; }

    /// <summary>Про себе.</summary>
    [MaxLength(500)]
    public string? About { get; set; }

    /// <summary>Telegram-нікнейм з «@».</summary>
    [MaxLength(33)]
    public string? Telegram { get; set; }

    /// <summary>Телефон у форматі +380XXXXXXXXX.</summary>
    [MaxLength(13)]
    public string? Phone { get; set; }

    /// <summary>Чи показується анкета іншим.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Дата створення (UTC).</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Дата останнього оновлення (UTC).</summary>
    public DateTime UpdatedAt { get; set; }
}
