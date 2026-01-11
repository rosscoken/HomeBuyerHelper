using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Data.Entities;

namespace HomeBuyerHelper.Data.Repositories;

/// <summary>
/// SQLite implementation of IIncomeRepository.
/// </summary>
public class IncomeRepository : IIncomeRepository
{
    private readonly DatabaseService _databaseService;

    public IncomeRepository(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<IncomeSource?> GetByIdAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = await db.FindAsync<IncomeSourceEntity>(id);
        return entity == null ? null : MapToModel(entity);
    }

    public async Task<IReadOnlyList<IncomeSource>> GetAllAsync()
    {
        var db = await _databaseService.GetConnectionAsync();
        var entities = await db.Table<IncomeSourceEntity>()
            .OrderBy(i => i.Name)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<IReadOnlyList<IncomeSource>> GetByTypeAsync(IncomeType type)
    {
        var db = await _databaseService.GetConnectionAsync();
        var typeInt = (int)type;
        var entities = await db.Table<IncomeSourceEntity>()
            .Where(i => i.IncomeType == typeInt)
            .OrderBy(i => i.Name)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<IReadOnlyList<IncomeSource>> GetReliableAsync()
    {
        var db = await _databaseService.GetConnectionAsync();
        var entities = await db.Table<IncomeSourceEntity>()
            .Where(i => i.IsReliable)
            .OrderBy(i => i.Name)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<int> CreateAsync(IncomeSource incomeSource)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = MapToEntity(incomeSource);
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.InsertAsync(entity);
        return entity.Id;
    }

    public async Task UpdateAsync(IncomeSource incomeSource)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = MapToEntity(incomeSource);
        entity.UpdatedAt = DateTime.UtcNow;
        await db.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        await db.DeleteAsync<IncomeSourceEntity>(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        var count = await db.Table<IncomeSourceEntity>()
            .Where(i => i.Id == id)
            .CountAsync();
        return count > 0;
    }

    public async Task<decimal> GetTotalMonthlyGrossIncomeAsync()
    {
        var allIncome = await GetAllAsync();
        return allIncome.Sum(i => i.MonthlyGrossIncome);
    }

    public async Task<decimal> GetTotalReliableMonthlyIncomeAsync()
    {
        var reliableIncome = await GetReliableAsync();
        return reliableIncome.Sum(i => i.MonthlyGrossIncome);
    }

    public async Task<int> GetCountAsync()
    {
        var db = await _databaseService.GetConnectionAsync();
        return await db.Table<IncomeSourceEntity>().CountAsync();
    }

    private static IncomeSource MapToModel(IncomeSourceEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        GrossAmount = entity.GrossAmount,
        NetAmount = entity.NetAmount,
        Frequency = (IncomeFrequency)entity.Frequency,
        IncomeType = (IncomeType)entity.IncomeType,
        IsReliable = entity.IsReliable,
        Notes = entity.Notes,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    private static IncomeSourceEntity MapToEntity(IncomeSource model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        GrossAmount = model.GrossAmount,
        NetAmount = model.NetAmount,
        Frequency = (int)model.Frequency,
        IncomeType = (int)model.IncomeType,
        IsReliable = model.IsReliable,
        Notes = model.Notes,
        CreatedAt = model.CreatedAt,
        UpdatedAt = model.UpdatedAt
    };
}
