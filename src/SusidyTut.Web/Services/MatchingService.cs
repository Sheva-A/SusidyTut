using Microsoft.EntityFrameworkCore;
using SusidyTut.Web.Data;
using SusidyTut.Web.Infrastructure;
using SusidyTut.Web.Models;
using SusidyTut.Web.Models.Enums;
using SusidyTut.Web.Models.ViewModels;

namespace SusidyTut.Web.Services;

/// <summary>Реалізація підбору кандидатів (ДВ-01–ДВ-03).</summary>
public class MatchingService(ApplicationDbContext db) : IMatchingService
{
    /// <summary>ДВ-03: максимум карток на Е-05.</summary>
    public const int MaxCards = 50;

    /// <inheritdoc />
    public bool IsCandidate(Profile a, Profile b)
    {
        // В4: однакове місто
        if (a.CityId != b.CityId)
            return false;

        // В5: перетин бюджетів, межі включно
        if (a.BudgetMin > b.BudgetMax || b.BudgetMin > a.BudgetMax)
            return false;

        // В6: стать узгоджена в обидва боки
        if (!GenderFits(a.PreferredGender, b.Gender) || !GenderFits(b.PreferredGender, a.Gender))
            return false;

        // В7: «Курю» несумісне з «проти куріння»
        if (IsSmokingConflict(a.Smoking, b.Smoking))
            return false;

        // В8: «Маю тварину» несумісне з «проти тварин»
        if (IsPetsConflict(a.Pets, b.Pets))
            return false;

        return true;
    }

    /// <inheritdoc />
    public int CalculateCompatibility(Profile a, Profile b) =>
        // ДВ-02: сума балів за 4 критеріями
        Points((int)a.SleepSchedule, (int)b.SleepSchedule)
        + Points((int)a.Cleanliness, (int)b.Cleanliness)
        + Points((int)a.Guests, (int)b.Guests)
        + Points((int)a.Noise, (int)b.Noise);

    /// <inheritdoc />
    public IReadOnlyList<CriterionScore> GetCriteria(Profile a, Profile b) =>
    [
        new("Режим сну", Format.EnumName(a.SleepSchedule), Format.EnumName(b.SleepSchedule),
            Points((int)a.SleepSchedule, (int)b.SleepSchedule)),
        new("Чистота", Format.EnumName(a.Cleanliness), Format.EnumName(b.Cleanliness),
            Points((int)a.Cleanliness, (int)b.Cleanliness)),
        new("Гості", Format.EnumName(a.Guests), Format.EnumName(b.Guests),
            Points((int)a.Guests, (int)b.Guests)),
        new("Шум", Format.EnumName(a.Noise), Format.EnumName(b.Noise),
            Points((int)a.Noise, (int)b.Noise))
    ];

    /// <summary>Бали за одним критерієм за різницею кодів.</summary>
    /// <param name="codeA">Код у користувача А (1–3).</param>
    /// <param name="codeB">Код у користувача Б (1–3).</param>
    /// <returns>25, 10 або 0 балів.</returns>
    public static int Points(int codeA, int codeB) => Math.Abs(codeA - codeB) switch
    {
        0 => 25, // ДВ-02: різниця 0 дає 25 балів
        1 => 10, // ДВ-02: різниця 1 дає 10 балів
        _ => 0   // ДВ-02: різниця 2 дає 0 балів
    };

