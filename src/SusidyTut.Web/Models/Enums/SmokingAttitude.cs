using System.ComponentModel.DataAnnotations;

namespace SusidyTut.Web.Models.Enums;

/// <summary>Ставлення до куріння.</summary>
public enum SmokingAttitude
{
    [Display(Name = "Курю")]
    Smoker = 1,

    [Display(Name = "Не курю, але не проти куріння")]
    NonSmokerTolerant = 2,

    [Display(Name = "Не курю і проти куріння")]
    NonSmokerAgainst = 3
}
