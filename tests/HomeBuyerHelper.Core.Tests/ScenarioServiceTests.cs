using Xunit;
using FluentAssertions;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Core.Services;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for ScenarioService (P5-SCN-001/003).
/// </summary>
public class ScenarioServiceTests
{
    private readonly ScenarioService _service;

    public ScenarioServiceTests()
    {
        var incomeService = new IncomeScenarioService();
        _service = new ScenarioService(
            new CalculationService(),
            incomeService,
            new AffordabilityService(incomeService));
    }

    private static IncomeSource[] Income(decimal monthly) => new[]
    {
        new IncomeSource
        {
            Name = "Salary",
            GrossAmount = monthly,
            Frequency = IncomeFrequency.Monthly,
            IsReliable = true
        }
    };

    [Fact]
    public void Evaluate_CalculatesCoreMetrics()
    {
        var scenario = new PurchaseScenario
        {
            Name = "Base Case",
            PurchasePrice = 500_000m,
            DownPaymentPercent = 20m,
            InterestRate = 7.0m,
            MortgageTermYears = 30
        };

        var result = _service.Evaluate(scenario, Income(12_000m));

        result.DownPayment.Should().Be(100_000m);
        result.LoanAmount.Should().Be(400_000m);
        result.MonthlyPayment.Should().BeApproximately(2_661m, 2m);
        result.TotalInterest.Should().BeGreaterThan(0);
        result.CashToClose.Should().BeGreaterThan(result.DownPayment);
        result.AffordabilityZone.Should().Be(AffordabilityZone.Comfortable); // ~22%
    }

    [Fact]
    public void Evaluate_LowerRate_ReducesMonthlyPayment()
    {
        var highRate = new PurchaseScenario { Name = "High", PurchasePrice = 500_000m, InterestRate = 7.5m };
        var lowRate = new PurchaseScenario { Name = "Low", PurchasePrice = 500_000m, InterestRate = 5.5m };

        var income = Income(12_000m);
        _service.Evaluate(lowRate, income).MonthlyPayment
            .Should().BeLessThan(_service.Evaluate(highRate, income).MonthlyPayment);
    }

    [Fact]
    public void Evaluate_AdditionalDownPayment_ReducesLoan()
    {
        var scenario = new PurchaseScenario
        {
            Name = "Extra Down",
            PurchasePrice = 500_000m,
            DownPaymentPercent = 20m,
            AdditionalDownPayment = 50_000m
        };

        var result = _service.Evaluate(scenario, Income(12_000m));

        result.DownPayment.Should().Be(150_000m);
        result.LoanAmount.Should().Be(350_000m);
    }

    [Fact]
    public void BuildWaitScenario_AdjustsPriceSavingsAndSunkRent()
    {
        var baseScenario = new PurchaseScenario
        {
            Name = "Base",
            PurchasePrice = 500_000m,
            DownPaymentPercent = 20m,
            InterestRate = 7m
        };

        var wait = _service.BuildWaitScenario(
            baseScenario,
            monthsToWait: 6,
            monthlySavings: 2_000m,
            annualMarketChangePercent: 4m,
            currentMonthlyRent: 2_500m);

        // Price grows ~2% over 6 months at 4%/yr.
        wait.PurchasePrice.Should().BeApproximately(509_902m, 50m);
        wait.AdditionalDownPayment.Should().Be(12_000m);
        wait.SunkCosts.Should().Be(15_000m);
        wait.Name.Should().Contain("+6mo");
    }

    [Fact]
    public void WaitScenario_TotalCostIncludesSunkRent()
    {
        var baseScenario = new PurchaseScenario
        {
            Name = "Base",
            PurchasePrice = 500_000m
        };
        var wait = _service.BuildWaitScenario(baseScenario, 6, 0m, 0m, 2_500m);

        var baseResult = _service.Evaluate(baseScenario, Income(12_000m));
        var waitResult = _service.Evaluate(wait, Income(12_000m));

        // Same price/loan, but waiting costs 15k of rent.
        waitResult.TotalCostIncludingSunk.Should().BeApproximately(
            baseResult.TotalCostIncludingSunk + 15_000m, 1m);
    }
}
