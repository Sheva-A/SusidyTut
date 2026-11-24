using SusidyTut.Web.Models;
using SusidyTut.Web.Models.Enums;

namespace SusidyTut.Tests;

/// <summary>Створення тестових анкет зі значеннями за замовчуванням.</summary>
internal static class ProfileFactory
{
    /// <summary>Анкета, сумісна з будь-якою іншою анкетою за замовчуванням.</summary>
    public static Profile Make(
        string name = "Тест",
        int cityId = 1,
        int budgetMin = 3000,
        int budgetMax = 5000,
        Gender gender = Gender.Female,
        PreferredGender preferred = PreferredGender.Any,
        SleepSchedule sleep = SleepSchedule.Medium,
        Cleanliness cleanliness = Cleanliness.Medium,
        GuestsFrequency guests = GuestsFrequency.Sometimes,
        NoiseTolerance noise = NoiseTolerance.Moderate,
        SmokingAttitude smoking = SmokingAttitude.NonSmokerTolerant,
        PetsAttitude pets = PetsAttitude.NoPetTolerant) => new()
    {
        Name = name,
        Age = 20,
        CityId = cityId,
        BudgetMin = budgetMin,
        BudgetMax = budgetMax,
        Gender = gender,
        PreferredGender = preferred,
        SleepSchedule = sleep,
        Cleanliness = cleanliness,
        Guests = guests,
        Noise = noise,
        Smoking = smoking,
        Pets = pets,
        IsActive = true
    };
}
