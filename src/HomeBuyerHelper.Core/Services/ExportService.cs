using System.Text.Json;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HomeBuyerHelper.Core.Services;

/// <summary>
/// Export service for generating reports and sharing data.
/// </summary>
public class ExportService : IExportService
{
    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private static readonly string[] ComparisonSummaryHeaders = { "Rank", "Property", "Price", "Weighted Score", "Score %" };
    private static readonly string[] ScoreDetailHeaders = { "Criterion", "Score", "Weighted", "Notes" };

    private readonly IPropertyRepository _propertyRepository;
    private readonly IScoreRepository _scoreRepository;
    private readonly ICriteriaRepository _criteriaRepository;
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly ICalculationService _calculationService;

    static ExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public ExportService(
        IPropertyRepository propertyRepository,
        IScoreRepository scoreRepository,
        ICriteriaRepository criteriaRepository,
        IUserPreferencesRepository preferencesRepository,
        ICalculationService calculationService)
    {
        _propertyRepository = propertyRepository;
        _scoreRepository = scoreRepository;
        _criteriaRepository = criteriaRepository;
        _preferencesRepository = preferencesRepository;
        _calculationService = calculationService;
    }

    public async Task<string> ExportComparisonToPdfAsync(IEnumerable<int> propertyIds)
    {
        var exportDir = GetExportDirectory();
        var filePath = Path.Combine(exportDir, $"comparison_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

        var criteria = (await _criteriaRepository.GetAllAsync())
            .OrderBy(c => c.DisplayOrder)
            .ToList();

        var properties = new List<Property>();
        foreach (var id in propertyIds)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null)
                continue;

            property.Scores = (await _scoreRepository.GetByPropertyIdAsync(id)).ToList();
            properties.Add(property);
        }

        var maxPossibleScore = await _scoreRepository.GetMaxPossibleScoreAsync();

        // Rank by total weighted score (highest first)
        properties = properties
            .OrderByDescending(p => p.Scores.Sum(s => s.WeightedScore))
            .ToList();

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(36);
                page.DefaultTextStyle(style => style.FontSize(9));

                page.Header().Element(header => ComposeReportHeader(header, "Property Comparison Report"));

                page.Content().PaddingVertical(10).Column(column =>
                {
                    column.Spacing(12);

                    column.Item().Element(item => ComposeComparisonSummary(item, properties, maxPossibleScore));

                    if (criteria.Count > 0 && properties.Count > 0)
                    {
                        column.Item().Element(item => ComposeScoreMatrix(item, criteria, properties));
                    }

                    column.Item().Element(item => ComposeFinancialComparison(item, properties));
                });

