using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Repository interface for funding source operations.
/// </summary>
public interface IFundingRepository
{
    /// <summary>Gets a funding source by ID.</summary>
    Task<FundingSource?> GetByIdAsync(int id);

    /// <summary>Gets all funding sources.</summary>
    Task<IReadOnlyList<FundingSource>> GetAllAsync();

    /// <summary>Creates a new funding source and returns its ID.</summary>
    Task<int> CreateAsync(FundingSource source);

    /// <summary>Updates an existing funding source.</summary>
    Task UpdateAsync(FundingSource source);

    /// <summary>Deletes a funding source.</summary>
    Task DeleteAsync(int id);
}

/// <summary>
/// Repository interface for property photo records.
/// </summary>
public interface IPhotoRepository
{
    /// <summary>Gets all photos for a property in display order.</summary>
    Task<IReadOnlyList<PropertyPhoto>> GetByPropertyIdAsync(int propertyId);

    /// <summary>Creates a new photo record and returns its ID.</summary>
    Task<int> CreateAsync(PropertyPhoto photo);

    /// <summary>Deletes a photo record.</summary>
    Task DeleteAsync(int id);

    /// <summary>Deletes all photo records for a property.</summary>
    Task DeleteByPropertyIdAsync(int propertyId);
}

/// <summary>
/// Repository interface for property pros/cons.
/// </summary>
public interface IProConRepository
{
    /// <summary>Gets all pros/cons for a property in display order.</summary>
    Task<IReadOnlyList<PropertyProCon>> GetByPropertyIdAsync(int propertyId);

    /// <summary>Creates a new pro/con item and returns its ID.</summary>
    Task<int> CreateAsync(PropertyProCon item);

    /// <summary>Updates an existing pro/con item.</summary>
    Task UpdateAsync(PropertyProCon item);

    /// <summary>Deletes a pro/con item.</summary>
    Task DeleteAsync(int id);

    /// <summary>Deletes all pros/cons for a property.</summary>
    Task DeleteByPropertyIdAsync(int propertyId);
}
