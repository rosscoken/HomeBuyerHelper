using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Repository interface for Property CRUD operations.
/// </summary>
public interface IPropertyRepository
{
    /// <summary>
    /// Gets a property by its ID.
    /// </summary>
    Task<Property?> GetByIdAsync(int id);

    /// <summary>
    /// Gets all properties.
    /// </summary>
    Task<IReadOnlyList<Property>> GetAllAsync();

    /// <summary>
    /// Gets all active (non-archived) properties.
    /// </summary>
    Task<IReadOnlyList<Property>> GetActiveAsync();

    /// <summary>
    /// Gets all favorite properties.
    /// </summary>
    Task<IReadOnlyList<Property>> GetFavoritesAsync();

    /// <summary>
    /// Creates a new property.
    /// </summary>
    /// <returns>The ID of the created property.</returns>
    Task<int> CreateAsync(Property property);

    /// <summary>
    /// Updates an existing property.
    /// </summary>
    Task UpdateAsync(Property property);

    /// <summary>
    /// Deletes a property by its ID.
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Checks if a property exists.
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Gets the count of all properties.
    /// </summary>
    Task<int> GetCountAsync();
}
