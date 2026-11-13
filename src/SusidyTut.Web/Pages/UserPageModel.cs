using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SusidyTut.Web.Data;
using SusidyTut.Web.Services;

namespace SusidyTut.Web.Pages;

/// <summary>Базова сторінка для ролі User; вимагає анкету (табл. 4.1).</summary>
[Authorize(Roles = DbInitializer.UserRole)]
public abstract class UserPageModel : PageModel
{
    /// <summary>Id поточного користувача.</summary>
    protected string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    /// <summary>Чи потрібна анкета для доступу.</summary>
    protected virtual bool RequiresProfile => true;

    public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        if (RequiresProfile)
        {
            var profiles = HttpContext.RequestServices.GetRequiredService<IProfileService>();
            if (!await profiles.HasProfileAsync(UserId))
            {
                TempData["Warning"] = "Спочатку заповніть анкету";
                context.Result = RedirectToPage("/Profile/Index");
                return;
            }
        }
        await next();
    }
}
