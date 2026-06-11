using Xunit;
using FluentAssertions;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Core.Services;
using NSubstitute;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for the ExportService (PDF generation, JSON backup/restore).
/// </summary>
public class ExportServiceTests : IDisposable
{
    private readonly IPropertyRepository _propertyRepository = Substitute.For<IPropertyRepository>();
    private readonly IScoreRepository _scoreRepository = Substitute.For<IScoreRepository>();
    private readonly ICriteriaRepository _criteriaRepository = Substitute.For<ICriteriaRepository>();
    private readonly IUserPreferencesRepository _preferencesRepository = Substitute.For<IUserPreferencesRepository>();
    private readonly IIncomeRepository _incomeRepository = Substitute.For<IIncomeRepository>();
    private readonly IExpenseRepository _expenseRepository = Substitute.For<IExpenseRepository>();
    private readonly IOneTimeEventRepository _oneTimeEventRepository = Substitute.For<IOneTimeEventRepository>();
    private readonly ExportService _service;
    private readonly List<string> _createdFiles = new();

    public ExportServiceTests()
    {
        var incomeScenarioService = new IncomeScenarioService();
        _incomeRepository.GetAllAsync().Returns(new List<IncomeSource>());
        _expenseRepository.GetAllAsync().Returns(new List<Expense>());
        _oneTimeEventRepository.GetAllAsync().Returns(new List<OneTimeEvent>());

        _service = new ExportService(
            _propertyRepository,
            _scoreRepository,
            _criteriaRepository,
            _preferencesRepository,
            new CalculationService(),
            _incomeRepository,
            _expenseRepository,
            _oneTimeEventRepository,
            new CashFlowProjectionService(incomeScenarioService),
            new AffordabilityService(incomeScenarioService));
    }

    public void Dispose()
    {
        foreach (var file in _createdFiles.Where(File.Exists))
        {
            File.Delete(file);
        }
        GC.SuppressFinalize(this);
    }

    private static EvaluationCriterion MakeCriterion(int id, string name, int weight) => new()
    {
        Id = id,
        Name = name,
        Weight = weight
    };

    private static Property MakeProperty(int id, string nickname, decimal price) => new()
    {
        Id = id,
        Nickname = nickname,
        AskingPrice = price,
        Bedrooms = 3,
        Bathrooms = 2,
        SquareFeet = 1800,
        YearBuilt = 1995,
        MonthlyHOA = 50
    };

    private static PropertyScore MakeScore(int propertyId, EvaluationCriterion criterion, int score) => new()
    {
        PropertyId = propertyId,
        CriterionId = criterion.Id,
        Score = score,
        Criterion = criterion
    };

    private void SetupTwoPropertyComparison()
    {
        var commute = MakeCriterion(1, "Commute Time", 8);
        var kitchen = MakeCriterion(2, "Kitchen Quality", 5);

        _criteriaRepository.GetAllAsync().Returns(new List<EvaluationCriterion> { commute, kitchen });
        _scoreRepository.GetMaxPossibleScoreAsync().Returns(10 * (8 + 5));

        var first = MakeProperty(1, "Blue House", 450_000m);
        var second = MakeProperty(2, "Brick Townhome", 410_000m);

        _propertyRepository.GetByIdAsync(1).Returns(first);
        _propertyRepository.GetByIdAsync(2).Returns(second);

        _scoreRepository.GetByPropertyIdAsync(1).Returns(new List<PropertyScore>
        {
            MakeScore(1, commute, 9),
            MakeScore(1, kitchen, 4)
        });
        _scoreRepository.GetByPropertyIdAsync(2).Returns(new List<PropertyScore>
        {
            MakeScore(2, commute, 6),
            MakeScore(2, kitchen, 8)
        });
    }

    [Fact]
    public async Task ExportComparisonToPdf_GeneratesPdfFile()
    {
        // Arrange
        SetupTwoPropertyComparison();

        // Act
        var filePath = await _service.ExportComparisonToPdfAsync(new[] { 1, 2 });
        _createdFiles.Add(filePath);

        // Assert
        File.Exists(filePath).Should().BeTrue();
        var bytes = await File.ReadAllBytesAsync(filePath);
        bytes.Length.Should().BeGreaterThan(500);
        // PDF magic header "%PDF"
        bytes.Take(4).Should().Equal((byte)'%', (byte)'P', (byte)'D', (byte)'F');
    }

    [Fact]
    public async Task ExportComparisonToPdf_SkipsMissingProperties()
    {
        // Arrange
        SetupTwoPropertyComparison();
        _propertyRepository.GetByIdAsync(99).Returns((Property?)null);

        // Act
        var filePath = await _service.ExportComparisonToPdfAsync(new[] { 1, 99 });
        _createdFiles.Add(filePath);

        // Assert
        File.Exists(filePath).Should().BeTrue();
    }

    [Fact]
    public async Task ExportPropertyDetailToPdf_GeneratesPdfFile()
    {
        // Arrange
        SetupTwoPropertyComparison();

        // Act
        var filePath = await _service.ExportPropertyDetailToPdfAsync(1);
        _createdFiles.Add(filePath);

        // Assert
        File.Exists(filePath).Should().BeTrue();
        var bytes = await File.ReadAllBytesAsync(filePath);
        bytes.Take(4).Should().Equal((byte)'%', (byte)'P', (byte)'D', (byte)'F');
    }

