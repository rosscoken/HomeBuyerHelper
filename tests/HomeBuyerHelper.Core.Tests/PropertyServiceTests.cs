using FluentAssertions;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Core.Services;
using NSubstitute;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for the PropertyService.
/// </summary>
public class PropertyServiceTests
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IScoreRepository _scoreRepository;
    private readonly ICriteriaRepository _criteriaRepository;
    private readonly PropertyService _sut;

    public PropertyServiceTests()
    {
        _propertyRepository = Substitute.For<IPropertyRepository>();
        _scoreRepository = Substitute.For<IScoreRepository>();
        _criteriaRepository = Substitute.For<ICriteriaRepository>();
        _sut = new PropertyService(_propertyRepository, _scoreRepository, _criteriaRepository);
    }

    #region GetPropertiesWithScoresAsync Tests

    [Fact]
    public async Task GetPropertiesWithScoresAsync_NoProperties_ReturnsEmptyList()
    {
        // Arrange
        _propertyRepository.GetActiveAsync().Returns(new List<Property>());
        _criteriaRepository.GetAllAsync().Returns(new List<EvaluationCriterion>());

        // Act
        var result = await _sut.GetPropertiesWithScoresAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPropertiesWithScoresAsync_WithProperties_CalculatesScores()
    {
        // Arrange
        var properties = new List<Property>
        {
            new() { Id = 1, Nickname = "Property A" },
            new() { Id = 2, Nickname = "Property B" }
        };

        var criteria = new List<EvaluationCriterion>
        {
            new() { Id = 1, Name = "Criterion 1", Weight = 50 },
            new() { Id = 2, Name = "Criterion 2", Weight = 50 }
        };

        var scoresProperty1 = new List<PropertyScore>
        {
            new() { PropertyId = 1, CriterionId = 1, Score = 8, Criterion = criteria[0] },
            new() { PropertyId = 1, CriterionId = 2, Score = 6, Criterion = criteria[1] }
        };

        var scoresProperty2 = new List<PropertyScore>
        {
            new() { PropertyId = 2, CriterionId = 1, Score = 5, Criterion = criteria[0] },
            new() { PropertyId = 2, CriterionId = 2, Score = 7, Criterion = criteria[1] }
        };

        _propertyRepository.GetActiveAsync().Returns(properties);
        _criteriaRepository.GetAllAsync().Returns(criteria);
        _scoreRepository.GetByPropertyIdAsync(1).Returns(scoresProperty1);
        _scoreRepository.GetByPropertyIdAsync(2).Returns(scoresProperty2);

        // Act
        var result = await _sut.GetPropertiesWithScoresAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Rank.Should().Be(1); // Higher score
        result[1].Rank.Should().Be(2);
    }

    [Fact]
    public async Task GetPropertiesWithScoresAsync_AssignsRanksCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new() { Id = 1, Nickname = "Low Scorer" },
            new() { Id = 2, Nickname = "High Scorer" },
            new() { Id = 3, Nickname = "Medium Scorer" }
        };

        var criteria = new List<EvaluationCriterion>
        {
            new() { Id = 1, Name = "Criterion", Weight = 100 }
        };

        _propertyRepository.GetActiveAsync().Returns(properties);
        _criteriaRepository.GetAllAsync().Returns(criteria);
        _scoreRepository.GetByPropertyIdAsync(1).Returns(new List<PropertyScore>
        {
            new() { PropertyId = 1, CriterionId = 1, Score = 3, Criterion = criteria[0] }
        });
        _scoreRepository.GetByPropertyIdAsync(2).Returns(new List<PropertyScore>
        {
            new() { PropertyId = 2, CriterionId = 1, Score = 9, Criterion = criteria[0] }
        });
        _scoreRepository.GetByPropertyIdAsync(3).Returns(new List<PropertyScore>
        {
            new() { PropertyId = 3, CriterionId = 1, Score = 6, Criterion = criteria[0] }
        });

        // Act
        var result = await _sut.GetPropertiesWithScoresAsync();

        // Assert
        var highScorer = result.First(p => p.Nickname == "High Scorer");
        var mediumScorer = result.First(p => p.Nickname == "Medium Scorer");
        var lowScorer = result.First(p => p.Nickname == "Low Scorer");

        highScorer.Rank.Should().Be(1);
        mediumScorer.Rank.Should().Be(2);
        lowScorer.Rank.Should().Be(3);
    }

    #endregion

    #region GetPropertyDetailAsync Tests

    [Fact]
    public async Task GetPropertyDetailAsync_PropertyNotFound_ReturnsNull()
    {
        // Arrange
        _propertyRepository.GetByIdAsync(999).Returns((Property?)null);

        // Act
        var result = await _sut.GetPropertyDetailAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPropertyDetailAsync_PropertyFound_ReturnsWithScores()
    {
        // Arrange
        var property = new Property { Id = 1, Nickname = "Test House" };
        var criteria = new List<EvaluationCriterion>
        {
            new() { Id = 1, Name = "Criterion", Weight = 100 }
        };
        var scores = new List<PropertyScore>
        {
            new() { PropertyId = 1, CriterionId = 1, Score = 8, Criterion = criteria[0] }
        };

        _propertyRepository.GetByIdAsync(1).Returns(property);
        _criteriaRepository.GetAllAsync().Returns(criteria);
        _scoreRepository.GetByPropertyIdAsync(1).Returns(scores);
        _propertyRepository.GetActiveAsync().Returns(new List<Property> { property });
        _scoreRepository.GetMaxPossibleScoreAsync().Returns(1000);
        _scoreRepository.GetTotalWeightedScoreAsync(1).Returns(800);

        // Act
        var result = await _sut.GetPropertyDetailAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Nickname.Should().Be("Test House");
        result.Scores.Should().HaveCount(1);
        result.ScoredCriteriaCount.Should().Be(1);
        result.TotalCriteriaCount.Should().Be(1);
    }

    #endregion

    #region CreatePropertyAsync Tests

    [Fact]
    public async Task CreatePropertyAsync_SetsTimestamps()
    {
        // Arrange
        var property = new Property { Nickname = "New House" };
        _propertyRepository.CreateAsync(Arg.Any<Property>()).Returns(1);

        // Act
        var result = await _sut.CreatePropertyAsync(property);

        // Assert
        result.Id.Should().Be(1);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task CreatePropertyAsync_CallsRepository()
    {
        // Arrange
        var property = new Property { Nickname = "New House" };
        _propertyRepository.CreateAsync(Arg.Any<Property>()).Returns(42);

        // Act
        await _sut.CreatePropertyAsync(property);

        // Assert
        await _propertyRepository.Received(1).CreateAsync(Arg.Is<Property>(p => p.Nickname == "New House"));
    }

    #endregion

    #region UpdatePropertyAsync Tests

    [Fact]
    public async Task UpdatePropertyAsync_UpdatesTimestamp()
    {
        // Arrange
        var property = new Property { Id = 1, Nickname = "Updated House" };
        var originalUpdatedAt = property.UpdatedAt;

        // Act
        await _sut.UpdatePropertyAsync(property);

        // Assert
        property.UpdatedAt.Should().BeAfter(originalUpdatedAt.AddMilliseconds(-100));
        await _propertyRepository.Received(1).UpdateAsync(property);
    }

    #endregion

    #region ArchivePropertyAsync Tests

    [Fact]
    public async Task ArchivePropertyAsync_PropertyExists_SetsIsArchivedTrue()
    {
        // Arrange
        var property = new Property { Id = 1, Nickname = "House to Archive", IsArchived = false };
        _propertyRepository.GetByIdAsync(1).Returns(property);

        // Act
        await _sut.ArchivePropertyAsync(1);

        // Assert
        property.IsArchived.Should().BeTrue();
        await _propertyRepository.Received(1).UpdateAsync(property);
    }

    [Fact]
    public async Task ArchivePropertyAsync_PropertyNotFound_DoesNothing()
    {
        // Arrange
        _propertyRepository.GetByIdAsync(999).Returns((Property?)null);

        // Act
        await _sut.ArchivePropertyAsync(999);

        // Assert
        await _propertyRepository.DidNotReceive().UpdateAsync(Arg.Any<Property>());
    }

    #endregion

    #region DeletePropertyAsync Tests

    [Fact]
    public async Task DeletePropertyAsync_DeletesScoresFirst()
    {
        // Arrange & Act
        await _sut.DeletePropertyAsync(1);

        // Assert
        Received.InOrder(() =>
        {
            _scoreRepository.DeleteByPropertyIdAsync(1);
            _propertyRepository.DeleteAsync(1);
        });
    }

    #endregion

    #region ToggleFavoriteAsync Tests

    [Fact]
    public async Task ToggleFavoriteAsync_PropertyNotFavorite_SetsFavoriteTrue()
    {
        // Arrange
        var property = new Property { Id = 1, Nickname = "House", IsFavorite = false };
        _propertyRepository.GetByIdAsync(1).Returns(property);

        // Act
        await _sut.ToggleFavoriteAsync(1);

        // Assert
        property.IsFavorite.Should().BeTrue();
        await _propertyRepository.Received(1).UpdateAsync(property);
    }

    [Fact]
    public async Task ToggleFavoriteAsync_PropertyIsFavorite_SetsFavoriteFalse()
    {
        // Arrange
        var property = new Property { Id = 1, Nickname = "House", IsFavorite = true };
        _propertyRepository.GetByIdAsync(1).Returns(property);

        // Act
        await _sut.ToggleFavoriteAsync(1);

        // Assert
        property.IsFavorite.Should().BeFalse();
    }

    [Fact]
    public async Task ToggleFavoriteAsync_PropertyNotFound_DoesNothing()
    {
        // Arrange
        _propertyRepository.GetByIdAsync(999).Returns((Property?)null);

        // Act
        await _sut.ToggleFavoriteAsync(999);

        // Assert
        await _propertyRepository.DidNotReceive().UpdateAsync(Arg.Any<Property>());
    }

    #endregion

    #region ComparePropertiesAsync Tests

    [Fact]
    public async Task ComparePropertiesAsync_ReturnsComparisonResults()
    {
        // Arrange
        var property = new Property { Id = 1, Nickname = "House A" };
        var scores = new List<PropertyScore>
        {
            new() { PropertyId = 1, CriterionId = 1, Score = 8, WeightedScore = 80 }
        };

        _propertyRepository.GetByIdAsync(1).Returns(property);
        _scoreRepository.GetByPropertyIdAsync(1).Returns(scores);
        _scoreRepository.GetMaxPossibleScoreAsync().Returns(100);

        // Act
        var result = await _sut.ComparePropertiesAsync(new[] { 1 });

        // Assert
        result.Should().HaveCount(1);
        result[0].Property.Should().Be(property);
    }

    [Fact]
    public async Task ComparePropertiesAsync_InvalidPropertyId_SkipsIt()
    {
        // Arrange
        _propertyRepository.GetByIdAsync(999).Returns((Property?)null);

        // Act
        var result = await _sut.ComparePropertiesAsync(new[] { 999 });

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region GetPropertyRankingsAsync Tests

    [Fact]
    public async Task GetPropertyRankingsAsync_ReturnsOrderedByScore()
    {
        // Arrange
        var properties = new List<Property>
        {
            new() { Id = 1, Nickname = "Low" },
            new() { Id = 2, Nickname = "High" }
        };

        _propertyRepository.GetActiveAsync().Returns(properties);
        _scoreRepository.GetMaxPossibleScoreAsync().Returns(100);
        _scoreRepository.GetTotalWeightedScoreAsync(1).Returns(30);
        _scoreRepository.GetTotalWeightedScoreAsync(2).Returns(80);

        // Act
        var result = await _sut.GetPropertyRankingsAsync();

        // Assert
        result.Should().HaveCount(2);
        result[0].Property.Nickname.Should().Be("High");
        result[0].Rank.Should().Be(1);
        result[1].Property.Nickname.Should().Be("Low");
        result[1].Rank.Should().Be(2);
    }

    #endregion
}
