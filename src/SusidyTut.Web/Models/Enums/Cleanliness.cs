using System.ComponentModel.DataAnnotations;

namespace SusidyTut.Web.Models.Enums;

/// <summary>Чистота (коди 1–3).</summary>
public enum Cleanliness
{
    [Display(Name = "Прибираю рідше ніж раз на тиждень")]
    Low = 1,

    [Display(Name = "Прибираю раз на тиждень")]
    Medium = 2,

    [Display(Name = "Підтримую порядок щодня")]
    High = 3
}
