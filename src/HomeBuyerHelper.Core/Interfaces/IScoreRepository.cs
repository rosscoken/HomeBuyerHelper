using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Repository interface for PropertyScore CRUD operations.
/// </summary>
public interface IScoreRepository
{
    /// <summary>
    /// Gets a score by its ID.
    /// </summary>
    Task<PropertyScore?> GetByIdAsync(int id);

    /// <summary>
    /// Gets all scores for a property.
    /// </summary>
    Task<IReadOnlyList<PropertyScore>> GetByPropertyIdAsync(int propertyId);

    /// <summary>
    /// Gets all scores for a criterion.
    /// </summary>
    Task<IReadOnlyList<PropertyScore>> GetByCriterionIdAsync(int criterionId);

    /// <summary>
    /// Gets a specific score for a property and criterion.
    /// </summary>
    Task<PropertyScore?> GetByPropertyAndCriterionAsync(int propertyId, int criterionId);

    /// <summary>
    /// Creates or updates a score.
    /// </summary>
    /// <returns>The ID of the created/updated score.</returns>
    Task<int> UpsertAsync(PropertyScore score);

    /// <summary>
    /// Deletes a score by its ID.
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Deletes all scores for a property.
    /// </summary>
    Task DeleteByPropertyIdAsync(int propertyId);

    /// <summary>
    /// Deletes all scores for a criterion.
    /// </summary>
    Task DeleteByCriterionIdAsync(int criterionId);

    /// <summary>
    /// Gets the total weighted score for a property.
    /// </summary>
    Task<int> GetTotalWeightedScoreAsync(int propertyId);

    /// <summary>
    /// Gets the maximum possible weighted score.
    /// </summary>
    Task<int> GetMaxPossibleScoreAsync();
}
