using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SusidyTut.Web.Data;
using SusidyTut.Web.Infrastructure;
using SusidyTut.Web.Models;
using SusidyTut.Web.Models.ViewModels;

namespace SusidyTut.Web.Pages.Account;

[AllowAnonymous]
public class RegisterModel(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : PageModel
{
    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage(Format.HomePage(User));
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage(Format.HomePage(User));

        if (!ModelState.IsValid)
            return Page();

        // ФК-01: email без урахування регістру
        if (await userManager.FindByEmailAsync(Input.Email!) is not null)
        {
            ModelState.AddModelError("Input.Email", "Користувач з таким email вже існує");
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            RegisteredAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, Input.Password!);
        if (!result.Succeeded)
        {
            var duplicate = result.Errors.Any(e => e.Code is "DuplicateEmail" or "DuplicateUserName");
            ModelState.AddModelError(duplicate ? "Input.Email" : string.Empty,
                duplicate ? "Користувач з таким email вже існує" : "Не вдалося створити обліковий запис");
            return Page();
        }

        await userManager.AddToRoleAsync(user, DbInitializer.UserRole);
        await signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToPage("/Profile/Index");
    }
}
