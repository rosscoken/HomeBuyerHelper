using Xunit;
using FluentAssertions;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Core.Services;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for IncomeScenarioService (P2-INC-006).
/// </summary>
public class IncomeScenarioServiceTests
{
    private readonly IncomeScenarioService _service = new();

    private static IncomeSource MonthlySalary(decimal monthly) => new()
    {
        Name = "Salary",
        GrossAmount = monthly,
        Frequency = IncomeFrequency.Monthly,
        IsReliable = true
    };

    private static IncomeSource Bonus(decimal amount, int month, decimal probability) => new()
    {
        Name = "Annual Bonus",
        GrossAmount = amount,
        Frequency = IncomeFrequency.Annually,
        IncomeType = IncomeType.Bonus,
        IsReliable = false,
        PaymentMonth = month,
        Probability = probability
    };

    [Fact]
    public void Conservative_ExcludesVariableIncome()
    {
        var sources = new[] { MonthlySalary(10_000m), Bonus(20_000m, 3, 80) };
        var march = new DateTime(2026, 3, 1);

        var total = _service.GetTotalForMonth(sources, march, IncomeScenario.Conservative);

        total.Should().Be(10_000m);
    }

    [Fact]
    public void Realistic_WeightsVariableIncomeByProbability()
    {
        var sources = new[] { MonthlySalary(10_000m), Bonus(20_000m, 3, 80) };
        var march = new DateTime(2026, 3, 1);

        var total = _service.GetTotalForMonth(sources, march, IncomeScenario.Realistic);

        total.Should().Be(10_000m + 20_000m * 0.8m);
    }

    [Fact]
    public void Expected_IncludesFullVariableIncome()
    {
        var sources = new[] { MonthlySalary(10_000m), Bonus(20_000m, 3, 80) };
        var march = new DateTime(2026, 3, 1);

        var total = _service.GetTotalForMonth(sources, march, IncomeScenario.Expected);

        total.Should().Be(30_000m);
    }

    [Fact]
    public void Bonus_OnlyLandsInItsPaymentMonth()
    {
        var bonus = Bonus(20_000m, 3, 100);

        _service.GetAmountForMonth(bonus, new DateTime(2026, 3, 1), IncomeScenario.Expected).Should().Be(20_000m);
        _service.GetAmountForMonth(bonus, new DateTime(2026, 4, 1), IncomeScenario.Expected).Should().Be(0);
    }

    [Fact]
    public void QuarterlyVest_LandsEveryThreeMonthsFromAnchor()
    {
        var rsu = new IncomeSource
        {
            Name = "RSU Vest",
            GrossAmount = 15_000m,
            Frequency = IncomeFrequency.Quarterly,
            IncomeType = IncomeType.RSU,
            IsReliable = false,
            PaymentMonth = 2, // Feb/May/Aug/Nov
            Probability = 100
        };

        _service.GetAmountForMonth(rsu, new DateTime(2026, 2, 1), IncomeScenario.Expected).Should().Be(15_000m);
        _service.GetAmountForMonth(rsu, new DateTime(2026, 5, 1), IncomeScenario.Expected).Should().Be(15_000m);
        _service.GetAmountForMonth(rsu, new DateTime(2026, 11, 1), IncomeScenario.Expected).Should().Be(15_000m);
        _service.GetAmountForMonth(rsu, new DateTime(2026, 3, 1), IncomeScenario.Expected).Should().Be(0);
        _service.GetAmountForMonth(rsu, new DateTime(2026, 6, 1), IncomeScenario.Expected).Should().Be(0);
    }

    [Fact]
    public void QuarterlyAnchor_HandlesMonthsBeforeAnchor()
    {
        var rsu = new IncomeSource
        {
            Name = "RSU Vest",
            GrossAmount = 15_000m,
            Frequency = IncomeFrequency.Quarterly,
            IsReliable = false,
            PaymentMonth = 11 // Nov cycle: Feb/May/Aug/Nov
        };

        // January is before the November anchor: (1 - 11) % 3 must wrap correctly.
        _service.GetAmountForMonth(rsu, new DateTime(2026, 1, 1), IncomeScenario.Expected).Should().Be(0);
        _service.GetAmountForMonth(rsu, new DateTime(2026, 2, 1), IncomeScenario.Expected).Should().Be(15_000m);
    }

    [Fact]
    public void StartDate_DelaysIncome()
    {
        var partner = new IncomeSource
        {
            Name = "Partner Contribution",
            GrossAmount = 2_000m,
            Frequency = IncomeFrequency.Monthly,
            IncomeType = IncomeType.PartnerContribution,
            IsReliable = true,
            StartDate = new DateTime(2026, 9, 1)
        };

        _service.GetAmountForMonth(partner, new DateTime(2026, 8, 1), IncomeScenario.Conservative).Should().Be(0);
        _service.GetAmountForMonth(partner, new DateTime(2026, 9, 1), IncomeScenario.Conservative).Should().Be(2_000m);
        _service.GetAmountForMonth(partner, new DateTime(2027, 1, 1), IncomeScenario.Conservative).Should().Be(2_000m);
    }

    [Fact]
    public void EndDate_StopsIncome()
    {
        var rental = new IncomeSource
        {
            Name = "Rental",
            GrossAmount = 1_500m,
            Frequency = IncomeFrequency.Monthly,
            IsReliable = true,
            EndDate = new DateTime(2026, 6, 30)
        };

        _service.GetAmountForMonth(rental, new DateTime(2026, 6, 1), IncomeScenario.Conservative).Should().Be(1_500m);
        _service.GetAmountForMonth(rental, new DateTime(2026, 7, 1), IncomeScenario.Conservative).Should().Be(0);
    }

    [Fact]
    public void AverageMonthlyIncome_SmoothsLumpyIncome()
    {
        var sources = new[]
        {
            MonthlySalary(10_000m),
            Bonus(24_000m, 3, 100) // Annual = 2,000/month smoothed
        };

        var average = _service.GetAverageMonthlyIncome(sources, IncomeScenario.Expected);

        average.Should().Be(12_000m);
    }

    [Fact]
    public void BiWeeklyIncome_ConvertsToMonthlyEquivalent()
    {
        var salary = new IncomeSource
        {
            Name = "Bi-weekly Pay",
            GrossAmount = 3_000m,
            Frequency = IncomeFrequency.BiWeekly,
            IsReliable = true
        };

        var monthly = _service.GetAmountForMonth(salary, new DateTime(2026, 1, 1), IncomeScenario.Conservative);

        monthly.Should().Be(3_000m * 26 / 12);
    }

    [Theory]
    [InlineData(100, 50, 22, 3900)]      // 100 shares * $50 = $5000 gross, 22% withheld
    [InlineData(250, 120.50, 37, 18978.75)]
    [InlineData(0, 50, 22, 0)]
    public void RsuNetPerVest_CalculatesAfterWithholding(decimal shares, decimal price, decimal withholding, decimal expected)
    {
        _service.CalculateRsuNetPerVest(shares, price, withholding).Should().Be(expected);
    }
}
