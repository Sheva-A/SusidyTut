using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SusidyTut.Web.Data;
using SusidyTut.Web.Models;
using SusidyTut.Web.Models.Enums;
using SusidyTut.Web.Models.ViewModels;

namespace SusidyTut.Web.Services;

/// <summary>Реалізація адміністрування.</summary>
public class AdminService(ApplicationDbContext db, UserManager<ApplicationUser> userManager) : IAdminService
{
    /// <summary>ФА-02: записів на сторінці.</summary>
    public const int PageSize = 25;

    /// <inheritdoc />
    public async Task<AdminStats> GetStatsAsync()
    {
        var users = UsersInRole();
        var byStatus = await db.ContactRequests
            .GroupBy(r => r.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count);

        return new AdminStats
        {
            UsersTotal = await users.CountAsync(),
            UsersBlocked = await users.CountAsync(u => u.IsBlocked),
            ActiveProfiles = await db.Profiles.CountAsync(p => p.IsActive),
            RequestsPending = byStatus.GetValueOrDefault(RequestStatus.Pending),
            RequestsAccepted = byStatus.GetValueOrDefault(RequestStatus.Accepted),
            RequestsDeclined = byStatus.GetValueOrDefault(RequestStatus.Declined)
        };
    }

    /// <inheritdoc />
    public async Task<PagedList<AdminUserRow>> GetUsersAsync(string? emailFilter, int page)
    {
        var query = UsersInRole();
        if (!string.IsNullOrWhiteSpace(emailFilter))
        {
            // Пошук без регістру через NormalizedEmail
            var normalized = userManager.NormalizeEmail(emailFilter.Trim());
            query = query.Where(u => u.NormalizedEmail!.Contains(normalized));
        }

        var total = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);

        var items = await query
            .OrderByDescending(u => u.RegisteredAt)
            .ThenBy(u => u.Email)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(u => new AdminUserRow
            {
                Id = u.Id,
                Email = u.Email!,
                Name = u.Profile != null ? u.Profile.Name : null,
                RegisteredAt = u.RegisteredAt,
                IsBlocked = u.IsBlocked
            })
            .ToListAsync();

        return new PagedList<AdminUserRow> { Items = items, Page = page, TotalPages = totalPages };
    }

    /// <inheritdoc />
    public async Task<bool> SetBlockedAsync(string userId, bool blocked)
    {
        var user = await UsersInRole().FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            return false;

        user.IsBlocked = blocked;
        await userManager.UpdateAsync(user);
        // ФА-03: скидаємо сесії користувача
        await userManager.UpdateSecurityStampAsync(user);
        return true;
    }

    private IQueryable<ApplicationUser> UsersInRole()
    {
        var roleId = db.Roles.Where(r => r.Name == DbInitializer.UserRole).Select(r => r.Id);
        return db.Users.Where(u => db.UserRoles.Any(ur => ur.UserId == u.Id && roleId.Contains(ur.RoleId)));
    }
}
