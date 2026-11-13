using System.ComponentModel.DataAnnotations;

namespace SusidyTut.Web.Models.ViewModels;

/// <summary>Поля форми реєстрації Е-02 (табл. 4.5).</summary>
public class RegisterInput
{
    /// <summary>Email.</summary>
    [Display(Name = "Email")]
    [Required(ErrorMessage = "Заповніть це поле")]
    [EmailAddress(ErrorMessage = "Введіть коректний email")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Введіть коректний email")]
    [StringLength(100, ErrorMessage = "Введіть коректний email")]
    public string? Email { get; set; }

    /// <summary>Пароль.</summary>
    [Display(Name = "Пароль")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Заповніть це поле")]
    [RegularExpression(@"^(?=.*[A-Za-zА-Яа-яІіЇїЄєҐґ])(?=.*\d).{8,64}$",
        ErrorMessage = "Пароль має містити від 8 до 64 символів, щонайменше одну літеру та одну цифру")]
    public string? Password { get; set; }

    /// <summary>Підтвердження пароля.</summary>
    [Display(Name = "Підтвердження пароля")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Заповніть це поле")]
    [Compare(nameof(Password), ErrorMessage = "Паролі не збігаються")]
    public string? ConfirmPassword { get; set; }
}

/// <summary>Поля форми входу Е-03.</summary>
public class LoginInput
{
    /// <summary>Email.</summary>
    [Display(Name = "Email")]
    [Required(ErrorMessage = "Заповніть це поле")]
    [EmailAddress(ErrorMessage = "Введіть коректний email")]
    [StringLength(100, ErrorMessage = "Введіть коректний email")]
    public string? Email { get; set; }

    /// <summary>Пароль.</summary>
    [Display(Name = "Пароль")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Заповніть це поле")]
    public string? Password { get; set; }
}
