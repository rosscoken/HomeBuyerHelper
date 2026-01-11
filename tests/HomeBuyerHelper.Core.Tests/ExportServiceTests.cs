using FluentAssertions;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Core.Services;
using NSubstitute;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for the ExportService.
/// </summary>
public class ExportServiceTests
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IScoreRepository _scoreRepository;
    private readonly ICriteriaRepository _criteriaRepository;
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly ExportService _sut;

    public ExportServiceTests()
    {
        _propertyRepository = Substitute.For<IPropertyRepository>();
        _scoreRepository = Substitute.For<IScoreRepository>();
        _criteriaRepository = Substitute.For<ICriteriaRepository>();
        _preferencesRepository = Substitute.For<IUserPreferencesRepository>();
        _sut = new ExportService(_propertyRepository, _scoreRepository, _criteriaRepository, _preferencesRepository);
    }

    #region ValidateImportFileAsync Tests

    [Fact]
    public async Task ValidateImportFileAsync_EmptyContent_ReturnsInvalid()
    {
        // Arrange & Act
        var result = await _sut.ValidateImportFileAsync(string.Empty);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("empty");
    }

    [Fact]
    public async Task ValidateImportFileAsync_NullContent_ReturnsInvalid()
    {
        // Arrange & Act
        var result = await _sut.ValidateImportFileAsync(null!);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateImportFileAsync_WhitespaceContent_ReturnsInvalid()
    {
        // Arrange & Act
        var result = await _sut.ValidateImportFileAsync("   ");

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("empty");
    }

    [Fact]
    public async Task ValidateImportFileAsync_InvalidJson_ReturnsInvalid()
    {
        // Arrange
        var invalidJson = "{ invalid json }}}";

        // Act
        var result = await _sut.ValidateImportFileAsync(invalidJson);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Error reading file");
    }

    [Fact]
    public async Task ValidateImportFileAsync_ValidJson_ReturnsValid()
    {
        // Arrange
        var validJson = """
            {
                "exportDate": "2024-01-15T10:30:00Z",
                "version": "1.0",
                "properties": [],
                "criteria": [],
                "scores": []
            }
            """;

        // Act
        var result = await _sut.ValidateImportFileAsync(validJson);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Version.Should().Be("1.0");
        result.PropertyCount.Should().Be(0);
        result.CriteriaCount.Should().Be(0);
        result.ScoreCount.Should().Be(0);
    }

    [Fact]
    public async Task ValidateImportFileAsync_WithData_ReturnsCorrectCounts()
    {
        // Arrange
        var jsonWithData = """
            {
                "exportDate": "2024-01-15T10:30:00Z",
                "version": "1.0",
                "properties": [
                    {"id": 1, "nickname": "House A"},
                    {"id": 2, "nickname": "House B"}
                ],
                "criteria": [
                    {"id": 1, "name": "Kitchen"}
                ],
                "scores": [
                    {"id": 1, "propertyId": 1, "criterionId": 1, "score": 8}
                ]
            }
            """;

        // Act
        var result = await _sut.ValidateImportFileAsync(jsonWithData);

        // Assert
        result.IsValid.Should().BeTrue();
        result.PropertyCount.Should().Be(2);
        result.CriteriaCount.Should().Be(1);
        result.ScoreCount.Should().Be(1);
    }

    [Fact]
    public async Task ValidateImportFileAsync_WithSettings_SetsHasSettings()
    {
        // Arrange
        var jsonWithSettings = """
            {
                "exportDate": "2024-01-15T10:30:00Z",
                "version": "1.0",
                "properties": [],
                "criteria": [],
                "scores": [],
                "settings": {
                    "id": 1,
                    "hasCompletedOnboarding": true
                }
            }
            """;

        // Act
        var result = await _sut.ValidateImportFileAsync(jsonWithSettings);

        // Assert
        result.IsValid.Should().BeTrue();
        result.HasSettings.Should().BeTrue();
    }

    #endregion

    #region ImportFromJsonAsync Tests

    [Fact]
    public async Task ImportFromJsonAsync_EmptyContent_ReturnsFalse()
    {
        // Arrange & Act
        var result = await _sut.ImportFromJsonAsync(string.Empty);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ImportFromJsonAsync_InvalidJson_ReturnsFalse()
    {
        // Arrange
        var invalidJson = "not valid json";

        // Act
        var result = await _sut.ImportFromJsonAsync(invalidJson);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ImportFromJsonAsync_ValidJson_ImportsData()
    {
        // Arrange
        var validJson = """
            {
                "exportDate": "2024-01-15T10:30:00Z",
                "version": "1.0",
                "properties": [
                    {"id": 1, "nickname": "Test House"}
                ],
                "criteria": [
                    {"id": 1, "name": "Kitchen"}
                ],
                "scores": []
            }
            """;

        _criteriaRepository.CreateAsync(Arg.Any<EvaluationCriterion>()).Returns(100);
        _propertyRepository.CreateAsync(Arg.Any<Property>()).Returns(200);

        // Act
        var result = await _sut.ImportFromJsonAsync(validJson);

        // Assert
        result.Should().BeTrue();
        await _criteriaRepository.Received(1).CreateAsync(Arg.Any<EvaluationCriterion>());
        await _propertyRepository.Received(1).CreateAsync(Arg.Any<Property>());
    }

    [Fact]
    public async Task ImportFromJsonAsync_WithReplaceExisting_ClearsExistingData()
    {
        // Arrange
        var existingProperties = new List<Property>
        {
            new() { Id = 1, Nickname = "Existing" }
        };
        var existingCriteria = new List<EvaluationCriterion>
        {
            new() { Id = 1, Name = "Existing Criterion" }
        };

        _propertyRepository.GetAllAsync().Returns(existingProperties);
        _criteriaRepository.GetAllAsync().Returns(existingCriteria);

        var validJson = """
            {
                "exportDate": "2024-01-15T10:30:00Z",
                "version": "1.0",
                "properties": [],
                "criteria": [],
                "scores": []
            }
            """;

        // Act
        var result = await _sut.ImportFromJsonAsync(validJson, replaceExisting: true);

        // Assert
        result.Should().BeTrue();
        await _scoreRepository.Received().DeleteByPropertyIdAsync(1);
        await _propertyRepository.Received().DeleteAsync(1);
        await _criteriaRepository.Received().DeleteAsync(1);
    }

    [Fact]
    public async Task ImportFromJsonAsync_WithScores_MapsIdsCorrectly()
    {
        // Arrange
        var validJson = """
            {
                "exportDate": "2024-01-15T10:30:00Z",
                "version": "1.0",
                "properties": [
                    {"id": 1, "nickname": "Test House"}
                ],
                "criteria": [
                    {"id": 1, "name": "Kitchen"}
                ],
                "scores": [
                    {"id": 1, "propertyId": 1, "criterionId": 1, "score": 8}
                ]
            }
            """;

        _criteriaRepository.CreateAsync(Arg.Any<EvaluationCriterion>()).Returns(100);
        _propertyRepository.CreateAsync(Arg.Any<Property>()).Returns(200);
        _scoreRepository.UpsertAsync(Arg.Any<PropertyScore>()).Returns(300);

        // Act
        var result = await _sut.ImportFromJsonAsync(validJson);

        // Assert
        result.Should().BeTrue();
        await _scoreRepository.Received(1).UpsertAsync(Arg.Is<PropertyScore>(s =>
            s.PropertyId == 200 && s.CriterionId == 100));
    }

    #endregion

    #region GetAvailableFormats Tests

    [Fact]
    public void GetAvailableFormats_ReturnsExpectedFormats()
    {
        // Act
        var formats = _sut.GetAvailableFormats();

        // Assert
        formats.Should().HaveCount(3);
        formats.Should().Contain(f => f.Name == "PDF" && f.IsSupported);
        formats.Should().Contain(f => f.Name == "JSON" && f.IsSupported);
        formats.Should().Contain(f => f.Name == "CSV" && !f.IsSupported);
    }

    [Fact]
    public void GetAvailableFormats_ReturnsCorrectExtensions()
    {
        // Act
        var formats = _sut.GetAvailableFormats();

        // Assert
        formats.Should().Contain(f => f.Extension == ".pdf");
        formats.Should().Contain(f => f.Extension == ".json");
        formats.Should().Contain(f => f.Extension == ".csv");
    }

    [Fact]
    public void GetAvailableFormats_ReturnsCorrectMimeTypes()
    {
        // Act
        var formats = _sut.GetAvailableFormats();

        // Assert
        formats.Should().Contain(f => f.MimeType == "application/pdf");
        formats.Should().Contain(f => f.MimeType == "application/json");
        formats.Should().Contain(f => f.MimeType == "text/csv");
    }

    #endregion

    #region GenerateShareTextAsync Tests

    [Fact]
    public async Task GenerateShareTextAsync_PropertyNotFound_ReturnsEmpty()
    {
        // Arrange
        _propertyRepository.GetByIdAsync(999).Returns((Property?)null);

        // Act
        var result = await _sut.GenerateShareTextAsync(999);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GenerateShareTextAsync_PropertyFound_ReturnsFormattedText()
    {
        // Arrange
        var property = new Property
        {
            Id = 1,
            Nickname = "Dream House",
            Address = "123 Main St",
            City = "Springfield",
            State = "IL",
            ZipCode = "62701",
            AskingPrice = 350000,
            Bedrooms = 3,
            Bathrooms = 2,
            SquareFeet = 1800
        };

        _propertyRepository.GetByIdAsync(1).Returns(property);
        _scoreRepository.GetByPropertyIdAsync(1).Returns(new List<PropertyScore>());
        _scoreRepository.GetMaxPossibleScoreAsync().Returns(100);

        // Act
        var result = await _sut.GenerateShareTextAsync(1);

        // Assert
        result.Should().Contain("Dream House");
        result.Should().Contain("123 Main St");
        result.Should().Contain("Springfield");
        result.Should().Contain("3 bed");
        result.Should().Contain("2 bath");
        result.Should().Contain("HomeBuyerHelper");
    }

    #endregion
}
