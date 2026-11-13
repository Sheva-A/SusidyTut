using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SusidyTut.Web.Data;
using SusidyTut.Web.Models;
using SusidyTut.Web.Models.ViewModels;
using SusidyTut.Web.Services;

namespace SusidyTut.Web.Pages.Search;

[Authorize(Roles = DbInitializer.UserRole)]
public class CandidateModel(IProfileService profiles, IContactRequestService requests) : UserPageModel
{
    public CandidateDetails Details { get; private set; } = new();

    public bool IsMyProfileActive { get; private set; }

    [BindProperty]
    [Display(Name = "Повідомлення (необов’язково)")]
    [StringLength(300, ErrorMessage = "Не більше 300 символів")]
    public string? Message { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var me = await MyProfileAsync();
        return await LoadAsync(me, id) ? Page() : NotFound();
    }

    public async Task<IActionResult> OnPostSendAsync(int id)
    {
        var me = await MyProfileAsync();
        if (!await LoadAsync(me, id))
            return NotFound();

        if (!ModelState.IsValid)
            return Page();

        switch (await requests.SendAsync(me, Details.UserId, Message))
        {
            case SendResult.Sent:
                TempData["Success"] = "Запит надіслано";
                break;
            case SendResult.LimitReached:
                TempData["Error"] = "Можна надіслати не більше 10 запитів на добу. Спробуйте пізніше";
                break;
            case SendResult.AlreadyExists:
                break;
            default:
                // Обхід умов ФК-10
                return StatusCode(StatusCodes.Status403Forbidden);
        }
        return RedirectToPage(new { id });
    }

    public Task<IActionResult> OnPostAcceptAsync(int id, int requestId) => RespondAsync(id, requestId, true);

    public Task<IActionResult> OnPostDeclineAsync(int id, int requestId) => RespondAsync(id, requestId, false);

    private async Task<IActionResult> RespondAsync(int id, int requestId, bool accept)
    {
        switch (await requests.RespondAsync(requestId, UserId, accept))
        {
            case RespondResult.Success:
                TempData["Success"] = accept ? "Запит прийнято" : "Запит відхилено";
                return RedirectToPage(new { id });
            case RespondResult.NotFound:
                return NotFound();
            default:
                return StatusCode(StatusCodes.Status403Forbidden);
        }
    }

    private async Task<Models.Profile> MyProfileAsync() => (await profiles.GetByUserIdAsync(UserId))!;

    private async Task<bool> LoadAsync(Models.Profile me, int id)
    {
        IsMyProfileActive = me.IsActive;
        var details = await requests.GetCandidateDetailsAsync(me, id);
        if (details is null)
            return false;
        Details = details;
        return true;
    }
}
