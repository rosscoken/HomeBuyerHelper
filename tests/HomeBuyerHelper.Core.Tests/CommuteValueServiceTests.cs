using Xunit;
using FluentAssertions;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Core.Services;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for CommuteValueService (P3-COM-003).
/// </summary>
public class CommuteValueServiceTests
{
    private readonly CommuteValueService _service = new();

    private static UserPreferences DefaultPreferences => new()
    {
        TimeValueHourlyRate = 100m,
        WorkdaysPerMonth = 22
    };

    [Fact]
    public void MonthlyValue_MatchesSpecFormula()
    {
        // (60 min × 22 days × $100/hr) / 60 = $2,200/month
        _service.CalculateMonthlyValue(60, 22, 100m).Should().Be(2_200m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void MonthlyValue_NoCommute_IsZero(int minutes)
    {
        _service.CalculateMonthlyValue(minutes, 22, 100m).Should().Be(0);
    }

    [Fact]
    public void Analyze_RemoteWorker_HasNoCommuteData()
    {
        var property = new Property { Nickname = "Home" };

        var analysis = _service.Analyze(property, DefaultPreferences);

        analysis.HasCommuteData.Should().BeFalse();
        analysis.MonthlyValue.Should().Be(0);
        analysis.ThirtyYearValue.Should().Be(0);
    }

    [Fact]
    public void Analyze_CalculatesAllOutputMetrics()
    {
        var property = new Property
        {
            Nickname = "Suburb House",
            CommuteMinutesPrimary = 60
        };

        var analysis = _service.Analyze(property, DefaultPreferences);

        analysis.HasCommuteData.Should().BeTrue();
        analysis.MonthlyValue.Should().Be(2_200m);
        analysis.AnnualValue.Should().Be(26_400m);
        analysis.ThirtyYearValue.Should().Be(792_000m);
        // Hours/year = 60 × 22 × 12 / 60 = 264
        analysis.HoursPerYear.Should().Be(264m);
        // Days/year = 264 / 8 = 33
        analysis.DaysPerYear.Should().Be(33m);
    }

    [Fact]
    public void Analyze_SumsPrimaryAndSecondaryCommutes()
    {
        var property = new Property
        {
            Nickname = "Two Commuters",
            CommuteMinutesPrimary = 40,
            CommuteMinutesSecondary = 20
        };

        var analysis = _service.Analyze(property, DefaultPreferences);

        analysis.TotalRoundTripMinutes.Should().Be(60);
        analysis.MonthlyValue.Should().Be(2_200m);
    }

    [Theory]
    [InlineData(20, CommuteZone.Excellent)]
    [InlineData(30, CommuteZone.Moderate)]
    [InlineData(60, CommuteZone.Moderate)]
    [InlineData(61, CommuteZone.Long)]
    [InlineData(90, CommuteZone.Long)]
    [InlineData(91, CommuteZone.VeryLong)]
    public void Analyze_AssignsZonesByPrimaryCommute(int minutes, CommuteZone expected)
    {
        var property = new Property { Nickname = "Test", CommuteMinutesPrimary = minutes };

        _service.Analyze(property, DefaultPreferences).Zone.Should().Be(expected);
    }
}
