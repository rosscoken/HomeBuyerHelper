using FluentAssertions;
using HomeBuyerHelper.Data.Entities;

namespace HomeBuyerHelper.Data.Tests;

/// <summary>
/// Tests for Data layer entity models.
/// </summary>
public class PropertyEntityTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var entity = new PropertyEntity();

        // Assert
        entity.Id.Should().Be(0);
        entity.Nickname.Should().Be(string.Empty);
        entity.Address.Should().BeNull();
        entity.City.Should().BeNull();
        entity.State.Should().BeNull();
        entity.ZipCode.Should().BeNull();
        entity.AskingPrice.Should().Be(0);
        entity.OfferPrice.Should().BeNull();
        entity.Bedrooms.Should().Be(0);
        entity.Bathrooms.Should().Be(0);
        entity.SquareFeet.Should().Be(0);
        entity.LotSquareFeet.Should().BeNull();
        entity.YearBuilt.Should().BeNull();
        entity.PropertyType.Should().Be(0);
        entity.MonthlyHOA.Should().Be(0);
        entity.AnnualPropertyTax.Should().BeNull();
        entity.AnnualInsurance.Should().BeNull();
        entity.ListingUrl.Should().BeNull();
        entity.Notes.Should().BeNull();
        entity.IsFavorite.Should().BeFalse();
        entity.IsArchived.Should().BeFalse();
    }

    [Fact]
    public void AllProperties_CanBeSet()
    {
        // Arrange & Act
        var now = DateTime.UtcNow;
        var entity = new PropertyEntity
        {
            Id = 1,
            Nickname = "Dream House",
            Address = "123 Main St",
            City = "Portland",
            State = "OR",
            ZipCode = "97201",
            AskingPrice = 500_000m,
            OfferPrice = 480_000m,
            Bedrooms = 4,
            Bathrooms = 2.5m,
            SquareFeet = 2200,
            LotSquareFeet = 7500,
            YearBuilt = 1985,
            PropertyType = 1,
            MonthlyHOA = 150m,
            AnnualPropertyTax = 6000m,
            AnnualInsurance = 1800m,
            ListingUrl = "https://example.com",
            Notes = "Test notes",
            IsFavorite = true,
            IsArchived = false,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Assert
        entity.Id.Should().Be(1);
        entity.Nickname.Should().Be("Dream House");
        entity.Address.Should().Be("123 Main St");
        entity.City.Should().Be("Portland");
        entity.State.Should().Be("OR");
        entity.ZipCode.Should().Be("97201");
        entity.AskingPrice.Should().Be(500_000m);
        entity.OfferPrice.Should().Be(480_000m);
        entity.Bedrooms.Should().Be(4);
        entity.Bathrooms.Should().Be(2.5m);
        entity.SquareFeet.Should().Be(2200);
        entity.LotSquareFeet.Should().Be(7500);
        entity.YearBuilt.Should().Be(1985);
        entity.PropertyType.Should().Be(1);
        entity.MonthlyHOA.Should().Be(150m);
        entity.AnnualPropertyTax.Should().Be(6000m);
        entity.AnnualInsurance.Should().Be(1800m);
        entity.ListingUrl.Should().Be("https://example.com");
        entity.Notes.Should().Be("Test notes");
        entity.IsFavorite.Should().BeTrue();
        entity.IsArchived.Should().BeFalse();
        entity.CreatedAt.Should().Be(now);
        entity.UpdatedAt.Should().Be(now);
    }
}

/// <summary>
/// Tests for EvaluationCriterionEntity.
/// </summary>
public class EvaluationCriterionEntityTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var entity = new EvaluationCriterionEntity();

        // Assert
        entity.Id.Should().Be(0);
        entity.Name.Should().Be(string.Empty);
        entity.Description.Should().BeNull();
        entity.Weight.Should().Be(0);
        entity.Category.Should().Be(0);
        entity.IsSystemSuggested.Should().BeFalse();
        entity.DisplayOrder.Should().Be(0);
    }

    [Fact]
    public void AllProperties_CanBeSet()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // Act
        var entity = new EvaluationCriterionEntity
        {
            Id = 1,
            Name = "Kitchen Quality",
            Description = "Rate the kitchen condition",
            Weight = 15,
            Category = 1,
            IsSystemSuggested = true,
            DisplayOrder = 5,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Assert
        entity.Id.Should().Be(1);
        entity.Name.Should().Be("Kitchen Quality");
        entity.Description.Should().Be("Rate the kitchen condition");
        entity.Weight.Should().Be(15);
        entity.Category.Should().Be(1);
        entity.IsSystemSuggested.Should().BeTrue();
        entity.DisplayOrder.Should().Be(5);
        entity.CreatedAt.Should().Be(now);
        entity.UpdatedAt.Should().Be(now);
    }
}

/// <summary>
/// Tests for PropertyScoreEntity.
/// </summary>
public class PropertyScoreEntityTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var entity = new PropertyScoreEntity();

        // Assert
        entity.Id.Should().Be(0);
        entity.PropertyId.Should().Be(0);
        entity.CriterionId.Should().Be(0);
        entity.Score.Should().Be(0);
        entity.Notes.Should().BeNull();
    }

    [Fact]
    public void AllProperties_CanBeSet()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // Act
        var entity = new PropertyScoreEntity
        {
            Id = 1,
            PropertyId = 100,
            CriterionId = 50,
            Score = 8,
            Notes = "Great condition",
            CreatedAt = now,
            UpdatedAt = now
        };

        // Assert
        entity.Id.Should().Be(1);
        entity.PropertyId.Should().Be(100);
        entity.CriterionId.Should().Be(50);
        entity.Score.Should().Be(8);
        entity.Notes.Should().Be("Great condition");
        entity.CreatedAt.Should().Be(now);
        entity.UpdatedAt.Should().Be(now);
    }
}

