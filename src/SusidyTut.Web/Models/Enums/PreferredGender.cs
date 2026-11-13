using System.ComponentModel.DataAnnotations;

namespace SusidyTut.Web.Models.Enums;

/// <summary>Бажана стать сусіда.</summary>
public enum PreferredGender
{
    [Display(Name = "Чоловіча")]
    Male = 1,

    [Display(Name = "Жіноча")]
    Female = 2,

    [Display(Name = "Не важливо")]
    Any = 3
}
