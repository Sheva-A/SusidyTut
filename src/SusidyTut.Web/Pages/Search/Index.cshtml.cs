using Microsoft.AspNetCore.Authorization;
using SusidyTut.Web.Data;
using SusidyTut.Web.Models.ViewModels;
using SusidyTut.Web.Services;

namespace SusidyTut.Web.Pages.Search;

[Authorize(Roles = DbInitializer.UserRole)]
public class IndexModel(IProfileService profiles, IMatchingService matching) : UserPageModel
{
    public SearchResult Result { get; private set; } = new();

    public bool IsProfileActive { get; private set; }

    public async Task OnGetAsync()
    {
        var me = (await profiles.GetByUserIdAsync(UserId))!;
        IsProfileActive = me.IsActive;
        Result = await matching.FindCandidatesAsync(me);
    }
}
