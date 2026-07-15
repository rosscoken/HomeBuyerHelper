using FluentAssertions;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Core.Services;
using Xunit;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for CashFlowProjectionService (P2-CFP-001, P2-EMF-002).
/// </summary>
public class CashFlowProjectionServiceTests
{
    private readonly CashFlowProjectionService _service = new(new IncomeScenarioService());

    private static readonly DateTime Start = new(2026, 1, 1);

    private static IncomeSource MonthlyIncome(decimal amount) => new()
    {
        Name = "Salary",
        GrossAmount = amount,
        Frequency = IncomeFrequency.Monthly,
        IsReliable = true
    };

    private static Expense Fixed(string name, decimal amount) => new()
    {
        Name = name,
        Amount = amount,
        Frequency = ExpenseFrequency.Monthly,
        IsVariable = false
    };

    private static Expense Variable(string name, decimal amount) => new()
    {
        Name = name,
        Amount = amount,
        Frequency = ExpenseFrequency.Monthly,
        IsVariable = true
    };

    [Fact]
    public void Project_Returns24MonthsByDefault()
    {
        var result = _service.Project(new CashFlowProjectionInput
        {
            IncomeSources = new[] { MonthlyIncome(5_000m) },
            StartMonth = Start
        });

        result.Should().HaveCount(24);
        result[0].Month.Should().Be(Start);
        result[23].Month.Should().Be(Start.AddMonths(23));
    }

    [Fact]
    public void Project_CalculatesSurplusAndCumulative()
    {
        var result = _service.Project(new CashFlowProjectionInput
        {
            IncomeSources = new[] { MonthlyIncome(6_000m) },
            Expenses = new[] { Fixed("Rent", 2_000m), Variable("Groceries", 1_000m) },
            StartMonth = Start,
            Months = 3
        });

        result.Should().AllSatisfy(month =>
        {
            month.TotalIncome.Should().Be(6_000m);
            month.FixedExpenses.Should().Be(2_000m);
            month.VariableExpenses.Should().Be(1_000m);
            month.Surplus.Should().Be(3_000m);
        });

        result[0].CumulativeSurplus.Should().Be(3_000m);
        result[1].CumulativeSurplus.Should().Be(6_000m);
        result[2].CumulativeSurplus.Should().Be(9_000m);
    }

    [Fact]
    public void Project_IncludesOneTimeEventInItsMonthOnly()
    {
        var result = _service.Project(new CashFlowProjectionInput
        {
            IncomeSources = new[] { MonthlyIncome(5_000m) },
            OneTimeEvents = new[]
            {
                new OneTimeEvent { Name = "Moving Costs", Amount = 4_000m, Date = new DateTime(2026, 3, 15) }
            },
            StartMonth = Start,
            Months = 4
        });

        result[0].OneTimeExpenses.Should().Be(0);
        result[2].OneTimeExpenses.Should().Be(4_000m);
        result[2].EventNames.Should().ContainSingle().Which.Should().Be("Moving Costs");
        result[3].OneTimeExpenses.Should().Be(0);
    }

    [Fact]
    public void Project_DetectsCrunchMonths()
    {
        var result = _service.Project(new CashFlowProjectionInput
        {
            IncomeSources = new[] { MonthlyIncome(5_000m) },
            Expenses = new[] { Fixed("Bills", 4_000m) },
            OneTimeEvents = new[]
            {
                new OneTimeEvent { Name = "New Roof", Amount = 10_000m, Date = new DateTime(2026, 2, 1) }
            },
            StartMonth = Start,
            Months = 3
        });

        result[0].IsCrunchMonth.Should().BeFalse();
        result[1].IsCrunchMonth.Should().BeTrue();
        result[2].IsCrunchMonth.Should().BeFalse();
    }

    [Fact]
    public void EmergencyFund_DrawsDuringDeficitAndRecovers()
    {
        var result = _service.Project(new CashFlowProjectionInput
        {
            IncomeSources = new[] { MonthlyIncome(5_000m) },
            Expenses = new[] { Fixed("Bills", 4_000m) },
            OneTimeEvents = new[]
            {
                new OneTimeEvent { Name = "Repair", Amount = 3_000m, Date = new DateTime(2026, 2, 1) }
            },
            StartMonth = Start,
            Months = 4,
            EmergencyFund = new EmergencyFundConfig { TargetMonths = 6, CurrentBalance = 10_000m }
        });

        // Target = 6 * 4000 = 24,000. Starting balance 10,000 is below target,
        // so surplus months contribute toward it.
        result[0].EmergencyFundBalance.Should().Be(11_000m);  // +1000 surplus
        // Feb: deficit of 2000 (5000 - 4000 - 3000) draws from the fund.
        result[1].EmergencyFundBalance.Should().Be(9_000m);
        // Mar/Apr: surplus replenishes again.
        result[2].EmergencyFundBalance.Should().Be(10_000m);
        result[3].EmergencyFundBalance.Should().Be(11_000m);
    }

    [Fact]
    public void EmergencyFund_NeverGoesNegative()
    {
        var result = _service.Project(new CashFlowProjectionInput
        {
            IncomeSources = new[] { MonthlyIncome(1_000m) },
            Expenses = new[] { Fixed("Bills", 3_000m) },
            StartMonth = Start,
            Months = 3,
            EmergencyFund = new EmergencyFundConfig { TargetMonths = 6, CurrentBalance = 3_000m }
        });

        // Monthly deficit of 2000 drains the 3000 fund.
        result[0].EmergencyFundBalance.Should().Be(1_000m);
        result[1].EmergencyFundBalance.Should().Be(0m);
        result[2].EmergencyFundBalance.Should().Be(0m);
    }

