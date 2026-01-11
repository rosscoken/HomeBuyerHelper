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
    private readonly IUserPreferencesRepository _preferencesRepository;

    public ExportService(
        IPropertyRepository propertyRepository,
        IScoreRepository scoreRepository,
        ICriteriaRepository criteriaRepository,
        IUserPreferencesRepository preferencesRepository)
    {
        _propertyRepository = propertyRepository;
        _scoreRepository = scoreRepository;
        _criteriaRepository = criteriaRepository;
        _preferencesRepository = preferencesRepository;
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

        var json = System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
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
        return await ImportFromJsonAsync(jsonContent, replaceExisting: false);
    }

    public async Task<bool> ImportFromJsonAsync(string jsonContent, bool replaceExisting)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
                return false;

            // Validate JSON structure
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            var importData = System.Text.Json.JsonSerializer.Deserialize<ExportDataModel>(jsonContent, options);
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

    public async Task<ImportValidationResult> ValidateImportFileAsync(string jsonContent)
    {
        var result = new ImportValidationResult();

        try
        {
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                result.IsValid = false;
                result.ErrorMessage = "File is empty.";
                return result;
            }

            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            var importData = System.Text.Json.JsonSerializer.Deserialize<ExportDataModel>(jsonContent, options);

            if (importData == null)
            {
                result.IsValid = false;
                result.ErrorMessage = "Invalid file format.";
                return result;
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

        return result;
    }

    private class ExportDataModel
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
}