    /// <inheritdoc />
    public async Task<SearchResult> FindCandidatesAsync(Profile me)
    {
        var pool = await CandidatePoolQuery(me).ToListAsync();

        // ДВ-03: % ↓, UpdatedAt ↓, Id ↑
        var ranked = pool
            .Where(p => IsCandidate(me, p))
            .Select(p => (Profile: p, Percent: CalculateCompatibility(me, p)))
            .OrderByDescending(x => x.Percent)
            .ThenByDescending(x => x.Profile.UpdatedAt)
            .ThenBy(x => x.Profile.Id)
            .ToList();

        var top = ranked.Take(MaxCards).ToList();
        var ids = top.Select(x => x.Profile.UserId).ToList();

        var requests = await db.ContactRequests.AsNoTracking()
            .Where(r => (r.SenderId == me.UserId && ids.Contains(r.ReceiverId))
                        || (r.ReceiverId == me.UserId && ids.Contains(r.SenderId)))
            .ToListAsync();
        var byOther = requests.ToDictionary(r => r.SenderId == me.UserId ? r.ReceiverId : r.SenderId);

        var cards = top.Select(x => new CandidateCard
        {
            ProfileId = x.Profile.Id,
            Name = x.Profile.Name,
            Age = x.Profile.Age,
            University = x.Profile.University,
            BudgetMin = x.Profile.BudgetMin,
            BudgetMax = x.Profile.BudgetMax,
            Housing = x.Profile.Housing,
            Compatibility = x.Percent,
            RequestState = RequestStates.From(byOther.GetValueOrDefault(x.Profile.UserId), me.UserId)
        }).ToList();

        return new SearchResult { TotalCount = ranked.Count, Cards = cards };
    }

    /// <inheritdoc />
    public async Task<bool> IsCandidateAsync(Profile me, string otherUserId)
    {
        var other = await CandidatePoolQuery(me).FirstOrDefaultAsync(p => p.UserId == otherUserId);
        return other is not null && IsCandidate(me, other);
    }

    /// <inheritdoc />
    public async Task<Profile?> GetViewableProfileAsync(Profile me, int profileId)
    {
        var userRoleId = await UserRoleIdAsync();
        var other = await db.Profiles.AsNoTracking()
            .Include(p => p.City)
            .Where(p => p.Id == profileId
                        && p.UserId != me.UserId
                        && !p.User!.IsBlocked
                        && db.UserRoles.Any(ur => ur.UserId == p.UserId && ur.RoleId == userRoleId))
            .FirstOrDefaultAsync();
        if (other is null)
            return null;

        // ФК-08: кандидат або вже є запит
        var hasRequest = await db.ContactRequests.AnyAsync(r =>
            (r.SenderId == me.UserId && r.ReceiverId == other.UserId)
            || (r.SenderId == other.UserId && r.ReceiverId == me.UserId));
        if (hasRequest)
            return other;

        return other.IsActive && IsCandidate(me, other) ? other : null;
    }

    /// <summary>Анкети, що проходять В1–В3 (і звуження за В4, В5).</summary>
    private IQueryable<Profile> CandidatePoolQuery(Profile me)
    {
        var userRoleId = db.Roles.Where(r => r.Name == DbInitializer.UserRole).Select(r => r.Id);
        return db.Profiles.AsNoTracking()
            .Where(p => p.UserId != me.UserId)                       // В1: не сам користувач
            .Where(p => !p.User!.IsBlocked
                        && db.UserRoles.Any(ur => ur.UserId == p.UserId
                                                  && userRoleId.Contains(ur.RoleId))) // В2: роль User, не заблокований
            .Where(p => p.IsActive)                                  // В3: анкета активна
            .Where(p => p.CityId == me.CityId)                       // В4: звуження в БД
            .Where(p => p.BudgetMin <= me.BudgetMax && me.BudgetMin <= p.BudgetMax); // В5: звуження в БД
    }

    private Task<string> UserRoleIdAsync() =>
        db.Roles.Where(r => r.Name == DbInitializer.UserRole).Select(r => r.Id).SingleAsync();

    private static bool GenderFits(PreferredGender preferred, Gender actual) =>
        preferred == PreferredGender.Any
        || (preferred == PreferredGender.Male && actual == Gender.Male)
        || (preferred == PreferredGender.Female && actual == Gender.Female);

    private static bool IsSmokingConflict(SmokingAttitude a, SmokingAttitude b) =>
        (a == SmokingAttitude.Smoker && b == SmokingAttitude.NonSmokerAgainst)
        || (a == SmokingAttitude.NonSmokerAgainst && b == SmokingAttitude.Smoker);

    private static bool IsPetsConflict(PetsAttitude a, PetsAttitude b) =>
        (a == PetsAttitude.HasPet && b == PetsAttitude.NoPetAgainst)
        || (a == PetsAttitude.NoPetAgainst && b == PetsAttitude.HasPet);
}
