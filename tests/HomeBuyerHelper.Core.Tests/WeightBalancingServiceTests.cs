using FluentAssertions;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Core.Services;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for the WeightBalancingService.
/// </summary>
public class WeightBalancingServiceTests
{
    private readonly WeightBalancingService _sut;

    public WeightBalancingServiceTests()
    {
        _sut = new WeightBalancingService();
    }

    #region Rebalance Tests

    [Fact]
    public void Rebalance_EmptyList_ReturnsSuccess()
    {
        // Arrange
        var criteria = new List<EvaluationCriterion>();

        // Act
        var result = _sut.Rebalance(criteria);

        // Assert
        result.Success.Should().BeTrue();
        result.Warnings.Should().BeEmpty();
    }

    [Fact]
    public void Rebalance_AlreadyBalanced_ReturnsSuccessWithNoChanges()
    {
        // Arrange
        var criteria = new List<EvaluationCriterion>
        {
            new() { Name = "A", Weight = 50 },
            new() { Name = "B", Weight = 50 }
        };

        // Act
        var result = _sut.Rebalance(criteria);

        // Assert
        result.Success.Should().BeTrue();
        criteria[0].Weight.Should().Be(50);
        criteria[1].Weight.Should().Be(50);
    }

    [Fact]
    public void Rebalance_UnbalancedWeights_NormalizesTo100()
    {
        // Arrange
        var criteria = new List<EvaluationCriterion>
        {
            new() { Name = "A", Weight = 10 },
            new() { Name = "B", Weight = 20 },
            new() { Name = "C", Weight = 30 }
        };

        // Act
        var result = _sut.Rebalance(criteria);

        // Assert
        result.Success.Should().BeTrue();
        criteria.Sum(c => c.Weight).Should().Be(100);
    }

    [Fact]
    public void Rebalance_WithLockedWeights_PreservesLockedValues()
    {
        // Arrange
        var criteria = new List<EvaluationCriterion>
        {
            new() { Name = "Locked", Weight = 30, IsWeightLocked = true },
            new() { Name = "A", Weight = 20 },
            new() { Name = "B", Weight = 10 }
        };

        // Act
        var result = _sut.Rebalance(criteria);

        // Assert
        result.Success.Should().BeTrue();
        criteria[0].Weight.Should().Be(30); // Locked, unchanged
        criteria.Sum(c => c.Weight).Should().Be(100);
    }

    [Fact]
    public void Rebalance_AllLocked_ReturnsFailure()
    {
        // Arrange
        var criteria = new List<EvaluationCriterion>
        {
            new() { Name = "A", Weight = 30, IsWeightLocked = true },
            new() { Name = "B", Weight = 30, IsWeightLocked = true }
        };

        // Act
        var result = _sut.Rebalance(criteria);

        // Assert
        result.Success.Should().BeFalse();
        result.Warnings.Should().Contain(w => w.Contains("All criteria are locked"));
    }

    [Fact]
    public void Rebalance_LockedWeightsExceed100_ReturnsFailure()
    {
        // Arrange
        var criteria = new List<EvaluationCriterion>
        {
            new() { Name = "A", Weight = 60, IsWeightLocked = true },
            new() { Name = "B", Weight = 50, IsWeightLocked = true },
            new() { Name = "C", Weight = 10 }
        };

        // Act
        var result = _sut.Rebalance(criteria);

        // Assert
        result.Success.Should().BeFalse();
        result.Warnings.Should().Contain(w => w.Contains("Locked weights exceed 100%"));
    }

    [Fact]
    public void Rebalance_HighWeight_AddsWarning()
    {
        // Arrange
        var criteria = new List<EvaluationCriterion>
        {
            new() { Name = "High", Weight = 50 },
            new() { Name = "Low", Weight = 50 }
        };

        // Act
        var result = _sut.Rebalance(criteria);

        // Assert
        result.Warnings.Should().Contain(w => w.Contains("High") && w.Contains("exceeds"));
    }

    [Fact]
    public void Rebalance_UnlockedWithZeroWeight_DistributesEqually()
    {
        // Arrange
        var criteria = new List<EvaluationCriterion>
        {
            new() { Name = "Locked", Weight = 40, IsWeightLocked = true },
            new() { Name = "A", Weight = 0 },
            new() { Name = "B", Weight = 0 }
        };

        // Act
        var result = _sut.Rebalance(criteria);

        // Assert
        result.Success.Should().BeTrue();
        criteria.Sum(c => c.Weight).Should().Be(100);
        // The remaining 60% should be distributed among A and B
        (criteria[1].Weight + criteria[2].Weight).Should().Be(60);
    }

    #endregion

    #region NormalizeToPercent Tests

    [Fact]
    public void NormalizeToPercent_EmptyList_DoesNothing()
    {
        // Arrange
        var criteria = new List<EvaluationCriterion>();

        // Act
        _sut.NormalizeToPercent(criteria);

        // Assert
        criteria.Should().BeEmpty();
    }

