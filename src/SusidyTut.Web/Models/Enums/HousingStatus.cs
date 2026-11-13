using System.ComponentModel.DataAnnotations;

namespace SusidyTut.Web.Models.Enums;

/// <summary>Житловий статус.</summary>
public enum HousingStatus
{
    [Display(Name = "Маю житло, шукаю сусіда")]
    HasHousing = 1,

    [Display(Name = "Шукаю житло разом із сусідом")]
    LookingTogether = 2
}
