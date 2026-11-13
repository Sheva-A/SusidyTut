using System.ComponentModel.DataAnnotations;
using SusidyTut.Web.Models.Enums;

namespace SusidyTut.Web.Models;

/// <summary>Запит на знайомство між двома користувачами.</summary>
public class ContactRequest
{
    /// <summary>Первинний ключ.</summary>
    public int Id { get; set; }

    /// <summary>Відправник.</summary>
    public string SenderId { get; set; } = string.Empty;

    /// <summary>Обліковий запис відправника.</summary>
    public ApplicationUser? Sender { get; set; }

    /// <summary>Одержувач.</summary>
    public string ReceiverId { get; set; } = string.Empty;

    /// <summary>Обліковий запис одержувача.</summary>
    public ApplicationUser? Receiver { get; set; }

    /// <summary>Повідомлення відправника.</summary>
    [MaxLength(300)]
    public string? Message { get; set; }

    /// <summary>Статус запиту.</summary>
    public RequestStatus Status { get; set; } = RequestStatus.Pending;

    /// <summary>Дата надсилання (UTC).</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Дата відповіді (UTC).</summary>
    public DateTime? RespondedAt { get; set; }
}
