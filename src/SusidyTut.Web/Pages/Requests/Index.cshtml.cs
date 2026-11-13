using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SusidyTut.Web.Data;
using SusidyTut.Web.Models.ViewModels;
using SusidyTut.Web.Services;

namespace SusidyTut.Web.Pages.Requests;

[Authorize(Roles = DbInitializer.UserRole)]
public class IndexModel(IProfileService profiles, IContactRequestService requests) : UserPageModel
{
    public RequestsOverview Overview { get; private set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Tab { get; set; }

    public async Task OnGetAsync()
    {
        Tab = Tab is "sent" or "contacts" ? Tab : "incoming";
        var me = (await profiles.GetByUserIdAsync(UserId))!;
        Overview = await requests.GetOverviewAsync(me);
    }

    public Task<IActionResult> OnPostAcceptAsync(int requestId) => RespondAsync(requestId, true);

    public Task<IActionResult> OnPostDeclineAsync(int requestId) => RespondAsync(requestId, false);

    private async Task<IActionResult> RespondAsync(int requestId, bool accept)
    {
        switch (await requests.RespondAsync(requestId, UserId, accept))
        {
            case RespondResult.Success:
                TempData["Success"] = accept ? "Запит прийнято" : "Запит відхилено";
                return RedirectToPage(new { tab = "incoming" });
            case RespondResult.NotFound:
                return NotFound();
            default:
                return StatusCode(StatusCodes.Status403Forbidden);
        }
    }
}
