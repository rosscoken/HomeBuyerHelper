using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Criteria template export/import (P4-TMP-001/002).
/// Templates carry only criteria definitions — never personal data.
/// </summary>
public interface ICriteriaTemplateService
{
    /// <summary>
    /// Serializes criteria into a shareable template JSON.
    /// </summary>
    string ExportTemplate(string name, string? description, IEnumerable<EvaluationCriterion> criteria);

    /// <summary>
    /// Parses and validates a template JSON. Throws InvalidDataException
    /// with a user-readable message when the file is not a valid template.
    /// </summary>
    CriteriaTemplate ParseTemplate(string json);

    /// <summary>
    /// Converts a template's entries into criteria ready to insert.
    /// </summary>
    IReadOnlyList<EvaluationCriterion> ToCriteria(CriteriaTemplate template);
}

/// <summary>
/// A shareable, anonymized criteria template.
/// </summary>
public class CriteriaTemplate
{
    public string Version { get; set; } = "1.0";
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<CriteriaTemplateEntry> Criteria { get; set; } = new();
}

/// <summary>
/// One criterion in a template.
/// </summary>
public class CriteriaTemplateEntry
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Weight { get; set; }
    public string Category { get; set; } = nameof(CriterionCategory.Other);
    public string? ScoreAnchorLow { get; set; }
    public string? ScoreAnchorHigh { get; set; }
}
