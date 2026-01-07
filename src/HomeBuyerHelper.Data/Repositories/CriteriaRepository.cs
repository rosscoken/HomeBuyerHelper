using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Data.Entities;

namespace HomeBuyerHelper.Data.Repositories;

/// <summary>
/// SQLite implementation of ICriteriaRepository.
/// </summary>
public class CriteriaRepository : ICriteriaRepository
{
    private readonly DatabaseService _databaseService;

    public CriteriaRepository(IDatabaseService databaseService)
    {
        _databaseService = (DatabaseService)databaseService;
    }

    public async Task<EvaluationCriterion?> GetByIdAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = await db.FindAsync<EvaluationCriterionEntity>(id);
        return entity == null ? null : MapToModel(entity);
    }

    public async Task<IReadOnlyList<EvaluationCriterion>> GetAllAsync()
    {
        var db = await _databaseService.GetConnectionAsync();
        var entities = await db.Table<EvaluationCriterionEntity>()
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<IReadOnlyList<EvaluationCriterion>> GetByCategoryAsync(CriterionCategory category)
    {
        var db = await _databaseService.GetConnectionAsync();
        var categoryInt = (int)category;
        var entities = await db.Table<EvaluationCriterionEntity>()
            .Where(c => c.Category == categoryInt)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<int> CreateAsync(EvaluationCriterion criterion)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = MapToEntity(criterion);
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        // Set display order to next available
        var maxOrder = await db.Table<EvaluationCriterionEntity>()
            .OrderByDescending(c => c.DisplayOrder)
            .FirstOrDefaultAsync();
        entity.DisplayOrder = (maxOrder?.DisplayOrder ?? 0) + 1;

        await db.InsertAsync(entity);
        return entity.Id;
    }

    public async Task CreateManyAsync(IEnumerable<EvaluationCriterion> criteria)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entities = criteria.Select((c, i) =>
        {
            var entity = MapToEntity(c);
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.DisplayOrder = i + 1;
            return entity;
        }).ToList();

        await db.InsertAllAsync(entities);
    }

    public async Task UpdateAsync(EvaluationCriterion criterion)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = MapToEntity(criterion);
        entity.UpdatedAt = DateTime.UtcNow;
        await db.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        await db.DeleteAsync<EvaluationCriterionEntity>(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        var count = await db.Table<EvaluationCriterionEntity>()
            .Where(c => c.Id == id)
            .CountAsync();
        return count > 0;
    }

    public async Task<int> GetCountAsync()
    {
        var db = await _databaseService.GetConnectionAsync();
        return await db.Table<EvaluationCriterionEntity>().CountAsync();
    }

    public async Task ReorderAsync(IEnumerable<(int Id, int NewOrder)> newOrders)
    {
        var db = await _databaseService.GetConnectionAsync();
        foreach (var (id, newOrder) in newOrders)
        {
            await db.ExecuteAsync(
                "UPDATE EvaluationCriteria SET DisplayOrder = ?, UpdatedAt = ? WHERE Id = ?",
                newOrder, DateTime.UtcNow, id);
        }
    }

    private static EvaluationCriterion MapToModel(EvaluationCriterionEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        Weight = entity.Weight,
        Category = (CriterionCategory)entity.Category,
        IsSystemSuggested = entity.IsSystemSuggested,
        DisplayOrder = entity.DisplayOrder,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    private static EvaluationCriterionEntity MapToEntity(EvaluationCriterion model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        Description = model.Description,
        Weight = model.Weight,
        Category = (int)model.Category,
        IsSystemSuggested = model.IsSystemSuggested,
        DisplayOrder = model.DisplayOrder,
        CreatedAt = model.CreatedAt,
        UpdatedAt = model.UpdatedAt
    };
}
