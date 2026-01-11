using FluentAssertions;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for the EvaluationCriterion model.
/// </summary>
public class EvaluationCriterionModelTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var criterion = new EvaluationCriterion { Name = "Test Criterion" };

        // Assert
        criterion.Id.Should().Be(0);
        criterion.Weight.Should().Be(5); // Default weight
        criterion.Category.Should().Be(CriterionCategory.Other); // Default category
        criterion.IsSystemSuggested.Should().BeFalse();
        criterion.DisplayOrder.Should().Be(0);
        criterion.IsWeightLocked.Should().BeFalse();
        criterion.Description.Should().BeNull();
        criterion.ScoreAnchorLow.Should().BeNull();
        criterion.ScoreAnchorMidLow.Should().BeNull();
        criterion.ScoreAnchorMid.Should().BeNull();
        criterion.ScoreAnchorMidHigh.Should().BeNull();
        criterion.ScoreAnchorHigh.Should().BeNull();
        criterion.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        criterion.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Name_IsRequired()
    {
        // Arrange & Act
        var criterion = new EvaluationCriterion { Name = "Kitchen Quality" };

        // Assert
        criterion.Name.Should().Be("Kitchen Quality");
    }

    [Theory]
    [InlineData(CriterionCategory.Location)]
    [InlineData(CriterionCategory.Interior)]
    [InlineData(CriterionCategory.Exterior)]
    [InlineData(CriterionCategory.Neighborhood)]
    [InlineData(CriterionCategory.Financial)]
    [InlineData(CriterionCategory.Lifestyle)]
    [InlineData(CriterionCategory.Other)]
    public void Category_AcceptsAllValidValues(CriterionCategory category)
    {
        // Arrange & Act
        var criterion = new EvaluationCriterion
        {
            Name = "Test",
            Category = category
        };

        // Assert
        criterion.Category.Should().Be(category);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public void Weight_AcceptsValidValues(int weight)
    {
        // Arrange & Act
        var criterion = new EvaluationCriterion
        {
            Name = "Test",
            Weight = weight
        };

        // Assert
        criterion.Weight.Should().Be(weight);
    }

    [Fact]
    public void ScoreAnchors_CanBeSet()
    {
        // Arrange & Act
        var criterion = new EvaluationCriterion
        {
            Name = "Kitchen Quality",
            ScoreAnchorLow = "Outdated, needs complete renovation",
            ScoreAnchorMidLow = "Functional but dated",
            ScoreAnchorMid = "Average condition, some updates needed",
            ScoreAnchorMidHigh = "Good condition, modern features",
            ScoreAnchorHigh = "Dream kitchen, fully updated"
        };

        // Assert
        criterion.ScoreAnchorLow.Should().Be("Outdated, needs complete renovation");
        criterion.ScoreAnchorMidLow.Should().Be("Functional but dated");
        criterion.ScoreAnchorMid.Should().Be("Average condition, some updates needed");
        criterion.ScoreAnchorMidHigh.Should().Be("Good condition, modern features");
        criterion.ScoreAnchorHigh.Should().Be("Dream kitchen, fully updated");
    }

    [Fact]
    public void IsWeightLocked_CanBeToggled()
    {
        // Arrange
        var criterion = new EvaluationCriterion { Name = "Test" };

        // Act
        criterion.IsWeightLocked = true;

        // Assert
        criterion.IsWeightLocked.Should().BeTrue();
    }

    [Fact]
    public void IsSystemSuggested_CanBeSet()
    {
        // Arrange & Act
        var criterion = new EvaluationCriterion
        {
            Name = "Short Commute",
            IsSystemSuggested = true
        };

        // Assert
        criterion.IsSystemSuggested.Should().BeTrue();
    }

    [Fact]
    public void DisplayOrder_CanBeSet()
    {
        // Arrange & Act
        var criterion = new EvaluationCriterion
        {
            Name = "Test",
            DisplayOrder = 5
        };

        // Assert
        criterion.DisplayOrder.Should().Be(5);
    }

    [Fact]
    public void Description_CanBeSet()
    {
        // Arrange & Act
        var criterion = new EvaluationCriterion
        {
            Name = "School Quality",
            Description = "Rate the quality of nearby schools for all grade levels"
        };

        // Assert
        criterion.Description.Should().Be("Rate the quality of nearby schools for all grade levels");
    }
}

/// <summary>
/// Tests for the CriterionCategory enum.
/// </summary>
public class CriterionCategoryTests
{
    [Fact]
    public void AllCategories_HaveExpectedValues()
    {
        // Assert
        Enum.GetValues<CriterionCategory>().Should().HaveCount(7);
        Enum.IsDefined(CriterionCategory.Location).Should().BeTrue();
        Enum.IsDefined(CriterionCategory.Interior).Should().BeTrue();
        Enum.IsDefined(CriterionCategory.Exterior).Should().BeTrue();
        Enum.IsDefined(CriterionCategory.Neighborhood).Should().BeTrue();
        Enum.IsDefined(CriterionCategory.Financial).Should().BeTrue();
        Enum.IsDefined(CriterionCategory.Lifestyle).Should().BeTrue();
        Enum.IsDefined(CriterionCategory.Other).Should().BeTrue();
    }
}
