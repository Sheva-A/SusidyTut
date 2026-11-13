using System.ComponentModel.DataAnnotations;

namespace SusidyTut.Web.Models.Enums;

/// <summary>Стать користувача.</summary>
public enum Gender
{
    [Display(Name = "Чоловіча")]
    Male = 1,

    [Display(Name = "Жіноча")]
    Female = 2
}
