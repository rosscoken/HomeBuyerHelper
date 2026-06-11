using Xunit;
using FluentAssertions;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for the IncomeSource model.
/// </summary>
public class IncomeSourceModelTests
{
    [Theory]
    [InlineData(IncomeFrequency.Weekly, 1000, 4333.33)]
    [InlineData(IncomeFrequency.BiWeekly, 2000, 4333.33)]
    [InlineData(IncomeFrequency.SemiMonthly, 2500, 5000)]
    [InlineData(IncomeFrequency.Monthly, 5000, 5000)]
    [InlineData(IncomeFrequency.Quarterly, 15000, 5000)]
    [InlineData(IncomeFrequency.Annually, 60000, 5000)]
    public void MonthlyGrossIncome_CalculatesCorrectlyForAllFrequencies(
        IncomeFrequency frequency,
        decimal grossAmount,
        decimal expectedMonthly)
    {
        // Arrange
        var income = new IncomeSource
        {
            Name = "Test Income",
            GrossAmount = grossAmount,
            Frequency = frequency
        };

        // Act & Assert
        income.MonthlyGrossIncome.Should().BeApproximately(expectedMonthly, 1m);
    }

    [Fact]
    public void AnnualGrossIncome_IsMonthlyTimes12()
    {
        // Arrange
        var income = new IncomeSource
        {
            Name = "Test Income",
            GrossAmount = 5000,
            Frequency = IncomeFrequency.Monthly
        };

        // Act & Assert
        income.AnnualGrossIncome.Should().Be(60_000m);
    }
}
