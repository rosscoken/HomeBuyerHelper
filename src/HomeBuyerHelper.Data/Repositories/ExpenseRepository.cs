using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Data.Entities;

namespace HomeBuyerHelper.Data.Repositories;

/// <summary>
/// SQLite implementation of IExpenseRepository.
/// </summary>
public class ExpenseRepository : IExpenseRepository
{
    private readonly DatabaseService _databaseService;

    public ExpenseRepository(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<Expense?> GetByIdAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = await db.FindAsync<ExpenseEntity>(id);
        return entity == null ? null : MapToModel(entity);
    }

    public async Task<IReadOnlyList<Expense>> GetAllAsync()
    {
        var db = await _databaseService.GetConnectionAsync();
        var entities = await db.Table<ExpenseEntity>()
            .OrderBy(e => e.Id)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<int> CreateAsync(Expense expense)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = MapToEntity(expense);
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.InsertAsync(entity);
        return entity.Id;
    }

    public async Task UpdateAsync(Expense expense)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = MapToEntity(expense);
        entity.UpdatedAt = DateTime.UtcNow;
        await db.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        await db.DeleteAsync<ExpenseEntity>(id);
    }

    private static Expense MapToModel(ExpenseEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Amount = entity.Amount,
        Frequency = (ExpenseFrequency)entity.Frequency,
        Category = (ExpenseCategory)entity.Category,
        IsEssential = entity.IsEssential,
        IsVariable = entity.IsVariable,
        ContinuesAfterPurchase = entity.ContinuesAfterPurchase,
        Notes = entity.Notes,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    private static ExpenseEntity MapToEntity(Expense model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        Amount = model.Amount,
        Frequency = (int)model.Frequency,
        Category = (int)model.Category,
        IsEssential = model.IsEssential,
        IsVariable = model.IsVariable,
        ContinuesAfterPurchase = model.ContinuesAfterPurchase,
        Notes = model.Notes,
        CreatedAt = model.CreatedAt,
        UpdatedAt = model.UpdatedAt
    };
}
