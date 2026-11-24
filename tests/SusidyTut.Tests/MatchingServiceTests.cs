using SusidyTut.Web.Models;
using SusidyTut.Web.Models.Enums;
using SusidyTut.Web.Services;
using static SusidyTut.Tests.ProfileFactory;

namespace SusidyTut.Tests;

/// <summary>Тести алгоритму підбору (ДВ-01, ДВ-02).</summary>
public class MatchingServiceTests
{
    // IsCandidate і CalculateCompatibility не звертаються до БД
    private readonly MatchingService _service = new(null!);

    /// <summary>Контрольна пара: дві анкети та очікуваний результат.</summary>
    public record ControlPair(string Title, Profile A, Profile B, bool IsCandidate, int Percent)
    {
        public override string ToString() => Title;
    }

    /// <summary>10 контрольних пар з очікуваним відбором і відсотком сумісності.</summary>
    public static TheoryData<ControlPair> ControlPairs => new()
    {
        new("01. Олена і Марко (приклад з ТЗ)",
            Make("Олена", gender: Gender.Female, sleep: SleepSchedule.Early, cleanliness: Cleanliness.High,
                guests: GuestsFrequency.Sometimes, noise: NoiseTolerance.Quiet),
            Make("Марко", gender: Gender.Male, sleep: SleepSchedule.Medium, cleanliness: Cleanliness.High,
                guests: GuestsFrequency.Sometimes, noise: NoiseTolerance.Any),
            true, 60),

        new("02. Однакові звички",
            Make("А"), Make("Б"),
            true, 100),

        new("03. Протилежні звички",
            Make("А", sleep: SleepSchedule.Early, cleanliness: Cleanliness.Low,
                guests: GuestsFrequency.Rare, noise: NoiseTolerance.Quiet),
            Make("Б", sleep: SleepSchedule.Late, cleanliness: Cleanliness.High,
                guests: GuestsFrequency.Often, noise: NoiseTolerance.Any),
            true, 0),

        new("04. Сергій і Тарас (демо-дані)",
            Make("Сергій", gender: Gender.Male, preferred: PreferredGender.Male, sleep: SleepSchedule.Late,
                cleanliness: Cleanliness.Low, guests: GuestsFrequency.Sometimes, noise: NoiseTolerance.Quiet),
            Make("Тарас", gender: Gender.Male, preferred: PreferredGender.Male, sleep: SleepSchedule.Early,
                cleanliness: Cleanliness.Low, guests: GuestsFrequency.Sometimes, noise: NoiseTolerance.Quiet),
            true, 75),

        new("05. Сусідні відповіді за всіма критеріями",
            Make("А", sleep: SleepSchedule.Early, cleanliness: Cleanliness.Low,
                guests: GuestsFrequency.Rare, noise: NoiseTolerance.Quiet),
            Make("Б", sleep: SleepSchedule.Medium, cleanliness: Cleanliness.Medium,
                guests: GuestsFrequency.Sometimes, noise: NoiseTolerance.Moderate),
            true, 40),

        new("06. Бюджети торкаються межею",
            Make("А", budgetMin: 3000, budgetMax: 4000),
            Make("Б", budgetMin: 4000, budgetMax: 5000),
            true, 100),

        new("07. Різні міста",
            Make("А", cityId: 1), Make("Б", cityId: 2),
            false, 100),

        new("08. Бюджети не перетинаються",
            Make("А", budgetMin: 3000, budgetMax: 4000),
            Make("Б", budgetMin: 4500, budgetMax: 6000),
            false, 100),

        new("09. Бажана стать не збігається",
            Make("А", gender: Gender.Female, preferred: PreferredGender.Female),
            Make("Б", gender: Gender.Male, preferred: PreferredGender.Any),
            false, 100),

        new("10. Курить і проти куріння",
            Make("А", smoking: SmokingAttitude.Smoker),
            Make("Б", smoking: SmokingAttitude.NonSmokerAgainst),
            false, 100)
    };

    [Theory]
    [MemberData(nameof(ControlPairs))]
    public void ControlPair_IsCandidate(ControlPair pair) =>
        Assert.Equal(pair.IsCandidate, _service.IsCandidate(pair.A, pair.B));

    [Theory]
    [MemberData(nameof(ControlPairs))]
    public void ControlPair_Compatibility(ControlPair pair) =>
        Assert.Equal(pair.Percent, _service.CalculateCompatibility(pair.A, pair.B));

    [Theory]
    [MemberData(nameof(ControlPairs))]
    public void ControlPair_IsSymmetric(ControlPair pair)
    {
        Assert.Equal(_service.IsCandidate(pair.A, pair.B), _service.IsCandidate(pair.B, pair.A));
        Assert.Equal(_service.CalculateCompatibility(pair.A, pair.B),
            _service.CalculateCompatibility(pair.B, pair.A));
    }

    [Theory]
    [InlineData(1, 1, 25)]
    [InlineData(2, 2, 25)]
    [InlineData(1, 2, 10)]
    [InlineData(3, 2, 10)]
    [InlineData(1, 3, 0)]
    [InlineData(3, 1, 0)]
    public void Points_DependOnDifference(int codeA, int codeB, int expected) =>
        Assert.Equal(expected, MatchingService.Points(codeA, codeB));

    [Fact]
    public void PetsConflict_IsNotCandidate()
    {
        var a = Make("А", pets: PetsAttitude.HasPet);
        var b = Make("Б", pets: PetsAttitude.NoPetAgainst);

        Assert.False(_service.IsCandidate(a, b));
    }

    [Fact]
    public void ToleratedSmokingAndPets_IsCandidate()
    {
        var a = Make("А", smoking: SmokingAttitude.Smoker, pets: PetsAttitude.HasPet);
        var b = Make("Б", smoking: SmokingAttitude.NonSmokerTolerant, pets: PetsAttitude.NoPetTolerant);

        Assert.True(_service.IsCandidate(a, b));
    }

    [Fact]
    public void GenderMustFitBothWays()
    {
        // А підходить Б, але Б шукає лише жінок
        var a = Make("А", gender: Gender.Male, preferred: PreferredGender.Any);
        var b = Make("Б", gender: Gender.Female, preferred: PreferredGender.Female);

        Assert.False(_service.IsCandidate(a, b));
    }

    [Fact]
    public void Criteria_SumEqualsCompatibility()
    {
        var a = Make("А", sleep: SleepSchedule.Early, noise: NoiseTolerance.Quiet);
        var b = Make("Б", sleep: SleepSchedule.Late, noise: NoiseTolerance.Moderate);

        var criteria = _service.GetCriteria(a, b);

        Assert.Equal(4, criteria.Count);
        Assert.Equal(_service.CalculateCompatibility(a, b), criteria.Sum(c => c.Points));
    }
}
