using System.ComponentModel.DataAnnotations;

namespace SusidyTut.Web.Models.Enums;

/// <summary>Ставлення до шуму (коди 1–3).</summary>
public enum NoiseTolerance
{
    [Display(Name = "Потрібна тиша вдома")]
    Quiet = 1,

    [Display(Name = "Помірний шум допустимий до 22:00")]
    Moderate = 2,

    [Display(Name = "Шум не заважає в будь-який час")]
    Any = 3
}
