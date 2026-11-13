using Microsoft.AspNetCore.Identity;

namespace SusidyTut.Web.Models;

/// <summary>Обліковий запис користувача або адміністратора.</summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>Дата реєстрації (UTC).</summary>
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    /// <summary>Ознака блокування адміністратором.</summary>
    public bool IsBlocked { get; set; }

    /// <summary>Анкета користувача (1:1).</summary>
    public Profile? Profile { get; set; }
}