    [Fact]
    public async Task ExportPropertyDetailToPdf_UnknownProperty_Throws()
    {
        // Arrange
        _propertyRepository.GetByIdAsync(42).Returns((Property?)null);

        // Act
        var act = () => _service.ExportPropertyDetailToPdfAsync(42);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ExportBudgetToPdf_GeneratesPdfFile()
    {
        // Arrange
        SetupTwoPropertyComparison();
        _preferencesRepository.GetAsync().Returns(new UserPreferences());

        // Act
        var filePath = await _service.ExportBudgetToPdfAsync(1);
        _createdFiles.Add(filePath);

        // Assert
        File.Exists(filePath).Should().BeTrue();
        var bytes = await File.ReadAllBytesAsync(filePath);
        bytes.Take(4).Should().Equal((byte)'%', (byte)'P', (byte)'D', (byte)'F');
    }

    [Fact]
    public async Task ExportAllData_ThenValidate_RoundTrips()
    {
        // Arrange
        SetupTwoPropertyComparison();
        _propertyRepository.GetAllAsync().Returns(new List<Property>
        {
            MakeProperty(1, "Blue House", 450_000m)
        });
        _preferencesRepository.GetAsync().Returns(new UserPreferences());

        // Act
        var filePath = await _service.ExportAllDataAsync();
        _createdFiles.Add(filePath);
        var json = await File.ReadAllTextAsync(filePath);
        var validation = await _service.ValidateImportFileAsync(json);

        // Assert
        validation.IsValid.Should().BeTrue();
        validation.PropertyCount.Should().Be(1);
        validation.CriteriaCount.Should().Be(2);
        validation.ScoreCount.Should().Be(2);
        validation.HasSettings.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateImportFile_EmptyContent_IsInvalid()
    {
        // Act
        var result = await _service.ValidateImportFileAsync("");

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ValidateImportFile_MalformedJson_IsInvalid()
    {
        // Act
        var result = await _service.ValidateImportFileAsync("{ not valid json");

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task ExportComparisonToCsv_WritesMatrixWithTotals()
    {
        // Arrange
        SetupTwoPropertyComparison();

        // Act
        var filePath = await _service.ExportComparisonToCsvAsync(new[] { 1, 2 });
        _createdFiles.Add(filePath);

        // Assert
        var lines = await File.ReadAllLinesAsync(filePath);
        lines[0].Should().Be("Criterion,Weight %,Blue House,Brick Townhome");
        lines.Should().Contain(line => line.StartsWith("Commute Time,8,9,6"));
        lines.Should().Contain(line => line.StartsWith("Total Weighted Score"));
    }

    [Fact]
    public async Task ExportCashFlowToCsv_WritesProjection()
    {
        // Arrange
        _preferencesRepository.GetAsync().Returns(new UserPreferences());
        _incomeRepository.GetAllAsync().Returns(new List<IncomeSource>
        {
            new() { Name = "Salary", GrossAmount = 5_000m, Frequency = IncomeFrequency.Monthly, IsReliable = true }
        });

        // Act
        var filePath = await _service.ExportCashFlowToCsvAsync();
        _createdFiles.Add(filePath);

        // Assert
        var lines = await File.ReadAllLinesAsync(filePath);
        lines[0].Should().StartWith("Month,Income,Fixed Expenses");
        lines.Should().HaveCount(25); // header + 24 months
        lines[1].Should().Contain("5000.00");
    }

    [Fact]
    public async Task ExportShareableHtml_HonorsPrivacyOptions()
    {
        // Arrange
        SetupTwoPropertyComparison();
        var first = MakeProperty(1, "Blue House", 450_000m);
        first.Notes = "Secret negotiation strategy";
        _propertyRepository.GetActiveAsync().Returns(new List<Property>
        {
            first,
            MakeProperty(2, "Brick Townhome", 410_000m)
        });

        // Act - exclude prices and notes
        var filePath = await _service.ExportShareableHtmlAsync(new ShareReportOptions
        {
            IncludePrices = false,
            IncludeScores = true,
            IncludeNotes = false
        });
        _createdFiles.Add(filePath);

        // Assert
        var html = await File.ReadAllTextAsync(filePath);
        html.Should().Contain("Blue House");
        html.Should().Contain("Scores by Criterion");
        html.Should().NotContain("450,000");
        html.Should().NotContain("Secret negotiation strategy");
    }

    [Fact]
    public async Task ExportShareableHtml_EscapesHtmlInUserContent()
    {
        // Arrange
        SetupTwoPropertyComparison();
        var sneaky = MakeProperty(1, "<script>alert(1)</script>", 450_000m);
        _propertyRepository.GetActiveAsync().Returns(new List<Property> { sneaky });

        // Act
        var filePath = await _service.ExportShareableHtmlAsync(new ShareReportOptions());
        _createdFiles.Add(filePath);

        // Assert - injection-safe share files (P4-QG-002)
        var html = await File.ReadAllTextAsync(filePath);
        html.Should().NotContain("<script>alert(1)</script>");
        html.Should().Contain("&lt;script&gt;");
    }

    [Fact]
    public async Task ImportFromJson_EmptyContent_ReturnsFalse()
    {
        // Act
        var result = await _service.ImportFromJsonAsync("");

        // Assert
        result.Should().BeFalse();
    }
}
