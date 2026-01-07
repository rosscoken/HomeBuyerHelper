using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Repository interface for EvaluationCriterion CRUD operations.
/// </summary>
public interface ICriteriaRepository
{
    /// <summary>
    /// Gets a criterion by its ID.
    /// </summary>
    Task<EvaluationCriterion?> GetByIdAsync(int id);

    /// <summary>
    /// Gets all criteria.
    /// </summary>
    Task<IReadOnlyList<EvaluationCriterion>> GetAllAsync();

    /// <summary>
    /// Gets criteria by category.
    /// </summary>
    Task<IReadOnlyList<EvaluationCriterion>> GetByCategoryAsync(CriterionCategory category);

    /// <summary>
    /// Creates a new criterion.
    /// </summary>
    /// <returns>The ID of the created criterion.</returns>
    Task<int> CreateAsync(EvaluationCriterion criterion);

    /// <summary>
    /// Creates multiple criteria at once.
    /// </summary>
    Task CreateManyAsync(IEnumerable<EvaluationCriterion> criteria);

    /// <summary>
    /// Updates an existing criterion.
    /// </summary>
    Task UpdateAsync(EvaluationCriterion criterion);

    /// <summary>
    /// Deletes a criterion by its ID.
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Checks if a criterion exists.
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Gets the count of all criteria.
    /// </summary>
    Task<int> GetCountAsync();

    /// <summary>
    /// Reorders criteria by updating their DisplayOrder.
    /// </summary>
    Task ReorderAsync(IEnumerable<(int Id, int NewOrder)> newOrders);
}
