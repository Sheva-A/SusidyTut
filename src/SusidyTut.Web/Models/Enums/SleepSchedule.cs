using System.ComponentModel.DataAnnotations;

namespace SusidyTut.Web.Models.Enums;

/// <summary>Режим сну (коди 1–3).</summary>
public enum SleepSchedule
{
    [Display(Name = "Лягаю спати до 23:00")]
    Early = 1,

    [Display(Name = "Лягаю спати з 23:00 до 01:00")]
    Medium = 2,

    [Display(Name = "Лягаю спати після 01:00")]
    Late = 3
}
