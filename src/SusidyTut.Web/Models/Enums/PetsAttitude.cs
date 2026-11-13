using System.ComponentModel.DataAnnotations;

namespace SusidyTut.Web.Models.Enums;

/// <summary>Ставлення до домашніх тварин.</summary>
public enum PetsAttitude
{
    [Display(Name = "Маю тварину")]
    HasPet = 1,

    [Display(Name = "Не маю, але не проти тварин")]
    NoPetTolerant = 2,

    [Display(Name = "Не маю і проти тварин")]
    NoPetAgainst = 3
}
