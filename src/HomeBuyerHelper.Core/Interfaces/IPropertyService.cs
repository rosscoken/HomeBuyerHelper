using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Service interface for property business operations.
/// </summary>
public interface IPropertyService
{
    /// <summary>
    /// Gets all active properties with their scores.
    /// </summary>
    Task<IReadOnlyList<Property>> GetPropertiesWithScoresAsync();

    /// <summary>
    /// Gets a property with all related data.
    /// </summary>
    Task<Property?> GetPropertyDetailAsync(int id);

    /// <summary>
    /// Creates a new property.
    /// </summary>
    Task<Property> CreatePropertyAsync(Property property);

    /// <summary>
    /// Updates an existing property.
    /// </summary>
    Task UpdatePropertyAsync(Property property);

    /// <summary>
    /// Archives a property (soft delete).
    /// </summary>
    Task ArchivePropertyAsync(int id);

    /// <summary>
    /// Permanently deletes a property and all related data.
    /// </summary>
    Task DeletePropertyAsync(int id);

    /// <summary>
    /// Toggles the favorite status of a property.
    /// </summary>
    Task ToggleFavoriteAsync(int id);

    /// <summary>
    /// Gets property comparison data for multiple properties.
    /// </summary>
    Task<IReadOnlyList<PropertyComparisonResult>> ComparePropertiesAsync(IEnumerable<int> propertyIds);

    /// <summary>
    /// Gets property ranking based on weighted scores.
    /// </summary>
    Task<IReadOnlyList<PropertyRanking>> GetPropertyRankingsAsync();
}

/// <summary>
/// Result of comparing properties.
/// </summary>
public class PropertyComparisonResult
{
    public required Property Property { get; init; }
    public required IReadOnlyList<PropertyScore> Scores { get; init; }
    public int TotalWeightedScore { get; init; }
    public decimal ScorePercentage { get; init; }
}

/// <summary>
/// Property ranking result.
/// </summary>
public class PropertyRanking
{
    public required Property Property { get; init; }
    public int Rank { get; init; }
    public int TotalWeightedScore { get; init; }
    public decimal ScorePercentage { get; init; }
}
