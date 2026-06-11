using Xunit;
using FluentAssertions;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Data.Repositories;

namespace HomeBuyerHelper.Data.Tests;

/// <summary>
/// Integration tests for the SQLite repositories using a temporary database file.
/// </summary>
public sealed class RepositoryIntegrationTests : IDisposable
{
    private readonly string _dbPath;
    private readonly DatabaseService _databaseService;
    private readonly PropertyRepository _propertyRepository;
    private readonly CriteriaRepository _criteriaRepository;
    private readonly ScoreRepository _scoreRepository;
    private readonly UserPreferencesRepository _preferencesRepository;

    public RepositoryIntegrationTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"hbh_test_{Guid.NewGuid():N}.db");
        _databaseService = new DatabaseService(_dbPath);
        _propertyRepository = new PropertyRepository(_databaseService);
        _criteriaRepository = new CriteriaRepository(_databaseService);
        _scoreRepository = new ScoreRepository(_databaseService, _criteriaRepository);
        _preferencesRepository = new UserPreferencesRepository(_databaseService);
    }

    public void Dispose()
    {
        if (File.Exists(_dbPath))
        {
            File.Delete(_dbPath);
        }
    }

    [Fact]
    public async Task PropertyRepository_CreateAndGet_RoundTrips()
    {
        // Arrange
        var property = new Property
        {
            Nickname = "Blue House",
            Address = "123 Oak St",
            City = "Seattle",
            State = "WA",
            AskingPrice = 650_000m,
            Bedrooms = 4,
            Bathrooms = 2.5m,
            SquareFeet = 2200
        };

        // Act
        var id = await _propertyRepository.CreateAsync(property);
        var loaded = await _propertyRepository.GetByIdAsync(id);

        // Assert
        loaded.Should().NotBeNull();
        loaded!.Nickname.Should().Be("Blue House");
        loaded.AskingPrice.Should().Be(650_000m);
        loaded.Bathrooms.Should().Be(2.5m);
    }

    [Fact]
    public async Task PropertyRepository_GetActive_ExcludesArchived()
    {
        // Arrange
        var active = new Property { Nickname = "Active" };
        var archived = new Property { Nickname = "Archived", IsArchived = true };
        await _propertyRepository.CreateAsync(active);
        await _propertyRepository.CreateAsync(archived);

        // Act
        var result = await _propertyRepository.GetActiveAsync();

        // Assert
        result.Should().ContainSingle(p => p.Nickname == "Active");
    }

    [Fact]
    public async Task ScoreRepository_Upsert_UpdatesExistingScore()
    {
        // Arrange
        var propertyId = await _propertyRepository.CreateAsync(new Property { Nickname = "Test" });
        var criterionId = await _criteriaRepository.CreateAsync(new EvaluationCriterion
        {
            Name = "Commute",
            Weight = 20
        });

        // Act - initial score then update
        await _scoreRepository.UpsertAsync(new PropertyScore
        {
            PropertyId = propertyId,
            CriterionId = criterionId,
            Score = 4
        });
        await _scoreRepository.UpsertAsync(new PropertyScore
        {
            PropertyId = propertyId,
            CriterionId = criterionId,
            Score = 9
        });

        var scores = await _scoreRepository.GetByPropertyIdAsync(propertyId);

        // Assert
        scores.Should().ContainSingle();
        scores[0].Score.Should().Be(9);
        scores[0].Criterion.Should().NotBeNull();
        scores[0].WeightedScore.Should().Be(9 * 20);
    }

    [Fact]
    public async Task ScoreRepository_MaxPossibleScore_UsesTenPointScale()
    {
        // Arrange
        await _criteriaRepository.CreateAsync(new EvaluationCriterion { Name = "A", Weight = 60 });
        await _criteriaRepository.CreateAsync(new EvaluationCriterion { Name = "B", Weight = 40 });

        // Act
        var max = await _scoreRepository.GetMaxPossibleScoreAsync();

        // Assert - 10-point scale: a perfect 10 on every criterion hits the max
        max.Should().Be(10 * 100);
    }

    [Fact]
    public async Task ScoreRepository_PerfectScores_Reach100Percent()
    {
        // Arrange
        var propertyId = await _propertyRepository.CreateAsync(new Property { Nickname = "Perfect" });
        var criterionId = await _criteriaRepository.CreateAsync(new EvaluationCriterion
        {
            Name = "Everything",
            Weight = 100
        });
        await _scoreRepository.UpsertAsync(new PropertyScore
        {
            PropertyId = propertyId,
            CriterionId = criterionId,
            Score = 10
        });

        // Act
        var total = await _scoreRepository.GetTotalWeightedScoreAsync(propertyId);
        var max = await _scoreRepository.GetMaxPossibleScoreAsync();

        // Assert
        total.Should().Be(max);
    }

    [Fact]
    public async Task CriteriaRepository_DeleteRemovesCriterion()
    {
        // Arrange
        var id = await _criteriaRepository.CreateAsync(new EvaluationCriterion { Name = "Temp", Weight = 10 });

        // Act
        await _criteriaRepository.DeleteAsync(id);

        // Assert
        (await _criteriaRepository.GetByIdAsync(id)).Should().BeNull();
        (await _criteriaRepository.GetCountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task UserPreferencesRepository_GetCreatesDefaults_AndSavePersists()
    {
        // Act - first get creates defaults
        var preferences = await _preferencesRepository.GetAsync();

        // Assert defaults
        preferences.HasCompletedOnboarding.Should().BeFalse();
        preferences.DefaultDownPaymentPercent.Should().Be(20m);

        // Act - mutate and save
        preferences.HasCompletedOnboarding = true;
        preferences.DefaultInterestRate = 6.5m;
        await _preferencesRepository.SaveAsync(preferences);

        var reloaded = await _preferencesRepository.GetAsync();

        // Assert
        reloaded.HasCompletedOnboarding.Should().BeTrue();
        reloaded.DefaultInterestRate.Should().Be(6.5m);
    }

    [Fact]
    public async Task DatabaseService_ClearAllData_RemovesEverything()
    {
        // Arrange
        await _propertyRepository.CreateAsync(new Property { Nickname = "House" });
        await _criteriaRepository.CreateAsync(new EvaluationCriterion { Name = "C", Weight = 10 });

        // Act
        await _databaseService.ClearAllDataAsync();

        // Assert
        (await _propertyRepository.GetCountAsync()).Should().Be(0);
        (await _criteriaRepository.GetCountAsync()).Should().Be(0);
    }
}
