using System.Text.Json;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Services;

/// <summary>
/// Criteria template export/import with schema validation (P4-TMP-001/002).
/// </summary>
public class CriteriaTemplateService : ICriteriaTemplateService
{
    private const string SupportedVersion = "1.0";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public string ExportTemplate(string name, string? description, IEnumerable<EvaluationCriterion> criteria)
    {
        var template = new CriteriaTemplate
        {
            Version = SupportedVersion,
            Name = name,
            Description = description,
            Criteria = criteria.Select(criterion => new CriteriaTemplateEntry
            {
                Name = criterion.Name,
                Description = criterion.Description,
                Weight = criterion.Weight,
                Category = criterion.Category.ToString(),
                ScoreAnchorLow = criterion.ScoreAnchorLow,
                ScoreAnchorHigh = criterion.ScoreAnchorHigh
            }).ToList()
        };

        return JsonSerializer.Serialize(template, JsonOptions);
    }

    public CriteriaTemplate ParseTemplate(string json)
    {
        CriteriaTemplate? template;
        try
        {
            template = JsonSerializer.Deserialize<CriteriaTemplate>(json, JsonOptions);
        }
        catch (JsonException)
        {
            throw new InvalidDataException("The file is not a valid criteria template.");
        }

        if (template == null || template.Criteria.Count == 0)
        {
            throw new InvalidDataException("The template contains no criteria.");
        }

        if (template.Version != SupportedVersion)
        {
            throw new InvalidDataException(
                $"Template version '{template.Version}' is not supported (expected {SupportedVersion}).");
        }

        if (template.Criteria.Any(entry => string.IsNullOrWhiteSpace(entry.Name) || entry.Weight <= 0))
        {
            throw new InvalidDataException("Every template criterion needs a name and a positive weight.");
        }

        return template;
    }

    public IReadOnlyList<EvaluationCriterion> ToCriteria(CriteriaTemplate template)
    {
        return template.Criteria.Select((entry, index) => new EvaluationCriterion
        {
            Name = entry.Name.Trim(),
            Description = entry.Description,
            Weight = entry.Weight,
            Category = Enum.TryParse<CriterionCategory>(entry.Category, ignoreCase: true, out var category)
                ? category
                : CriterionCategory.Other,
            ScoreAnchorLow = entry.ScoreAnchorLow,
            ScoreAnchorHigh = entry.ScoreAnchorHigh,
            DisplayOrder = index,
            IsSystemSuggested = false
        }).ToList();
    }
}
