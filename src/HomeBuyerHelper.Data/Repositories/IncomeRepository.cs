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
            .OrderBy(s => s.Id)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<int> CreateAsync(IncomeSource source)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = MapToEntity(source);
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.InsertAsync(entity);
        return entity.Id;
    }

    public async Task UpdateAsync(IncomeSource source)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = MapToEntity(source);
        entity.UpdatedAt = DateTime.UtcNow;
        await db.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        await db.DeleteAsync<IncomeSourceEntity>(id);
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
        PaymentMonth = entity.PaymentMonth,
        Probability = entity.Probability,
        StartDate = entity.StartDate,
        EndDate = entity.EndDate,
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
        PaymentMonth = model.PaymentMonth,
        Probability = model.Probability,
        StartDate = model.StartDate,
        EndDate = model.EndDate,
        Notes = model.Notes,
        CreatedAt = model.CreatedAt,
        UpdatedAt = model.UpdatedAt
    };
}
