using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Services;

/// <summary>
/// Export service for generating reports and sharing data.
/// </summary>
public class ExportService : IExportService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IScoreRepository _scoreRepository;
    private readonly ICriteriaRepository _criteriaRepository;

    public ExportService(
        IPropertyRepository propertyRepository,
        IScoreRepository scoreRepository,
        ICriteriaRepository criteriaRepository)
    {
        _propertyRepository = propertyRepository;
        _scoreRepository = scoreRepository;
        _criteriaRepository = criteriaRepository;
    }

    public async Task<string> ExportComparisonToPdfAsync(IEnumerable<int> propertyIds)
    {
        // PDF generation will be implemented with QuestPDF in Phase 2
        // For now, return a placeholder path
        var exportDir = GetExportDirectory();
        var filePath = Path.Combine(exportDir, $"comparison_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

        // TODO: Implement PDF generation with QuestPDF
        await Task.CompletedTask;

        return filePath;
    }

    public async Task<string> ExportPropertyDetailToPdfAsync(int propertyId)
    {
        var exportDir = GetExportDirectory();
        var filePath = Path.Combine(exportDir, $"property_{propertyId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

        // TODO: Implement PDF generation with QuestPDF
        await Task.CompletedTask;

        return filePath;
    }

    public async Task<string> ExportBudgetToPdfAsync(int propertyId)
    {
        var exportDir = GetExportDirectory();
        var filePath = Path.Combine(exportDir, $"budget_{propertyId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

        // TODO: Implement PDF generation with QuestPDF
        await Task.CompletedTask;

        return filePath;
    }

    public async Task<string> ExportAllDataAsync()
    {
        var exportDir = GetExportDirectory();
        var filePath = Path.Combine(exportDir, $"homebuyerhelper_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json");

        var properties = await _propertyRepository.GetAllAsync();
        var criteria = await _criteriaRepository.GetAllAsync();

        var exportData = new
        {
            ExportDate = DateTime.UtcNow,
            Version = "1.0",
            Properties = properties,
            Criteria = criteria
            // Scores will be included with properties in full implementation
        };

        var json = System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });

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
        try
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
                return false;

            // Validate JSON structure
            var importData = System.Text.Json.JsonSerializer.Deserialize<ImportDataModel>(jsonContent);
            if (importData == null)
                return false;

            // Import criteria first (properties may reference them)
            if (importData.Criteria != null)
            {
                foreach (var criterion in importData.Criteria)
                {
                    criterion.Id = 0; // Reset ID for new insert
                    await _criteriaRepository.CreateAsync(criterion);
                }
            }

            // Import properties
            if (importData.Properties != null)
            {
                foreach (var property in importData.Properties)
                {
                    property.Id = 0; // Reset ID for new insert
                    await _propertyRepository.CreateAsync(property);
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private class ImportDataModel
    {
        public DateTime ExportDate { get; set; }
        public string? Version { get; set; }
        public List<Property>? Properties { get; set; }
        public List<EvaluationCriterion>? Criteria { get; set; }
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
}
