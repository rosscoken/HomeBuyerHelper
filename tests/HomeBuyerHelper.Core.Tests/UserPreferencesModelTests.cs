using FluentAssertions;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for the UserPreferences model.
/// </summary>
public class UserPreferencesModelTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var prefs = new UserPreferences();

        // Assert
        prefs.Id.Should().Be(0);
        prefs.HasCompletedOnboarding.Should().BeFalse();
        prefs.BuyingGoal.Should().Be(BuyingGoal.Exploring);
        prefs.PropertyCountRange.Should().Be(PropertyCountRange.TwoToFive);
        prefs.HouseholdSize.Should().Be(2);
        prefs.HasChildren.Should().BeFalse();
        prefs.HasPets.Should().BeFalse();
        prefs.WorkArrangement.Should().Be(WorkArrangement.Hybrid);
        prefs.PrioritizesLocation.Should().BeFalse();
        prefs.PrioritizesSize.Should().BeFalse();
        prefs.PrioritizesCondition.Should().BeFalse();
        prefs.PrioritizesPrice.Should().BeFalse();
        prefs.Currency.Should().Be("USD");
        prefs.UseDarkMode.Should().BeFalse();
        prefs.EnableNotifications.Should().BeTrue();
        prefs.DefaultDownPaymentPercent.Should().Be(20m);
        prefs.DefaultInterestRate.Should().Be(7.0m);
        prefs.DefaultMortgageTerm.Should().Be(30);
        prefs.DefaultPropertyTaxRate.Should().Be(0.96m);
        prefs.DefaultMonthlyInsurance.Should().Be(125m);
        prefs.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        prefs.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(BuyingGoal.Exploring)]
    [InlineData(BuyingGoal.WithinYear)]
    [InlineData(BuyingGoal.ActivelySearching)]
    [InlineData(BuyingGoal.MadeOffer)]
    [InlineData(BuyingGoal.UnderContract)]
    public void BuyingGoal_AcceptsAllValidValues(BuyingGoal goal)
    {
        // Arrange & Act
        var prefs = new UserPreferences { BuyingGoal = goal };

        // Assert
        prefs.BuyingGoal.Should().Be(goal);
    }

    [Theory]
    [InlineData(PropertyCountRange.One)]
    [InlineData(PropertyCountRange.TwoToFive)]
    [InlineData(PropertyCountRange.MoreThanFive)]
    public void PropertyCountRange_AcceptsAllValidValues(PropertyCountRange range)
    {
        // Arrange & Act
        var prefs = new UserPreferences { PropertyCountRange = range };

        // Assert
        prefs.PropertyCountRange.Should().Be(range);
    }

    [Theory]
    [InlineData(WorkArrangement.FullyRemote)]
    [InlineData(WorkArrangement.Hybrid)]
    [InlineData(WorkArrangement.FullyOnsite)]
    [InlineData(WorkArrangement.Retired)]
    [InlineData(WorkArrangement.Other)]
    public void WorkArrangement_AcceptsAllValidValues(WorkArrangement arrangement)
    {
        // Arrange & Act
        var prefs = new UserPreferences { WorkArrangement = arrangement };

        // Assert
        prefs.WorkArrangement.Should().Be(arrangement);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    public void HouseholdSize_AcceptsValidValues(int size)
    {
        // Arrange & Act
        var prefs = new UserPreferences { HouseholdSize = size };

        // Assert
        prefs.HouseholdSize.Should().Be(size);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    public void DefaultDownPaymentPercent_AcceptsValidValues(decimal percent)
    {
        // Arrange & Act
        var prefs = new UserPreferences { DefaultDownPaymentPercent = percent };

        // Assert
        prefs.DefaultDownPaymentPercent.Should().Be(percent);
    }

    [Theory]
    [InlineData(3.5)]
    [InlineData(5.0)]
    [InlineData(7.0)]
    [InlineData(10.0)]
    public void DefaultInterestRate_AcceptsValidValues(decimal rate)
    {
        // Arrange & Act
        var prefs = new UserPreferences { DefaultInterestRate = rate };

        // Assert
        prefs.DefaultInterestRate.Should().Be(rate);
    }

    [Theory]
    [InlineData(15)]
    [InlineData(20)]
    [InlineData(30)]
    public void DefaultMortgageTerm_AcceptsValidValues(int term)
    {
        // Arrange & Act
        var prefs = new UserPreferences { DefaultMortgageTerm = term };

        // Assert
        prefs.DefaultMortgageTerm.Should().Be(term);
    }

    [Fact]
    public void Priorities_CanBeSet()
    {
        // Arrange & Act
        var prefs = new UserPreferences
        {
            PrioritizesLocation = true,
            PrioritizesSize = true,
            PrioritizesCondition = false,
            PrioritizesPrice = true
        };

        // Assert
        prefs.PrioritizesLocation.Should().BeTrue();
        prefs.PrioritizesSize.Should().BeTrue();
        prefs.PrioritizesCondition.Should().BeFalse();
        prefs.PrioritizesPrice.Should().BeTrue();
    }

    [Fact]
    public void Currency_CanBeChanged()
    {
        // Arrange & Act
        var prefs = new UserPreferences { Currency = "EUR" };

        // Assert
        prefs.Currency.Should().Be("EUR");
    }
}

/// <summary>
/// Tests for buying-related enums.
/// </summary>
public class BuyingEnumTests
{
    [Fact]
    public void BuyingGoal_HasExpectedValues()
    {
        Enum.GetValues<BuyingGoal>().Should().HaveCount(5);
    }

    [Fact]
    public void PropertyCountRange_HasExpectedValues()
    {
        Enum.GetValues<PropertyCountRange>().Should().HaveCount(3);
    }

    [Fact]
    public void WorkArrangement_HasExpectedValues()
    {
        Enum.GetValues<WorkArrangement>().Should().HaveCount(5);
    }
}
