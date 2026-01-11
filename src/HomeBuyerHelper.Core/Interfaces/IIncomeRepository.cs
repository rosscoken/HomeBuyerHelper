using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Repository interface for IncomeSource CRUD operations.
/// </summary>
public interface IIncomeRepository
{
    /// <summary>
    /// Gets an income source by its ID.
    /// </summary>
    Task<IncomeSource?> GetByIdAsync(int id);

    /// <summary>
    /// Gets all income sources.
    /// </summary>
    Task<IReadOnlyList<IncomeSource>> GetAllAsync();

    /// <summary>
    /// Gets income sources by type.
    /// </summary>
    Task<IReadOnlyList<IncomeSource>> GetByTypeAsync(IncomeType type);

    /// <summary>
    /// Gets only reliable income sources.
    /// </summary>
    Task<IReadOnlyList<IncomeSource>> GetReliableAsync();

    /// <summary>
    /// Creates a new income source.
    /// </summary>
    /// <returns>The ID of the created income source.</returns>
    Task<int> CreateAsync(IncomeSource incomeSource);

    /// <summary>
    /// Updates an existing income source.
    /// </summary>
    Task UpdateAsync(IncomeSource incomeSource);

    /// <summary>
    /// Deletes an income source by its ID.
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Checks if an income source exists.
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Gets the total monthly gross income from all sources.
    /// </summary>
    Task<decimal> GetTotalMonthlyGrossIncomeAsync();

    /// <summary>
    /// Gets the total monthly gross income from reliable sources only.
    /// </summary>
    Task<decimal> GetTotalReliableMonthlyIncomeAsync();

    /// <summary>
    /// Gets the count of all income sources.
    /// </summary>
    Task<int> GetCountAsync();
}
