using System.ComponentModel.DataAnnotations;

namespace SusidyTut.Web.Models;

/// <summary>Місто з довідника.</summary>
public class City
{
    /// <summary>Первинний ключ.</summary>
    public int Id { get; set; }

    /// <summary>Назва міста.</summary>
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
