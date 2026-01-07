namespace HomeBuyerHelper.Core.Models;

/// <summary>
/// Represents a user-defined criterion for evaluating properties.
/// </summary>
public class EvaluationCriterion
{
    public int Id { get; set; }

    /// <summary>
    /// Name of the criterion (e.g., "Kitchen Quality", "Backyard Size").
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Optional description explaining what this criterion measures.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Weight/importance of this criterion (1-10 scale).
    /// Higher values mean more important.
    /// </summary>
    public int Weight { get; set; } = 5;

    /// <summary>
    /// Category for grouping related criteria.
    /// </summary>
    public CriterionCategory Category { get; set; } = CriterionCategory.Other;

    /// <summary>
    /// Whether this is a system-suggested criterion vs user-created.
    /// </summary>
    public bool IsSystemSuggested { get; set; }

    /// <summary>
    /// Display order for sorting criteria in the UI.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// When the criterion was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the criterion was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Categories for grouping evaluation criteria.
/// </summary>
public enum CriterionCategory
{
    Location,
    Interior,
    Exterior,
    Neighborhood,
    Financial,
    Lifestyle,
    Other
}
