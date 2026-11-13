using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SusidyTut.Web.Infrastructure;

namespace SusidyTut.Web.Pages;

[AllowAnonymous]
[IgnoreAntiforgeryToken]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorModel : PageModel
{
    public int Code { get; private set; }

    public string Message { get; private set; } = string.Empty;

    public string HomeUrl { get; private set; } = "/";

    public void OnGet(int? code) => Fill(code);

    public void OnPost(int? code) => Fill(code);

    private void Fill(int? code)
    {
        Code = code is 403 or 404 ? code.Value : 500;
        Message = Code switch
        {
            404 => "Сторінку не знайдено",
            403 => "Доступ заборонено",
            _ => "Сталася помилка. Спробуйте пізніше."
        };
        Response.StatusCode = Code;
        HomeUrl = Url.Page(Format.HomePage(User)) ?? "/";
    }
}
