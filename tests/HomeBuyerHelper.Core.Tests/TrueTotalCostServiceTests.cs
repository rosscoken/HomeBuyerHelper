using FluentAssertions;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Core.Services;
using Xunit;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for TrueTotalCostService (P3-TTC-001).
/// </summary>
public class TrueTotalCostServiceTests
{
    private readonly TrueTotalCostService _service = new(
        new CalculationService(),
        new CommuteValueService());

    private static UserPreferences DefaultPreferences => new()
    {
        DefaultDownPaymentPercent = 20m,
        DefaultInterestRate = 7.0m,
        DefaultMortgageTerm = 30,
        DefaultPropertyTaxRate = 0.96m,
        DefaultMonthlyInsurance = 125m,
        TimeValueHourlyRate = 100m,
        WorkdaysPerMonth = 22
    };

    [Fact]
    public void Calculate_CombinesHousingUtilitiesAndCommute()
    {
        var property = new Property
        {
            Nickname = "Full Cost House",
            AskingPrice = 500_000m,
            MonthlyHOA = 100m,
            MonthlyUtilities = 250m,
            CommuteMinutesPrimary = 60
        };

        var result = _service.Calculate(property, DefaultPreferences);

        result.MonthlyHousing.Should().BeGreaterThan(0);
        result.MonthlyUtilities.Should().Be(250m);
        result.MonthlyCommuteValue.Should().Be(2_200m);
        result.MonthlyTotal.Should().Be(
            result.MonthlyHousing + result.MonthlyUtilities + result.MonthlyCommuteValue);
        result.ThirtyYearTotal.Should().Be(result.MonthlyTotal * 360);
    }

    [Fact]
    public void Calculate_HandlesMissingComponentsGracefully()
    {
        var property = new Property { Nickname = "Bare Minimum" };

        var result = _service.Calculate(property, DefaultPreferences);

        result.MonthlyHousing.Should().Be(0);
        result.MonthlyUtilities.Should().Be(0);
        result.MonthlyCommuteValue.Should().Be(0);
        result.MonthlyTotal.Should().Be(0);
    }

    [Fact]
    public void Calculate_RemoteWorkerOnlyPaysHousing()
    {
        var property = new Property
        {
            Nickname = "Remote Worker House",
            AskingPrice = 400_000m
        };

        var result = _service.Calculate(property, DefaultPreferences);

        result.MonthlyHousing.Should().BeGreaterThan(0);
        result.MonthlyCommuteValue.Should().Be(0);
        result.Commute!.HasCommuteData.Should().BeFalse();
    }

    [Fact]
    public void Calculate_ShorterCommuteWinsOnTrueCostDespiteHigherPrice()
    {
        // Spec scenario: a closer, pricier home can be cheaper in true total cost.
        var cityCondo = new Property
        {
            Nickname = "City Condo",
            AskingPrice = 550_000m,
            CommuteMinutesPrimary = 20
        };
        var suburbHouse = new Property
        {
            Nickname = "Suburb House",
            AskingPrice = 500_000m,
            CommuteMinutesPrimary = 120
        };

        var condoCost = _service.Calculate(cityCondo, DefaultPreferences);
        var suburbCost = _service.Calculate(suburbHouse, DefaultPreferences);

        // Suburb saves ~$330/mo on housing but loses 100 min/day = ~$3,667/mo of time.
        condoCost.MonthlyTotal.Should().BeLessThan(suburbCost.MonthlyTotal);
    }
}