                page.Footer().Element(ComposeReportFooter);
            });
        }).GeneratePdf(filePath);

        return filePath;
    }

    public async Task<string> ExportPropertyDetailToPdfAsync(int propertyId)
    {
        var exportDir = GetExportDirectory();
        var filePath = Path.Combine(exportDir, $"property_{propertyId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

        var property = await _propertyRepository.GetByIdAsync(propertyId)
            ?? throw new InvalidOperationException($"Property {propertyId} not found.");

        var scores = (await _scoreRepository.GetByPropertyIdAsync(propertyId)).ToList();
        var maxPossibleScore = await _scoreRepository.GetMaxPossibleScoreAsync();
        var totalWeightedScore = scores.Sum(s => s.WeightedScore);
        var scorePercentage = maxPossibleScore > 0
            ? (decimal)totalWeightedScore / maxPossibleScore * 100
            : 0;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(style => style.FontSize(10));

                page.Header().Element(header => ComposeReportHeader(header, "Property Evaluation Report"));

                page.Content().PaddingVertical(10).Column(column =>
                {
                    column.Spacing(12);

                    column.Item().Text(property.Nickname).FontSize(16).SemiBold();
                    column.Item().Text(FormatAddress(property)).FontColor(Colors.Grey.Darken1);

                    column.Item().Element(item => ComposePropertyFacts(item, property));

                    column.Item().Text(text =>
                    {
                        text.Span("Overall Score: ").SemiBold();
                        text.Span($"{scorePercentage:F0}% ({totalWeightedScore}/{maxPossibleScore} weighted points)");
                    });

                    if (scores.Count > 0)
                    {
                        column.Item().Element(item => ComposeScoreDetails(item, scores));
                    }

                    if (!string.IsNullOrWhiteSpace(property.Notes))
                    {
                        column.Item().Column(notes =>
                        {
                            notes.Item().Text("Notes").FontSize(12).SemiBold();
                            notes.Item().Text(property.Notes);
                        });
                    }
                });

                page.Footer().Element(ComposeReportFooter);
            });
        }).GeneratePdf(filePath);

        return filePath;
    }

    public async Task<string> ExportBudgetToPdfAsync(int propertyId)
    {
        var exportDir = GetExportDirectory();
        var filePath = Path.Combine(exportDir, $"budget_{propertyId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

        var property = await _propertyRepository.GetByIdAsync(propertyId)
            ?? throw new InvalidOperationException($"Property {propertyId} not found.");

        var preferences = await _preferencesRepository.GetAsync();

        var price = property.EffectivePrice;
        var annualPropertyTax = property.AnnualPropertyTax
            ?? price * preferences.DefaultPropertyTaxRate / 100;
        var annualInsurance = property.AnnualInsurance
            ?? preferences.DefaultMonthlyInsurance * 12;

        var breakdown = _calculationService.CalculateMonthlyHousingCost(
            price,
            preferences.DefaultDownPaymentPercent,
            preferences.DefaultInterestRate,
            preferences.DefaultMortgageTerm,
            annualPropertyTax,
            annualInsurance,
            property.MonthlyHOA);

        var downPayment = _calculationService.CalculateDownPayment(price, preferences.DefaultDownPaymentPercent);
        var loanAmount = price - downPayment;
        var monthlyPayment = _calculationService.CalculateMonthlyMortgagePayment(
            loanAmount, preferences.DefaultInterestRate, preferences.DefaultMortgageTerm);
        var closingCosts = _calculationService.CalculateClosingCosts(price, property.State);
        var totalPayments = monthlyPayment * preferences.DefaultMortgageTerm * 12;
        var totalInterest = totalPayments - loanAmount;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(style => style.FontSize(10));

                page.Header().Element(header => ComposeReportHeader(header, "Cost & Budget Report"));

                page.Content().PaddingVertical(10).Column(column =>
                {
                    column.Spacing(12);

                    column.Item().Text(property.Nickname).FontSize(16).SemiBold();
                    column.Item().Text(FormatAddress(property)).FontColor(Colors.Grey.Darken1);

                    column.Item().Element(item => ComposeKeyValueTable(item, "Loan Assumptions", new[]
                    {
                        ("Purchase Price", $"{price:C0}"),
                        ("Down Payment", $"{downPayment:C0} ({preferences.DefaultDownPaymentPercent:F0}%)"),
                        ("Loan Amount", $"{loanAmount:C0}"),
                        ("Interest Rate", $"{preferences.DefaultInterestRate:F2}%"),
                        ("Term", $"{preferences.DefaultMortgageTerm} years")
                    }));

                    column.Item().Element(item => ComposeKeyValueTable(item, "Estimated Monthly Housing Cost", new[]
                    {
                        ("Principal", $"{breakdown.Principal:C2}"),
                        ("Interest", $"{breakdown.Interest:C2}"),
                        ("Property Tax", $"{breakdown.PropertyTax:C2}"),
                        ("Insurance", $"{breakdown.Insurance:C2}"),
                        ("HOA", $"{breakdown.HOA:C2}"),
                        ("PMI", $"{breakdown.PMI:C2}"),
                        ("Total", $"{breakdown.Total:C2}")
                    }));

                    column.Item().Element(item => ComposeKeyValueTable(item, "Estimated Closing Costs", new[]
                    {
                        ("Loan Origination Fee", $"{closingCosts.LoanOriginationFee:C0}"),
                        ("Appraisal Fee", $"{closingCosts.AppraisalFee:C0}"),
                        ("Title Insurance & Search", $"{closingCosts.TitleInsurance + closingCosts.TitleSearch:C0}"),
                        ("Escrow & Recording", $"{closingCosts.EscrowFees + closingCosts.RecordingFees:C0}"),
                        ("Prepaid Interest", $"{closingCosts.PrepaidInterest:C0}"),
                        ("Inspection & Other", $"{closingCosts.HomeInspection + closingCosts.OtherFees:C0}"),
                        ("Total Estimate", $"{closingCosts.Total:C0}"),
                        ("Typical Range", $"{closingCosts.LowEstimate:C0} - {closingCosts.HighEstimate:C0}")
                    }));

                    column.Item().Element(item => ComposeKeyValueTable(item, "Lifetime Cost (Loan Term)", new[]
                    {
                        ("Total of Payments", $"{totalPayments:C0}"),
                        ("Total Interest Paid", $"{totalInterest:C0}"),
                        ("Cash Needed at Closing", $"{downPayment + closingCosts.Total:C0}")
                    }));
                });

                page.Footer().Element(ComposeReportFooter);
            });
        }).GeneratePdf(filePath);

        return filePath;
    }

    public async Task<string> ExportAllDataAsync()
    {
        var exportDir = GetExportDirectory();
        var filePath = Path.Combine(exportDir, $"homebuyerhelper_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json");

        var properties = await _propertyRepository.GetAllAsync();
        var criteria = await _criteriaRepository.GetAllAsync();
        var preferences = await _preferencesRepository.GetAsync();

        // Get all scores for all properties
        var allScores = new List<PropertyScore>();
        foreach (var property in properties)
        {
            var scores = await _scoreRepository.GetByPropertyIdAsync(property.Id);
            foreach (var score in scores)
            {
                allScores.Add(score);
            }
        }

        var exportData = new ExportDataModel
        {
            ExportDate = DateTime.UtcNow,
            Version = "1.0",
            Properties = properties.ToList(),
            Criteria = criteria.ToList(),
            Scores = allScores,
            Settings = preferences
        };

        var json = JsonSerializer.Serialize(exportData, WriteOptions);

        await File.WriteAllTextAsync(filePath, json);

        return filePath;
    }

    public async Task ImportDataAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Import file not found", filePath);
        }

        var json = await File.ReadAllTextAsync(filePath);
        await ImportFromJsonAsync(json);
    }

    public async Task<bool> ImportFromJsonAsync(string jsonContent)
    {
        return await ImportFromJsonAsync(jsonContent, replaceExisting: false);
    }

    public async Task<bool> ImportFromJsonAsync(string jsonContent, bool replaceExisting)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
                return false;

            // Validate JSON structure
            var importData = JsonSerializer.Deserialize<ExportDataModel>(jsonContent, ReadOptions);
            if (importData == null)
                return false;

            // Track ID mappings for updating score references
            var criteriaIdMap = new Dictionary<int, int>();
            var propertyIdMap = new Dictionary<int, int>();

            // Clear existing data if replacing
            if (replaceExisting)
            {
                var existingProperties = await _propertyRepository.GetAllAsync();
                foreach (var prop in existingProperties)
                {
                    await _scoreRepository.DeleteByPropertyIdAsync(prop.Id);
                    await _propertyRepository.DeleteAsync(prop.Id);
                }

                var existingCriteria = await _criteriaRepository.GetAllAsync();
                foreach (var crit in existingCriteria)
                {
                    await _criteriaRepository.DeleteAsync(crit.Id);
                }
            }

            // Import criteria first (properties/scores reference them)
            if (importData.Criteria != null)
            {
                foreach (var criterion in importData.Criteria)
                {
                    var oldId = criterion.Id;
                    criterion.Id = 0; // Reset ID for new insert
                    var newId = await _criteriaRepository.CreateAsync(criterion);
                    criteriaIdMap[oldId] = newId;
                }
            }

            // Import properties
            if (importData.Properties != null)
            {
                foreach (var property in importData.Properties)
                {
                    var oldId = property.Id;
                    property.Id = 0; // Reset ID for new insert
                    var newId = await _propertyRepository.CreateAsync(property);
                    propertyIdMap[oldId] = newId;
                }
            }

            // Import scores with updated IDs
            if (importData.Scores != null)
            {
                var validScores = importData.Scores
                    .Where(score => propertyIdMap.ContainsKey(score.PropertyId) &&
                                   criteriaIdMap.ContainsKey(score.CriterionId));

                foreach (var score in validScores)
                {
                    score.Id = 0;
                    score.PropertyId = propertyIdMap[score.PropertyId];
                    score.CriterionId = criteriaIdMap[score.CriterionId];
                    await _scoreRepository.UpsertAsync(score);
                }
            }

            // Import settings if replacing
            if (replaceExisting && importData.Settings != null)
            {
                await _preferencesRepository.SaveAsync(importData.Settings);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public Task<ImportValidationResult> ValidateImportFileAsync(string jsonContent)
    {
        var result = new ImportValidationResult();

        try
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                result.IsValid = false;
                result.ErrorMessage = "File is empty.";
                return Task.FromResult(result);
            }

            var importData = JsonSerializer.Deserialize<ExportDataModel>(jsonContent, ReadOptions);

            if (importData == null)
            {
                result.IsValid = false;
                result.ErrorMessage = "Invalid file format.";
                return Task.FromResult(result);
            }

            result.IsValid = true;
            result.Version = importData.Version ?? "Unknown";
            result.ExportDate = importData.ExportDate;
            result.PropertyCount = importData.Properties?.Count ?? 0;
            result.CriteriaCount = importData.Criteria?.Count ?? 0;
            result.ScoreCount = importData.Scores?.Count ?? 0;
            result.HasSettings = importData.Settings != null;
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.ErrorMessage = $"Error reading file: {ex.Message}";
        }

        return Task.FromResult(result);
    }

    private sealed class ExportDataModel
    {
        public DateTime ExportDate { get; set; }
        public string? Version { get; set; }
        public List<Property>? Properties { get; set; }
        public List<EvaluationCriterion>? Criteria { get; set; }
        public List<PropertyScore>? Scores { get; set; }
        public UserPreferences? Settings { get; set; }
    }

    public async Task<string> GenerateShareTextAsync(int propertyId)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId);
        if (property == null)
        {
            return string.Empty;
        }

        var scores = await _scoreRepository.GetByPropertyIdAsync(propertyId);
        var maxScore = await _scoreRepository.GetMaxPossibleScoreAsync();
        var totalScore = scores.Sum(s => s.WeightedScore);
        var percentage = maxScore > 0 ? (decimal)totalScore / maxScore * 100 : 0;

        var text = $"""
            {property.Nickname}
            {property.Address}, {property.City}, {property.State} {property.ZipCode}

            Price: {property.EffectivePrice:C0}
            {property.Bedrooms} bed / {property.Bathrooms} bath / {property.SquareFeet:N0} sqft
            Price/sqft: {property.PricePerSquareFoot:C0}

            Score: {percentage:F0}% ({totalScore}/{maxScore} points)

            Shared from HomeBuyerHelper
            """;

        return text;
    }

    public IReadOnlyList<ExportFormat> GetAvailableFormats()
    {
        return new List<ExportFormat>
        {
            new() { Name = "PDF", Extension = ".pdf", MimeType = "application/pdf", IsSupported = true },
            new() { Name = "JSON", Extension = ".json", MimeType = "application/json", IsSupported = true },
            new() { Name = "CSV", Extension = ".csv", MimeType = "text/csv", IsSupported = false }
        };
    }

    private static string GetExportDirectory()
    {
        var exportDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "HomeBuyerHelper",
            "Exports");

        if (!Directory.Exists(exportDir))
        {
            Directory.CreateDirectory(exportDir);
        }

        return exportDir;
    }

    // ---- PDF composition helpers ----

    private static void ComposeReportHeader(IContainer container, string title)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text(title).FontSize(18).SemiBold().FontColor(Colors.Indigo.Darken2);
                column.Item().Text($"Generated {DateTime.Now:MMMM d, yyyy h:mm tt}")
                    .FontSize(8).FontColor(Colors.Grey.Darken1);
            });
            row.ConstantItem(120).AlignRight().AlignMiddle()
                .Text("HomeBuyerHelper").FontSize(10).SemiBold().FontColor(Colors.Indigo.Medium);
        });
    }

    private static void ComposeReportFooter(IContainer container)
    {
        container.AlignCenter().Text(text =>
        {
            text.DefaultTextStyle(style => style.FontSize(8).FontColor(Colors.Grey.Darken1));
            text.Span("Page ");
            text.CurrentPageNumber();
            text.Span(" of ");
            text.TotalPages();
        });
    }

    private static void ComposeComparisonSummary(IContainer container, List<Property> rankedProperties, int maxPossibleScore)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(40);
                columns.RelativeColumn(3);
                columns.RelativeColumn(2);
                columns.RelativeColumn(2);
                columns.RelativeColumn(2);
            });

            table.Header(header =>
            {
                foreach (var label in ComparisonSummaryHeaders)
                {
                    header.Cell().Element(HeaderCellStyle).Text(label);
                }
            });

            for (var i = 0; i < rankedProperties.Count; i++)
            {
                var property = rankedProperties[i];
                var totalScore = property.Scores.Sum(s => s.WeightedScore);
                var percentage = maxPossibleScore > 0 ? (decimal)totalScore / maxPossibleScore * 100 : 0;
                var isTop = i == 0;

                table.Cell().Element(cell => BodyCellStyle(cell, isTop)).Text($"#{i + 1}");
                table.Cell().Element(cell => BodyCellStyle(cell, isTop)).Text(property.Nickname);
                table.Cell().Element(cell => BodyCellStyle(cell, isTop)).Text($"{property.EffectivePrice:C0}");
                table.Cell().Element(cell => BodyCellStyle(cell, isTop)).Text($"{totalScore} / {maxPossibleScore}");
                table.Cell().Element(cell => BodyCellStyle(cell, isTop)).Text($"{percentage:F0}%");
            }
        });
    }

    private static void ComposeScoreMatrix(IContainer container, List<EvaluationCriterion> criteria, List<Property> properties)
    {
        container.Column(column =>
        {
            column.Spacing(6);
            column.Item().Text("Scores by Criterion").FontSize(12).SemiBold();

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    foreach (var _ in properties)
                    {
                        columns.RelativeColumn(2);
                    }
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("Criterion (Weight)");
                    foreach (var property in properties)
                    {
                        header.Cell().Element(HeaderCellStyle).Text(property.Nickname);
                    }
                });

                foreach (var criterion in criteria)
                {
                    table.Cell().Element(cell => BodyCellStyle(cell, highlight: false))
                        .Text($"{criterion.Name} ({criterion.Weight})");

                    foreach (var property in properties)
                    {
                        var score = property.Scores.FirstOrDefault(s => s.CriterionId == criterion.Id);
                        if (score == null)
                        {
                            table.Cell().Element(cell => BodyCellStyle(cell, highlight: false))
                                .Text("-").FontColor(Colors.Grey.Medium);
                        }
                        else
                        {
                            table.Cell().Element(cell => ScoreCellStyle(cell, score.Score))
                                .Text($"{score.Score}");
                        }
                    }
                }
            });

            column.Item().Text("Color key: green = 8-10, yellow = 5-7, red = 1-4")
                .FontSize(8).FontColor(Colors.Grey.Darken1);
        });
    }

    private static void ComposeFinancialComparison(IContainer container, List<Property> properties)
    {
        container.Column(column =>
        {
            column.Spacing(6);
            column.Item().Text("Property Facts").FontSize(12).SemiBold();

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    foreach (var _ in properties)
                    {
                        columns.RelativeColumn(2);
                    }
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("");
                    foreach (var property in properties)
                    {
                        header.Cell().Element(HeaderCellStyle).Text(property.Nickname);
                    }
                });

                var rows = new (string Label, Func<Property, string> Value)[]
                {
                    ("Price", p => $"{p.EffectivePrice:C0}"),
                    ("Beds / Baths", p => $"{p.Bedrooms} / {p.Bathrooms}"),
                    ("Square Feet", p => p.SquareFeet > 0 ? $"{p.SquareFeet:N0}" : "-"),
                    ("Price / Sqft", p => p.SquareFeet > 0 ? $"{p.PricePerSquareFoot:C0}" : "-"),
                    ("Year Built", p => p.YearBuilt?.ToString(System.Globalization.CultureInfo.CurrentCulture) ?? "-"),
                    ("Monthly HOA", p => $"{p.MonthlyHOA:C0}")
                };

                foreach (var (label, value) in rows)
                {
                    table.Cell().Element(cell => BodyCellStyle(cell, highlight: false)).Text(label).SemiBold();
                    foreach (var property in properties)
                    {
                        table.Cell().Element(cell => BodyCellStyle(cell, highlight: false)).Text(value(property));
                    }
                }
            });
        });
    }

    private static void ComposePropertyFacts(IContainer container, Property property)
    {
        var facts = new List<(string, string)>
        {
            ("Asking Price", $"{property.AskingPrice:C0}"),
            ("Bedrooms / Bathrooms", $"{property.Bedrooms} / {property.Bathrooms}"),
            ("Square Feet", property.SquareFeet > 0 ? $"{property.SquareFeet:N0}" : "-"),
            ("Price / Sqft", property.SquareFeet > 0 ? $"{property.PricePerSquareFoot:C0}" : "-"),
            ("Year Built", property.YearBuilt?.ToString(System.Globalization.CultureInfo.CurrentCulture) ?? "-"),
            ("Property Type", property.PropertyType.ToString()),
            ("Monthly HOA", $"{property.MonthlyHOA:C0}")
        };

        if (property.OfferPrice.HasValue)
        {
            facts.Insert(1, ("Offer Price", $"{property.OfferPrice.Value:C0}"));
        }

        ComposeKeyValueTable(container, "Property Facts", facts);
    }

    private static void ComposeScoreDetails(IContainer container, List<PropertyScore> scores)
    {
        container.Column(column =>
        {
            column.Spacing(6);
            column.Item().Text("Scores by Criterion").FontSize(12).SemiBold();

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.ConstantColumn(50);
                    columns.ConstantColumn(60);
                    columns.RelativeColumn(4);
                });

                table.Header(header =>
                {
                    foreach (var label in ScoreDetailHeaders)
                    {
                        header.Cell().Element(HeaderCellStyle).Text(label);
                    }
                });

                foreach (var score in scores.OrderByDescending(s => s.Criterion?.Weight ?? 0))
                {
                    table.Cell().Element(cell => BodyCellStyle(cell, highlight: false))
                        .Text(score.Criterion?.Name ?? "(deleted criterion)");
                    table.Cell().Element(cell => ScoreCellStyle(cell, score.Score)).Text($"{score.Score}");
                    table.Cell().Element(cell => BodyCellStyle(cell, highlight: false)).Text($"{score.WeightedScore}");
                    table.Cell().Element(cell => BodyCellStyle(cell, highlight: false)).Text(score.Notes ?? "");
                }
            });
        });
    }

    private static void ComposeKeyValueTable(IContainer container, string title, IReadOnlyList<(string Label, string Value)> rows)
    {
        container.Column(column =>
        {
            column.Spacing(6);
            column.Item().Text(title).FontSize(12).SemiBold();

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });

                foreach (var (label, value) in rows)
                {
                    table.Cell().Element(cell => BodyCellStyle(cell, highlight: false)).Text(label).SemiBold();
                    table.Cell().Element(cell => BodyCellStyle(cell, highlight: false)).Text(value);
                }
            });
        });
    }

    private static IContainer HeaderCellStyle(IContainer container)
    {
        return container
            .Background(Colors.Indigo.Darken2)
            .PaddingVertical(4)
            .PaddingHorizontal(6)
            .DefaultTextStyle(style => style.FontColor(Colors.White).SemiBold());
    }

    private static IContainer BodyCellStyle(IContainer container, bool highlight)
    {
        return container
            .Background(highlight ? Colors.Green.Lighten4 : Colors.White)
            .BorderBottom(0.5f)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(4)
            .PaddingHorizontal(6);
    }

    private static IContainer ScoreCellStyle(IContainer container, int score)
    {
        var background = score >= 8 ? Colors.Green.Lighten3
            : score >= 5 ? Colors.Yellow.Lighten3
            : Colors.Red.Lighten3;

        return container
            .Background(background)
            .BorderBottom(0.5f)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(4)
            .PaddingHorizontal(6);
    }

    private static string FormatAddress(Property property)
    {
        var parts = new[] { property.Address, property.City, property.State }
            .Where(part => !string.IsNullOrWhiteSpace(part));
        var address = string.Join(", ", parts);
        return string.IsNullOrWhiteSpace(property.ZipCode) ? address : $"{address} {property.ZipCode}";
    }
}
