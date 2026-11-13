using System.ComponentModel.DataAnnotations;

namespace SusidyTut.Web.Models.Enums;

/// <summary>Частота гостей (коди 1–3).</summary>
public enum GuestsFrequency
{
    [Display(Name = "Гості рідше ніж раз на місяць")]
    Rare = 1,

    [Display(Name = "Гості 1–3 рази на місяць")]
    Sometimes = 2,

    [Display(Name = "Гості 4 і більше разів на місяць")]
    Often = 3
}
