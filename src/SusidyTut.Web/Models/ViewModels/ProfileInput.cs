using System.ComponentModel.DataAnnotations;
using SusidyTut.Web.Models.Enums;

namespace SusidyTut.Web.Models.ViewModels;

/// <summary>Поля форми анкети Е-04 (табл. 4.6).</summary>
public class ProfileInput
{
    /// <summary>Поле 1. Ім'я.</summary>
    [Display(Name = "Ім’я")]
    [Required(ErrorMessage = "Заповніть це поле")]
    [RegularExpression(@"^[A-Za-zА-Яа-яІіЇїЄєҐґ'’ -]{2,50}$", ErrorMessage = "Ім’я має містити від 2 до 50 літер")]
    public string? Name { get; set; }

    /// <summary>Поле 2. Вік.</summary>
    [Display(Name = "Вік")]
    [Required(ErrorMessage = "Заповніть це поле")]
    [Range(16, 60, ErrorMessage = "Вкажіть вік від 16 до 60 років")]
    public int? Age { get; set; }

    /// <summary>Поле 3. Стать.</summary>
    [Display(Name = "Стать")]
    [Required(ErrorMessage = "Оберіть варіант")]
    public Gender? Gender { get; set; }

    /// <summary>Поле 4. Бажана стать сусіда.</summary>
    [Display(Name = "Бажана стать сусіда")]
    [Required(ErrorMessage = "Оберіть варіант")]
    public PreferredGender? PreferredGender { get; set; }

    /// <summary>Поле 5. Місто.</summary>
    [Display(Name = "Місто")]
    [Required(ErrorMessage = "Оберіть варіант")]
    public int? CityId { get; set; }

    /// <summary>Поле 6. Навчальний заклад.</summary>
    [Display(Name = "Навчальний заклад")]
    [StringLength(100, ErrorMessage = "Не більше 100 символів")]
    public string? University { get; set; }

    /// <summary>Поле 7. Житло.</summary>
    [Display(Name = "Житло")]
    [Required(ErrorMessage = "Оберіть варіант")]
    public HousingStatus? Housing { get; set; }

    /// <summary>Поле 8. Бюджет від.</summary>
    [Display(Name = "Бюджет від, грн/міс")]
    [Required(ErrorMessage = "Заповніть це поле")]
    [Range(1000, 50000, ErrorMessage = "Вкажіть суму від 1000 до 50000 грн")]
    public int? BudgetMin { get; set; }

    /// <summary>Поле 9. Бюджет до.</summary>
    [Display(Name = "Бюджет до, грн/міс")]
    [Required(ErrorMessage = "Заповніть це поле")]
    [Range(1000, 50000, ErrorMessage = "Вкажіть суму від 1000 до 50000 грн")]
    public int? BudgetMax { get; set; }

    /// <summary>Поле 10. Режим сну.</summary>
    [Display(Name = "Режим сну")]
    [Required(ErrorMessage = "Оберіть варіант")]
    public SleepSchedule? SleepSchedule { get; set; }

    /// <summary>Поле 11. Чистота.</summary>
    [Display(Name = "Чистота")]
    [Required(ErrorMessage = "Оберіть варіант")]
    public Cleanliness? Cleanliness { get; set; }

    /// <summary>Поле 12. Гості.</summary>
    [Display(Name = "Гості")]
    [Required(ErrorMessage = "Оберіть варіант")]
    public GuestsFrequency? Guests { get; set; }

    /// <summary>Поле 13. Шум.</summary>
    [Display(Name = "Шум")]
    [Required(ErrorMessage = "Оберіть варіант")]
    public NoiseTolerance? Noise { get; set; }

    /// <summary>Поле 14. Куріння.</summary>
    [Display(Name = "Куріння")]
    [Required(ErrorMessage = "Оберіть варіант")]
    public SmokingAttitude? Smoking { get; set; }

    /// <summary>Поле 15. Домашні тварини.</summary>
    [Display(Name = "Домашні тварини")]
    [Required(ErrorMessage = "Оберіть варіант")]
    public PetsAttitude? Pets { get; set; }

    /// <summary>Поле 16. Про себе.</summary>
    [Display(Name = "Про себе")]
    [StringLength(500, ErrorMessage = "Не більше 500 символів")]
    public string? About { get; set; }

    /// <summary>Поле 17. Telegram.</summary>
    [Display(Name = "Telegram")]
    [RegularExpression(@"^@[A-Za-z0-9_]{5,32}$", ErrorMessage = "Вкажіть Telegram у форматі @нікнейм")]
    public string? Telegram { get; set; }

    /// <summary>Поле 18. Телефон.</summary>
    [Display(Name = "Телефон")]
    [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Вкажіть телефон у форматі +380XXXXXXXXX")]
    public string? Phone { get; set; }

    /// <summary>Поле 19. Анкета активна.</summary>
    [Display(Name = "Анкета активна")]
    public bool IsActive { get; set; } = true;
}

/// <summary>Помилка бізнес-перевірки для конкретного поля.</summary>
/// <param name="Field">Назва поля ProfileInput.</param>
/// <param name="Message">Текст помилки.</param>
public record FieldError(string Field, string Message);
