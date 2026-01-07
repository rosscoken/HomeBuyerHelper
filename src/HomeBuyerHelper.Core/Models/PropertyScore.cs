namespace HomeBuyerHelper.Core.Models;

/// <summary>
/// Represents a score given to a property for a specific evaluation criterion.
/// </summary>
public class PropertyScore
{
    public int Id { get; set; }

    /// <summary>
    /// The property being scored.
    /// </summary>
    public int PropertyId { get; set; }

    /// <summary>
    /// The criterion being evaluated.
    /// </summary>
    public int CriterionId { get; set; }

    /// <summary>
    /// The score value (1-10 scale).
    /// 1-2 = Poor, 3-4 = Below Average, 5-6 = Average, 7-8 = Good, 9-10 = Excellent
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Optional notes explaining the score.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// When the score was recorded.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the score was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation property for the property.
    /// </summary>
    public Property? Property { get; set; }

    /// <summary>
    /// Navigation property for the criterion.
    /// </summary>
    public EvaluationCriterion? Criterion { get; set; }

    /// <summary>
    /// Gets the weighted score (score * criterion weight).
    /// Returns 0 if criterion was deleted.
    /// </summary>
    public int WeightedScore => Criterion?.Weight is int weight ? Score * weight : 0;
}
