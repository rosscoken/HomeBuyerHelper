using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Data.Entities;

namespace HomeBuyerHelper.Data.Repositories;

/// <summary>
/// SQLite implementation of IOneTimeEventRepository.
/// </summary>
public class OneTimeEventRepository : IOneTimeEventRepository
{
    private readonly DatabaseService _databaseService;

    public OneTimeEventRepository(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<OneTimeEvent?> GetByIdAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = await db.FindAsync<OneTimeEventEntity>(id);
        return entity == null ? null : MapToModel(entity);
    }

    public async Task<IReadOnlyList<OneTimeEvent>> GetAllAsync()
    {
        var db = await _databaseService.GetConnectionAsync();
        var entities = await db.Table<OneTimeEventEntity>()
            .OrderBy(e => e.Date)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<int> CreateAsync(OneTimeEvent oneTimeEvent)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = MapToEntity(oneTimeEvent);
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.InsertAsync(entity);
        return entity.Id;
    }

    public async Task UpdateAsync(OneTimeEvent oneTimeEvent)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = MapToEntity(oneTimeEvent);
        entity.UpdatedAt = DateTime.UtcNow;
        await db.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        await db.DeleteAsync<OneTimeEventEntity>(id);
    }

    private static OneTimeEvent MapToModel(OneTimeEventEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Amount = entity.Amount,
        Date = entity.Date,
        Category = (OneTimeEventCategory)entity.Category,
        Notes = entity.Notes,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    private static OneTimeEventEntity MapToEntity(OneTimeEvent model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        Amount = model.Amount,
        Date = model.Date,
        Category = (int)model.Category,
        Notes = model.Notes,
        CreatedAt = model.CreatedAt,
        UpdatedAt = model.UpdatedAt
    };
}