    [Fact]
    public void NormalizeToPercent_ZeroTotalWeight_DistributesEqually()
    {
        // Arrange
        var criteria = new List<EvaluationCriterion>
        {
            new() { Name = "A", Weight = 0 },
            new() { Name = "B", Weight = 0 },
            new() { Name = "C", Weight = 0 },
            new() { Name = "D", Weight = 0 }
        };

        // Act
        _sut.NormalizeToPercent(criteria);

        // Assert
        criteria.Sum(c => c.Weight).Should().Be(100);
        criteria.All(c => c.Weight >= 24 && c.Weight <= 26).Should().BeTrue();
    }

    [Fact]
    public void NormalizeToPercent_ProportionallyScales()
    {
        // Arrange
        var criteria = new List<EvaluationCriterion>
        {
            new() { Name = "A", Weight = 1 },
            new() { Name = "B", Weight = 2 },
            new() { Name = "C", Weight = 2 }
        };

        // Act
        _sut.NormalizeToPercent(criteria);

        // Assert
        criteria.Sum(c => c.Weight).Should().Be(100);
        // A should be ~20%, B and C should be ~40% each
        criteria[0].Weight.Should().BeInRange(18, 22);
        criteria[1].Weight.Should().BeInRange(38, 42);
    }

    [Fact]
    public void NormalizeToPercent_EnsuresMinimumWeightOf1()
    {
        // Arrange
        var criteria = new List<EvaluationCriterion>
        {
            new() { Name = "Large", Weight = 100 },
            new() { Name = "Tiny", Weight = 1 }
        };

        // Act
        _sut.NormalizeToPercent(criteria);

        // Assert
        criteria.Sum(c => c.Weight).Should().Be(100);
        criteria.All(c => c.Weight >= 1).Should().BeTrue();
    }

    [Fact]
    public void NormalizeToPercent_SingleCriterion_SetsTo100()
    {
        // Arrange
        var criteria = new List<EvaluationCriterion>
        {
            new() { Name = "Only", Weight = 50 }
        };

        // Act
        _sut.NormalizeToPercent(criteria);

        // Assert
        criteria[0].Weight.Should().Be(100);
    }

    #endregion

    #region ApplyRankingWeights Tests

    [Fact]
    public void ApplyRankingWeights_EmptyList_DoesNothing()
    {
        // Arrange
        var criteria = new List<EvaluationCriterion>();

        // Act
        _sut.ApplyRankingWeights(criteria);

        // Assert
        criteria.Should().BeEmpty();
    }

    [Fact]
    public void ApplyRankingWeights_AppliesDescendingWeights()
    {
        // Arrange
        var criteria = new List<EvaluationCriterion>
        {
            new() { Name = "First", Weight = 0 },
            new() { Name = "Second", Weight = 0 },
            new() { Name = "Third", Weight = 0 }
        };

        // Act
        _sut.ApplyRankingWeights(criteria);

        // Assert
        criteria[0].Weight.Should().BeGreaterThan(criteria[1].Weight);
        criteria[1].Weight.Should().BeGreaterThan(criteria[2].Weight);
        criteria.Sum(c => c.Weight).Should().Be(100);
    }

    [Fact]
    public void ApplyRankingWeights_FirstCriterionGetsHighestWeight()
    {
        // Arrange
        var criteria = new List<EvaluationCriterion>
        {
            new() { Name = "Top Priority", Weight = 0 },
            new() { Name = "Other", Weight = 0 }
        };

        // Act
        _sut.ApplyRankingWeights(criteria);

        // Assert
        // First criterion should get the highest weight (~25% before normalization)
        criteria[0].Weight.Should().BeGreaterThan(criteria[1].Weight);
    }

    [Fact]
    public void ApplyRankingWeights_ManyCriteria_AllGetAtLeast1()
    {
        // Arrange - More criteria than predefined weights
        var criteria = Enumerable.Range(1, 15)
            .Select(i => new EvaluationCriterion { Name = $"C{i}", Weight = 0 })
            .ToList();

        // Act
        _sut.ApplyRankingWeights(criteria);

        // Assert
        criteria.All(c => c.Weight >= 1).Should().BeTrue();
        criteria.Sum(c => c.Weight).Should().Be(100);
    }

    [Fact]
    public void ApplyRankingWeights_SingleCriterion_GetsFullWeight()
    {
        // Arrange
        var criteria = new List<EvaluationCriterion>
        {
            new() { Name = "Only", Weight = 0 }
        };

        // Act
        _sut.ApplyRankingWeights(criteria);

        // Assert
        criteria[0].Weight.Should().Be(100);
    }

    #endregion

    #region WarningThreshold Tests

    [Fact]
    public void WarningThreshold_Is40Percent()
    {
        WeightBalancingService.WarningThreshold.Should().Be(40);
    }

    #endregion
}
