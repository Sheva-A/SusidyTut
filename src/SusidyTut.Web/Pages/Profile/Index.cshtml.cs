using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SusidyTut.Web.Data;
using SusidyTut.Web.Models.ViewModels;
using SusidyTut.Web.Services;

namespace SusidyTut.Web.Pages.Profile;

[Authorize(Roles = DbInitializer.UserRole)]
public class IndexModel(IProfileService profiles) : UserPageModel
{
    protected override bool RequiresProfile => false;

    [BindProperty]
    public ProfileInput Input { get; set; } = new();

    public IEnumerable<SelectListItem> CityItems { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Input = await profiles.GetInputAsync(UserId);
        await LoadCitiesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Бізнес-правила перевіряє сервіс
        foreach (var error in await profiles.ValidateAsync(Input))
            ModelState.AddModelError($"{nameof(Input)}.{error.Field}", error.Message);

        if (!ModelState.IsValid)
        {
            await LoadCitiesAsync();
            return Page();
        }

        var created = await profiles.SaveAsync(UserId, Input);
        TempData["Success"] = "Анкету збережено";
        // ФК-04: після першого збереження — на Е-05
        return created ? RedirectToPage("/Search/Index") : RedirectToPage();
    }

    private async Task LoadCitiesAsync()
    {
        var cities = await profiles.GetCitiesAsync();
        CityItems = cities.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
    }
}
