using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SusidyTut.Web.Models;

namespace SusidyTut.Web.Services;

/// <summary>SignInManager, що не пускає заблокованих користувачів (ФА-03).</summary>
public class AppSignInManager(
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor contextAccessor,
    IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
    IOptions<IdentityOptions> optionsAccessor,
    ILogger<SignInManager<ApplicationUser>> logger,
    IAuthenticationSchemeProvider schemes,
    IUserConfirmation<ApplicationUser> confirmation)
    : SignInManager<ApplicationUser>(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
{
    /// <summary>Забороняє вхід, якщо IsBlocked = true.</summary>
    /// <param name="user">Користувач.</param>
    /// <returns>true, якщо вхід дозволено.</returns>
    public override async Task<bool> CanSignInAsync(ApplicationUser user) =>
        !user.IsBlocked && await base.CanSignInAsync(user);

    /// <summary>Скидає сесію заблокованого під час перевірки штампа.</summary>
    /// <param name="principal">Поточний користувач.</param>
    /// <returns>Користувач або null, якщо сесія недійсна.</returns>
    public override async Task<ApplicationUser?> ValidateSecurityStampAsync(System.Security.Claims.ClaimsPrincipal? principal)
    {
        var user = await base.ValidateSecurityStampAsync(principal);
        return user is { IsBlocked: true } ? null : user;
    }
}
