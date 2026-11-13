using Microsoft.EntityFrameworkCore;
using SusidyTut.Web.Data;
using SusidyTut.Web.Models;
using SusidyTut.Web.Models.ViewModels;

namespace SusidyTut.Web.Services;

/// <summary>Реалізація роботи з анкетами.</summary>
public class ProfileService(ApplicationDbContext db) : IProfileService
{
    /// <inheritdoc />
    public Task<Profile?> GetByUserIdAsync(string userId) =>
        db.Profiles.Include(p => p.City).FirstOrDefaultAsync(p => p.UserId == userId);

    /// <inheritdoc />
    public Task<bool> HasProfileAsync(string userId) =>
        db.Profiles.AnyAsync(p => p.UserId == userId);

    /// <inheritdoc />
    public async Task<ProfileInput> GetInputAsync(string userId)
    {
        var p = await db.Profiles.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
        if (p is null)
            return new ProfileInput();

        return new ProfileInput
        {
            Name = p.Name,
            Age = p.Age,
            Gender = p.Gender,
            PreferredGender = p.PreferredGender,
            CityId = p.CityId,
            University = p.University,
            Housing = p.Housing,
            BudgetMin = p.BudgetMin,
            BudgetMax = p.BudgetMax,
            SleepSchedule = p.SleepSchedule,
            Cleanliness = p.Cleanliness,
            Guests = p.Guests,
            Noise = p.Noise,
            Smoking = p.Smoking,
            Pets = p.Pets,
            About = p.About,
            Telegram = p.Telegram,
            Phone = p.Phone,
            IsActive = p.IsActive
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<City>> GetCitiesAsync() =>
        await db.Cities.AsNoTracking().OrderBy(c => c.Id).ToListAsync();

    /// <inheritdoc />
    public async Task<IReadOnlyList<FieldError>> ValidateAsync(ProfileInput input)
    {
        var errors = new List<FieldError>();

        if (input.BudgetMin is not null && input.BudgetMax is not null && input.BudgetMax < input.BudgetMin)
            errors.Add(new(nameof(ProfileInput.BudgetMax), "Сума «до» не може бути меншою за суму «від»"));

        if (string.IsNullOrWhiteSpace(input.Telegram) && string.IsNullOrWhiteSpace(input.Phone))
            errors.Add(new(nameof(ProfileInput.Telegram), "Вкажіть хоча б один контакт: Telegram або телефон"));

        if (input.CityId is not null && !await db.Cities.AnyAsync(c => c.Id == input.CityId))
            errors.Add(new(nameof(ProfileInput.CityId), "Оберіть варіант"));

        return errors;
    }

    /// <inheritdoc />
    public async Task<bool> SaveAsync(string userId, ProfileInput input)
    {
        var profile = await db.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
        var isNew = profile is null;
        var now = DateTime.UtcNow;

        if (profile is null)
        {
            profile = new Profile { UserId = userId, CreatedAt = now };
            db.Profiles.Add(profile);
        }

        profile.Name = input.Name!;
        profile.Age = input.Age!.Value;
        profile.Gender = input.Gender!.Value;
        profile.PreferredGender = input.PreferredGender!.Value;
        profile.CityId = input.CityId!.Value;
        profile.University = input.University;
        profile.Housing = input.Housing!.Value;
        profile.BudgetMin = input.BudgetMin!.Value;
        profile.BudgetMax = input.BudgetMax!.Value;
        profile.SleepSchedule = input.SleepSchedule!.Value;
        profile.Cleanliness = input.Cleanliness!.Value;
        profile.Guests = input.Guests!.Value;
        profile.Noise = input.Noise!.Value;
        profile.Smoking = input.Smoking!.Value;
        profile.Pets = input.Pets!.Value;
        profile.About = input.About;
        profile.Telegram = input.Telegram;
        profile.Phone = input.Phone;
        profile.IsActive = input.IsActive;
        profile.UpdatedAt = now;

        await db.SaveChangesAsync();
        return isNew;
    }
}
