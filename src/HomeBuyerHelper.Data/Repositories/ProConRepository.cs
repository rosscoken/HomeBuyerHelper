using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Data.Entities;

namespace HomeBuyerHelper.Data.Repositories;

/// <summary>
/// SQLite implementation of IProConRepository.
/// </summary>
public class ProConRepository : IProConRepository
{
    private readonly DatabaseService _databaseService;

    public ProConRepository(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<IReadOnlyList<PropertyProCon>> GetByPropertyIdAsync(int propertyId)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entities = await db.Table<PropertyProConEntity>()
            .Where(p => p.PropertyId == propertyId)
            .OrderBy(p => p.SortOrder)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<int> CreateAsync(PropertyProCon item)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = MapToEntity(item);
        entity.CreatedAt = DateTime.UtcNow;
        await db.InsertAsync(entity);
        return entity.Id;
    }

    public async Task UpdateAsync(PropertyProCon item)
    {
        var db = await _databaseService.GetConnectionAsync();
        await db.UpdateAsync(MapToEntity(item));
    }

    public async Task DeleteAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        await db.DeleteAsync<PropertyProConEntity>(id);
    }

    public async Task DeleteByPropertyIdAsync(int propertyId)
    {
        var db = await _databaseService.GetConnectionAsync();
        await db.ExecuteAsync("DELETE FROM PropertyProsCons WHERE PropertyId = ?", propertyId);
    }

    private static PropertyProCon MapToModel(PropertyProConEntity entity) => new()
    {
        Id = entity.Id,
        PropertyId = entity.PropertyId,
        IsPro = entity.IsPro,
        Description = entity.Description,
        SortOrder = entity.SortOrder,
        CreatedAt = entity.CreatedAt
    };

    private static PropertyProConEntity MapToEntity(PropertyProCon model) => new()
    {
        Id = model.Id,
        PropertyId = model.PropertyId,
        IsPro = model.IsPro,
        Description = model.Description,
        SortOrder = model.SortOrder,
        CreatedAt = model.CreatedAt
    };
}