/// <summary>
/// Tests for UserPreferencesEntity.
/// </summary>
public class UserPreferencesEntityTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var entity = new UserPreferencesEntity();

        // Assert
        entity.Id.Should().Be(0);
        entity.HasCompletedOnboarding.Should().BeFalse();
    }

    [Fact]
    public void AllProperties_CanBeSet()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // Act
        var entity = new UserPreferencesEntity
        {
            Id = 1,
            HasCompletedOnboarding = true,
            BuyingGoal = 2,
            PropertyCountRange = 1,
            HouseholdSize = 3,
            HasChildren = true,
            HasPets = false,
            WorkArrangement = 1,
            Currency = "USD",
            UseDarkMode = true,
            EnableNotifications = false,
            DefaultDownPaymentPercent = 25m,
            DefaultInterestRate = 6.5m,
            DefaultMortgageTerm = 30,
            DefaultPropertyTaxRate = 1.2m,
            DefaultMonthlyInsurance = 150m,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Assert
        entity.Id.Should().Be(1);
        entity.HasCompletedOnboarding.Should().BeTrue();
        entity.BuyingGoal.Should().Be(2);
        entity.PropertyCountRange.Should().Be(1);
        entity.HouseholdSize.Should().Be(3);
        entity.HasChildren.Should().BeTrue();
        entity.HasPets.Should().BeFalse();
        entity.WorkArrangement.Should().Be(1);
        entity.Currency.Should().Be("USD");
        entity.UseDarkMode.Should().BeTrue();
        entity.EnableNotifications.Should().BeFalse();
        entity.DefaultDownPaymentPercent.Should().Be(25m);
        entity.DefaultInterestRate.Should().Be(6.5m);
        entity.DefaultMortgageTerm.Should().Be(30);
        entity.DefaultPropertyTaxRate.Should().Be(1.2m);
        entity.DefaultMonthlyInsurance.Should().Be(150m);
        entity.CreatedAt.Should().Be(now);
        entity.UpdatedAt.Should().Be(now);
    }
}

/// <summary>
/// Tests for IncomeSourceEntity.
/// </summary>
public class IncomeSourceEntityTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var entity = new IncomeSourceEntity();

        // Assert
        entity.Id.Should().Be(0);
        entity.Name.Should().Be(string.Empty);
        entity.GrossAmount.Should().Be(0);
        entity.Frequency.Should().Be(0);
    }

    [Fact]
    public void AllProperties_CanBeSet()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // Act
        var entity = new IncomeSourceEntity
        {
            Id = 1,
            Name = "Salary",
            GrossAmount = 5000m,
            Frequency = 4, // Monthly
            CreatedAt = now,
            UpdatedAt = now
        };

        // Assert
        entity.Id.Should().Be(1);
        entity.Name.Should().Be("Salary");
        entity.GrossAmount.Should().Be(5000m);
        entity.Frequency.Should().Be(4);
        entity.CreatedAt.Should().Be(now);
        entity.UpdatedAt.Should().Be(now);
    }
}

/// <summary>
/// Tests for ExpenseEntity.
/// </summary>
public class ExpenseEntityTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var entity = new ExpenseEntity();

        // Assert
        entity.Id.Should().Be(0);
        entity.Name.Should().Be(string.Empty);
        entity.Amount.Should().Be(0);
    }

    [Fact]
    public void AllProperties_CanBeSet()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // Act
        var entity = new ExpenseEntity
        {
            Id = 1,
            Name = "Car Payment",
            Amount = 450m,
            Category = 1,
            IsRequired = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        // Assert
        entity.Id.Should().Be(1);
        entity.Name.Should().Be("Car Payment");
        entity.Amount.Should().Be(450m);
        entity.Category.Should().Be(1);
        entity.IsRequired.Should().BeTrue();
        entity.CreatedAt.Should().Be(now);
        entity.UpdatedAt.Should().Be(now);
    }
}

/// <summary>
/// Tests for FundingSourceEntity.
/// </summary>
public class FundingSourceEntityTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var entity = new FundingSourceEntity();

        // Assert
        entity.Id.Should().Be(0);
        entity.Name.Should().Be(string.Empty);
        entity.Amount.Should().Be(0);
    }

    [Fact]
    public void AllProperties_CanBeSet()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // Act
        var entity = new FundingSourceEntity
        {
            Id = 1,
            Name = "Savings Account",
            Amount = 50000m,
            SourceType = 1,
            IsLiquid = true,
            Notes = "Primary savings",
            CreatedAt = now,
            UpdatedAt = now
        };

        // Assert
        entity.Id.Should().Be(1);
        entity.Name.Should().Be("Savings Account");
        entity.Amount.Should().Be(50000m);
        entity.SourceType.Should().Be(1);
        entity.IsLiquid.Should().BeTrue();
        entity.Notes.Should().Be("Primary savings");
        entity.CreatedAt.Should().Be(now);
        entity.UpdatedAt.Should().Be(now);
    }
}
