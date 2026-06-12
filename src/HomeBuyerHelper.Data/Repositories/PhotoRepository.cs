using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Data.Entities;

namespace HomeBuyerHelper.Data.Repositories;

/// <summary>
/// SQLite implementation of IPhotoRepository.
/// </summary>
public class PhotoRepository : IPhotoRepository
{
    private readonly DatabaseService _databaseService;

    public PhotoRepository(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<IReadOnlyList<PropertyPhoto>> GetByPropertyIdAsync(int propertyId)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entities = await db.Table<PropertyPhotoEntity>()
            .Where(p => p.PropertyId == propertyId)
            .OrderBy(p => p.SortOrder)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<int> CreateAsync(PropertyPhoto photo)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = MapToEntity(photo);
        entity.CreatedAt = DateTime.UtcNow;
        await db.InsertAsync(entity);
        return entity.Id;
    }

    public async Task DeleteAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        await db.DeleteAsync<PropertyPhotoEntity>(id);
    }

    public async Task DeleteByPropertyIdAsync(int propertyId)
    {
        var db = await _databaseService.GetConnectionAsync();
        await db.ExecuteAsync("DELETE FROM PropertyPhotos WHERE PropertyId = ?", propertyId);
    }

    private static PropertyPhoto MapToModel(PropertyPhotoEntity entity) => new()
    {
        Id = entity.Id,
        PropertyId = entity.PropertyId,
        FilePath = entity.FilePath,
        Caption = entity.Caption,
        SortOrder = entity.SortOrder,
        CreatedAt = entity.CreatedAt
    };

    private static PropertyPhotoEntity MapToEntity(PropertyPhoto model) => new()
    {
        Id = model.Id,
        PropertyId = model.PropertyId,
        FilePath = model.FilePath,
        Caption = model.Caption,
        SortOrder = model.SortOrder,
        CreatedAt = model.CreatedAt
    };
}
