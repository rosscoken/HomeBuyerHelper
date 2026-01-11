using FluentAssertions;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for the PropertyScore model.
/// </summary>
public class PropertyScoreModelTests
{
    [Fact]
    public void WeightedScore_WithCriterion_ReturnsScoreTimesWeight()
    {
        // Arrange
        var criterion = new EvaluationCriterion { Name = "Test", Weight = 10 };
        var score = new PropertyScore
        {
            PropertyId = 1,
            CriterionId = 1,
            Score = 8,
            Criterion = criterion
        };

        // Act & Assert
        score.WeightedScore.Should().Be(80); // 8 * 10
    }

    [Fact]
    public void WeightedScore_WithNullCriterion_ReturnsZero()
    {
        // Arrange
        var score = new PropertyScore
        {
            PropertyId = 1,
            CriterionId = 1,
            Score = 8,
            Criterion = null
        };

        // Act & Assert
        score.WeightedScore.Should().Be(0);
    }

    [Fact]
    public void WeightedScore_WithZeroWeight_ReturnsZero()
    {
        // Arrange
        var criterion = new EvaluationCriterion { Name = "Test", Weight = 0 };
        var score = new PropertyScore
        {
            PropertyId = 1,
            CriterionId = 1,
            Score = 10,
            Criterion = criterion
        };

        // Act & Assert
        score.WeightedScore.Should().Be(0);
    }

    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(5, 5, 25)]
    [InlineData(10, 10, 100)]
    [InlineData(7, 15, 105)]
    public void WeightedScore_CalculatesCorrectly(int scoreValue, int weight, int expected)
    {
        // Arrange
        var criterion = new EvaluationCriterion { Name = "Test", Weight = weight };
        var score = new PropertyScore
        {
            PropertyId = 1,
            CriterionId = 1,
            Score = scoreValue,
            Criterion = criterion
        };

        // Act & Assert
        score.WeightedScore.Should().Be(expected);
    }

    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var score = new PropertyScore
        {
            PropertyId = 1,
            CriterionId = 1,
            Score = 5
        };

        // Assert
        score.Id.Should().Be(0);
        score.Notes.Should().BeNull();
        score.Property.Should().BeNull();
        score.Criterion.Should().BeNull();
        score.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        score.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Notes_CanBeSet()
    {
        // Arrange & Act
        var score = new PropertyScore
        {
            PropertyId = 1,
            CriterionId = 1,
            Score = 7,
            Notes = "Great kitchen, needs some updates"
        };

        // Assert
        score.Notes.Should().Be("Great kitchen, needs some updates");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Score_AcceptsValidValues(int scoreValue)
    {
        // Arrange & Act
        var score = new PropertyScore
        {
            PropertyId = 1,
            CriterionId = 1,
            Score = scoreValue
        };

        // Assert
        score.Score.Should().Be(scoreValue);
    }
}
