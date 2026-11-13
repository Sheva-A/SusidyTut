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
public class LoginModel(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : PageModel
{
    [BindProperty]
    public LoginInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public string? ErrorMessage { get; private set; }

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

        var user = await userManager.FindByEmailAsync(Input.Email!);
        if (user is null)
            return Fail("Невірний email або пароль");

        // ФК-02: блок адміністратором після перевірки пароля
        if (user.IsBlocked)
        {
            return await userManager.CheckPasswordAsync(user, Input.Password!)
                ? Fail("Обліковий запис заблоковано адміністратором")
                : Fail("Невірний email або пароль");
        }

        var result = await signInManager.PasswordSignInAsync(user, Input.Password!, isPersistent: false, lockoutOnFailure: true);
        if (result.IsLockedOut)
            return Fail("Забагато невдалих спроб. Спробуйте через 5 хвилин");
        if (!result.Succeeded)
            return Fail("Невірний email або пароль");

        if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            return LocalRedirect(ReturnUrl);

        var isAdmin = await userManager.IsInRoleAsync(user, DbInitializer.AdminRole);
        return RedirectToPage(isAdmin ? "/Admin/Index" : "/Search/Index");
    }

    private PageResult Fail(string message)
    {
        ErrorMessage = message;
        return Page();
    }
}
