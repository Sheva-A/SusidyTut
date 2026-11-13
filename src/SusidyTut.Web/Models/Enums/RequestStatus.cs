using System.ComponentModel.DataAnnotations;

namespace SusidyTut.Web.Models.Enums;

/// <summary>Статус запиту на знайомство.</summary>
public enum RequestStatus
{
    [Display(Name = "Очікує")]
    Pending = 1,

    [Display(Name = "Прийнято")]
    Accepted = 2,

    [Display(Name = "Відхилено")]
    Declined = 3
}
