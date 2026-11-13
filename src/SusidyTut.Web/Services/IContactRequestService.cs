using SusidyTut.Web.Models;
using SusidyTut.Web.Models.ViewModels;

namespace SusidyTut.Web.Services;

/// <summary>Запити на знайомство та контакти (ДВ-04, ФК-10–ФК-15).</summary>
public interface IContactRequestService
{
    /// <summary>Дані сторінки Е-06 з урахуванням доступу.</summary>
    /// <param name="me">Анкета поточного користувача.</param>
    /// <param name="profileId">Id анкети кандидата.</param>
    /// <returns>Дані або null, якщо доступу немає.</returns>
    Task<CandidateDetails?> GetCandidateDetailsAsync(Profile me, int profileId);

    /// <summary>Надсилає запит після перевірки умов ФК-10.</summary>
    /// <param name="sender">Анкета відправника.</param>
    /// <param name="receiverId">Id одержувача.</param>
    /// <param name="message">Повідомлення (до 300 символів).</param>
    /// <returns>Результат надсилання.</returns>
    Task<SendResult> SendAsync(Profile sender, string receiverId, string? message);

    /// <summary>Приймає або відхиляє запит (лише одержувач).</summary>
    /// <param name="requestId">Id запиту.</param>
    /// <param name="userId">Id поточного користувача.</param>
    /// <param name="accept">true — прийняти, false — відхилити.</param>
    /// <returns>Результат дії.</returns>
    Task<RespondResult> RespondAsync(int requestId, string userId, bool accept);

    /// <summary>Вхідні, надіслані запити та контакти для Е-07.</summary>
    /// <param name="me">Анкета поточного користувача.</param>
    /// <returns>Дані сторінки.</returns>
    Task<RequestsOverview> GetOverviewAsync(Profile me);

    /// <summary>Кількість вхідних запитів «Очікує» без заблокованих.</summary>
    /// <param name="userId">Id користувача.</param>
    /// <returns>Кількість для лічильника в меню.</returns>
    Task<int> CountPendingIncomingAsync(string userId);
}
