using FluentAssertions;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Services;
using Xunit;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for RentVsBuyService (P4-RVB-001).
/// </summary>
public class RentVsBuyServiceTests
{
    private readonly RentVsBuyService _service = new(new CalculationService());

    private static RentVsBuyInput TypicalInput => new()
    {
        PurchasePrice = 500_000m,
        DownPaymentPercent = 20m,
        InterestRate = 7.0m,
        MortgageTermYears = 30,
        MonthlyOwnershipCosts = 800m,
        CurrentMonthlyRent = 2_500m,
        AnnualRentIncreasePercent = 3m,
        InvestmentReturnPercent = 7m,
        HomeAppreciationPercent = 3m,
        HorizonYears = 10
    };

    [Fact]
    public void Compare_ReturnsOneEntryPerYear()
    {
        var result = _service.Compare(TypicalInput);

        result.Years.Should().HaveCount(10);
        result.Years[0].Year.Should().Be(1);
        result.Years[^1].Year.Should().Be(10);
    }

    [Fact]
    public void Compare_InvalidInput_ReturnsEmpty()
    {
        var result = _service.Compare(new RentVsBuyInput { PurchasePrice = 0 });

        result.Years.Should().BeEmpty();
        result.BreakevenYear.Should().BeNull();
    }

    [Fact]
    public void Compare_BuyWealthGrowsWithEquityAndAppreciation()
    {
        var result = _service.Compare(TypicalInput);

        // Equity should grow year over year (principal paydown + appreciation).
        for (var i = 1; i < result.Years.Count; i++)
        {
            result.Years[i].BuyNetWealth.Should().BeGreaterThan(result.Years[i - 1].BuyNetWealth);
        }
    }

    [Fact]
    public void Compare_RentIncreasesAnnually()
    {
        var result = _service.Compare(TypicalInput);

        for (var i = 1; i < result.Years.Count; i++)
        {
            result.Years[i].MonthlyRent.Should().BeGreaterThan(result.Years[i - 1].MonthlyRent);
        }
    }

    [Fact]
    public void Compare_CheapRentAndHighReturns_FavorsRenting()
    {
        var input = new RentVsBuyInput
        {
            PurchasePrice = 800_000m,
            DownPaymentPercent = 20m,
            InterestRate = 8m,
            MonthlyOwnershipCosts = 1_500m,
            CurrentMonthlyRent = 1_200m,      // far below ownership cost
            AnnualRentIncreasePercent = 1m,
            InvestmentReturnPercent = 10m,    // strong market
            HomeAppreciationPercent = 1m,     // weak housing market
            HorizonYears = 10
        };

        var result = _service.Compare(input);

        result.BreakevenYear.Should().BeNull();
        result.FinalAdvantage.Should().BeLessThan(0);
    }

    [Fact]
    public void Compare_ExpensiveRisingRent_FavorsBuyingQuickly()
    {
        var input = new RentVsBuyInput
        {
            PurchasePrice = 400_000m,
            DownPaymentPercent = 20m,
            InterestRate = 6m,
            MonthlyOwnershipCosts = 500m,
            CurrentMonthlyRent = 3_500m,      // rent above ownership cost
            AnnualRentIncreasePercent = 5m,
            InvestmentReturnPercent = 5m,
            HomeAppreciationPercent = 4m,
            HorizonYears = 10
        };

        var result = _service.Compare(input);

        result.BreakevenYear.Should().NotBeNull();
        result.BreakevenYear.Should().BeLessThanOrEqualTo(3);
        result.FinalAdvantage.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Compare_InitialOwnershipCost_IncludesPandIAndOtherCosts()
    {
        var result = _service.Compare(TypicalInput);

        // P&I on 400k at 7%/30yr is ~2661; plus 800 other = ~3461.
        result.InitialMonthlyOwnershipCost.Should().BeApproximately(3_461m, 5m);
    }
}