    [Fact]
    public void EmergencyFund_WarnsBelowThreeMonthsOfExpenses()
    {
        var result = _service.Project(new CashFlowProjectionInput
        {
            IncomeSources = new[] { MonthlyIncome(4_100m) },
            Expenses = new[] { Fixed("Bills", 4_000m) },
            StartMonth = Start,
            Months = 2,
            // Warning threshold = 3 * 4000 = 12,000
            EmergencyFund = new EmergencyFundConfig { TargetMonths = 6, CurrentBalance = 11_000m }
        });

        // Surplus is only 100/month, so the fund stays below the 12,000 threshold.
        result[0].EmergencyFundWarning.Should().BeTrue();   // 11,100 < 12,000
        result[1].EmergencyFundWarning.Should().BeTrue();   // 11,200 < 12,000
    }

    [Fact]
    public void EmergencyFund_StopsContributingAtTarget()
    {
        var result = _service.Project(new CashFlowProjectionInput
        {
            IncomeSources = new[] { MonthlyIncome(5_000m) },
            Expenses = new[] { Fixed("Bills", 1_000m) },
            StartMonth = Start,
            Months = 2,
            // Target = 6 * 1000 = 6000, already there.
            EmergencyFund = new EmergencyFundConfig { TargetMonths = 6, CurrentBalance = 6_000m }
        });

        result[0].EmergencyFundBalance.Should().Be(6_000m);
        result[1].EmergencyFundBalance.Should().Be(6_000m);
    }

    [Fact]
    public void Project_IncludesHousingCostInExpenses()
    {
        var result = _service.Project(new CashFlowProjectionInput
        {
            IncomeSources = new[] { MonthlyIncome(8_000m) },
            Expenses = new[] { Fixed("Car", 500m) },
            MonthlyHousingCost = 3_200m,
            StartMonth = Start,
            Months = 1
        });

        result[0].HousingCost.Should().Be(3_200m);
        result[0].TotalExpenses.Should().Be(3_700m);
        result[0].Surplus.Should().Be(4_300m);
    }

    private static Expense EndsAtPurchase(string name, decimal amount) => new()
    {
        Name = name,
        Amount = amount,
        Frequency = ExpenseFrequency.Monthly,
        IsVariable = false,
        EndsAtPurchase = true
    };

    [Fact]
    public void PlannedHousing_AddsHousingFromStartMonthOnward()
    {
        var result = _service.Project(new CashFlowProjectionInput
        {
            IncomeSources = new[] { MonthlyIncome(8_000m) },
            Expenses = new[] { Fixed("Car", 500m) },
            StartMonth = Start,
            Months = 4,
            PlannedHousing = new PlannedHousing
            {
                MonthlyCost = 3_200m,
                StartMonth = Start.AddMonths(2) // March 2026
            }
        });

        // Before purchase: no housing cost layered in.
        result[0].HousingCost.Should().Be(0m);
        result[1].HousingCost.Should().Be(0m);
        result[0].TotalExpenses.Should().Be(500m);

        // From the purchase month onward: housing cost appears.
        result[2].HousingCost.Should().Be(3_200m);
        result[3].HousingCost.Should().Be(3_200m);
        result[2].TotalExpenses.Should().Be(3_700m);
    }

    [Fact]
    public void PlannedHousing_StopsEndsAtPurchaseExpensesAtStartMonth()
    {
        var result = _service.Project(new CashFlowProjectionInput
        {
            IncomeSources = new[] { MonthlyIncome(8_000m) },
            Expenses = new[] { EndsAtPurchase("Rent", 2_000m), Fixed("Car", 500m) },
            StartMonth = Start,
            Months = 3,
            PlannedHousing = new PlannedHousing
            {
                MonthlyCost = 3_200m,
                StartMonth = Start.AddMonths(1) // February 2026
            }
        });

        // January: rent still counts, no housing yet.
        result[0].FixedExpenses.Should().Be(2_500m);
        result[0].HousingCost.Should().Be(0m);
        result[0].TotalExpenses.Should().Be(2_500m);

        // February onward: rent drops off, housing kicks in.
        result[1].FixedExpenses.Should().Be(500m);
        result[1].HousingCost.Should().Be(3_200m);
        result[1].TotalExpenses.Should().Be(3_700m);
        result[2].FixedExpenses.Should().Be(500m);
        result[2].HousingCost.Should().Be(3_200m);
    }

    [Fact]
    public void PlannedHousing_Null_KeepsLegacyBehavior()
    {
        var input = new CashFlowProjectionInput
        {
            IncomeSources = new[] { MonthlyIncome(8_000m) },
            Expenses = new[] { EndsAtPurchase("Rent", 2_000m), Fixed("Car", 500m) },
            StartMonth = Start,
            Months = 3,
            PlannedHousing = null
        };

        var result = _service.Project(input);

        // No plan: the EndsAtPurchase expense keeps counting all months.
        result.Should().AllSatisfy(m =>
        {
            m.FixedExpenses.Should().Be(2_500m);
            m.HousingCost.Should().Be(0m);
            m.TotalExpenses.Should().Be(2_500m);
        });
    }

    [Fact]
    public void Project_PerformanceUnder100msFor24Months()
    {
        var input = new CashFlowProjectionInput
        {
            IncomeSources = Enumerable.Range(1, 10).Select(i => MonthlyIncome(1_000m * i)).ToList(),
            Expenses = Enumerable.Range(1, 30).Select(i => Fixed($"Expense {i}", 100m * i)).ToList(),
            OneTimeEvents = Enumerable.Range(1, 12)
                .Select(i => new OneTimeEvent { Name = $"Event {i}", Amount = 500m, Date = Start.AddMonths(i) })
                .ToList(),
            StartMonth = Start
        };

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = _service.Project(input);
        stopwatch.Stop();

        result.Should().HaveCount(24);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
    }
}
