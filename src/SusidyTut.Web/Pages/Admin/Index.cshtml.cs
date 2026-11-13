using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SusidyTut.Web.Data;
using SusidyTut.Web.Models.ViewModels;
using SusidyTut.Web.Services;

namespace SusidyTut.Web.Pages.Admin;

[Authorize(Roles = DbInitializer.AdminRole)]
public class IndexModel(IAdminService admin) : PageModel
{
    public AdminStats Stats { get; private set; } = new();

    public PagedList<AdminUserRow> Users { get; private set; } = new();

    [BindProperty(SupportsGet = true)]
    [Display(Name = "Пошук за email")]
    [StringLength(100, ErrorMessage = "Не більше 100 символів")]
    public string? Email { get; set; }

    [BindProperty(SupportsGet = true, Name = "p")]
    public int PageNumber { get; set; } = 1;

    public async Task OnGetAsync()
    {
        Stats = await admin.GetStatsAsync();
        // Некоректний пошук — показуємо всіх
        var filter = ModelState.IsValid ? Email : null;
        Users = await admin.GetUsersAsync(filter, PageNumber);
    }

    public Task<IActionResult> OnPostBlockAsync(string userId) => SetBlockedAsync(userId, true);

    public Task<IActionResult> OnPostUnblockAsync(string userId) => SetBlockedAsync(userId, false);

    private async Task<IActionResult> SetBlockedAsync(string userId, bool blocked)
    {
        if (!await admin.SetBlockedAsync(userId, blocked))
            return NotFound();

        TempData["Success"] = blocked ? "Користувача заблоковано" : "Користувача розблоковано";
        var email = Email is { Length: <= 100 } ? Email : null;
        return RedirectToPage(new { email, p = PageNumber });
    }
}
