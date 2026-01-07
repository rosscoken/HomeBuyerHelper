using FluentAssertions;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for the Property model.
/// </summary>
public class PropertyModelTests
{
    [Fact]
    public void EffectivePrice_WhenOfferPriceSet_ReturnsOfferPrice()
    {
        // Arrange
        var property = new Property
        {
            Nickname = "Test House",
            AskingPrice = 500_000m,
            OfferPrice = 480_000m
        };

        // Act & Assert
        property.EffectivePrice.Should().Be(480_000m);
    }

    [Fact]
    public void EffectivePrice_WhenOfferPriceNull_ReturnsAskingPrice()
    {
        // Arrange
        var property = new Property
        {
            Nickname = "Test House",
            AskingPrice = 500_000m,
            OfferPrice = null
        };

        // Act & Assert
        property.EffectivePrice.Should().Be(500_000m);
    }

    [Fact]
    public void PricePerSquareFoot_CalculatesCorrectly()
    {
        // Arrange
        var property = new Property
        {
            Nickname = "Test House",
            AskingPrice = 400_000m,
            SquareFeet = 2000
        };

        // Act & Assert
        property.PricePerSquareFoot.Should().Be(200m);
    }

    [Fact]
    public void PricePerSquareFoot_WhenZeroSquareFeet_ReturnsZero()
    {
        // Arrange
        var property = new Property
        {
            Nickname = "Test House",
            AskingPrice = 400_000m,
            SquareFeet = 0
        };

        // Act & Assert
        property.PricePerSquareFoot.Should().Be(0);
    }

    [Fact]
    public void MonthlyPropertyTax_CalculatesFromAnnual()
    {
        // Arrange
        var property = new Property
        {
            Nickname = "Test House",
            AnnualPropertyTax = 6_000m
        };

        // Act & Assert
        property.MonthlyPropertyTax.Should().Be(500m);
    }

    [Fact]
    public void MonthlyInsurance_CalculatesFromAnnual()
    {
        // Arrange
        var property = new Property
        {
            Nickname = "Test House",
            AnnualInsurance = 2_400m
        };

        // Act & Assert
        property.MonthlyInsurance.Should().Be(200m);
    }

    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var property = new Property
        {
            Nickname = "Test House"
        };

        // Assert
        property.PropertyType.Should().Be(PropertyType.SingleFamily);
        property.IsFavorite.Should().BeFalse();
        property.IsArchived.Should().BeFalse();
        property.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
