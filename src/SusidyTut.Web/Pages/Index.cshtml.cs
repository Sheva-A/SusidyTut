using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SusidyTut.Web.Infrastructure;

namespace SusidyTut.Web.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        // Табл. 4.1: увійшлих ведемо на свою стартову
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage(Format.HomePage(User));
        return Page();
    }
}
