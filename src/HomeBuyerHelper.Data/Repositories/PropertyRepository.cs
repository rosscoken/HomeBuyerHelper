using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Data.Entities;

namespace HomeBuyerHelper.Data.Repositories;

/// <summary>
/// SQLite implementation of IPropertyRepository.
/// </summary>
public class PropertyRepository : IPropertyRepository
{
    private readonly DatabaseService _databaseService;

    public PropertyRepository(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<Property?> GetByIdAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = await db.FindAsync<PropertyEntity>(id);
        return entity == null ? null : MapToModel(entity);
    }

    public async Task<IReadOnlyList<Property>> GetAllAsync()
    {
        var db = await _databaseService.GetConnectionAsync();
        var entities = await db.Table<PropertyEntity>()
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<IReadOnlyList<Property>> GetActiveAsync()
    {
        var db = await _databaseService.GetConnectionAsync();
        var entities = await db.Table<PropertyEntity>()
            .Where(p => !p.IsArchived)
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<IReadOnlyList<Property>> GetFavoritesAsync()
    {
        var db = await _databaseService.GetConnectionAsync();
        var entities = await db.Table<PropertyEntity>()
            .Where(p => p.IsFavorite && !p.IsArchived)
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<int> CreateAsync(Property property)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = MapToEntity(property);
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.InsertAsync(entity);
        return entity.Id;
    }

    public async Task UpdateAsync(Property property)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = MapToEntity(property);
        entity.UpdatedAt = DateTime.UtcNow;
        await db.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        await db.DeleteAsync<PropertyEntity>(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        var count = await db.Table<PropertyEntity>()
            .Where(p => p.Id == id)
            .CountAsync();
        return count > 0;
    }

    public async Task<int> GetCountAsync()
    {
        var db = await _databaseService.GetConnectionAsync();
        return await db.Table<PropertyEntity>().CountAsync();
    }

    private static Property MapToModel(PropertyEntity entity) => new()
    {
        Id = entity.Id,
        Nickname = entity.Nickname,
        Address = entity.Address,
        City = entity.City,
        State = entity.State,
        ZipCode = entity.ZipCode,
        AskingPrice = entity.AskingPrice,
        OfferPrice = entity.OfferPrice,
        Bedrooms = entity.Bedrooms,
        Bathrooms = entity.Bathrooms,
        SquareFeet = entity.SquareFeet,
        LotSquareFeet = entity.LotSquareFeet,
        YearBuilt = entity.YearBuilt,
        PropertyType = (PropertyType)entity.PropertyType,
        MonthlyHOA = entity.MonthlyHOA,
        AnnualPropertyTax = entity.AnnualPropertyTax,
        AnnualInsurance = entity.AnnualInsurance,
        ListingUrl = entity.ListingUrl,
        Notes = entity.Notes,
        IsFavorite = entity.IsFavorite,
        IsArchived = entity.IsArchived,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    private static PropertyEntity MapToEntity(Property model) => new()
    {
        Id = model.Id,
        Nickname = model.Nickname,
        Address = model.Address,
        City = model.City,
        State = model.State,
        ZipCode = model.ZipCode,
        AskingPrice = model.AskingPrice,
        OfferPrice = model.OfferPrice,
        Bedrooms = model.Bedrooms,
        Bathrooms = model.Bathrooms,
        SquareFeet = model.SquareFeet,
        LotSquareFeet = model.LotSquareFeet,
        YearBuilt = model.YearBuilt,
        PropertyType = (int)model.PropertyType,
        MonthlyHOA = model.MonthlyHOA,
        AnnualPropertyTax = model.AnnualPropertyTax,
        AnnualInsurance = model.AnnualInsurance,
        ListingUrl = model.ListingUrl,
        Notes = model.Notes,
        IsFavorite = model.IsFavorite,
        IsArchived = model.IsArchived,
        CreatedAt = model.CreatedAt,
        UpdatedAt = model.UpdatedAt
    };
}
