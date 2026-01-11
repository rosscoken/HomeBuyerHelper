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
            .OrderBy(e => e.Category)
            .ThenBy(e => e.Name)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<IReadOnlyList<Expense>> GetByCategoryAsync(ExpenseCategory category)
    {
        var db = await _databaseService.GetConnectionAsync();
        var categoryInt = (int)category;
        var entities = await db.Table<ExpenseEntity>()
            .Where(e => e.Category == categoryInt)
            .OrderBy(e => e.Name)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<IReadOnlyList<Expense>> GetEssentialAsync()
    {
        var db = await _databaseService.GetConnectionAsync();
        var entities = await db.Table<ExpenseEntity>()
            .Where(e => e.IsEssential)
            .OrderBy(e => e.Category)
            .ThenBy(e => e.Name)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<IReadOnlyList<Expense>> GetContinuingExpensesAsync()
    {
        var db = await _databaseService.GetConnectionAsync();
        var entities = await db.Table<ExpenseEntity>()
            .Where(e => e.ContinuesAfterPurchase)
            .OrderBy(e => e.Category)
            .ThenBy(e => e.Name)
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

    public async Task<bool> ExistsAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        var count = await db.Table<ExpenseEntity>()
            .Where(e => e.Id == id)
            .CountAsync();
        return count > 0;
    }

    public async Task<decimal> GetTotalMonthlyExpensesAsync()
    {
        var allExpenses = await GetAllAsync();
        return allExpenses.Sum(e => e.MonthlyAmount);
    }

    public async Task<decimal> GetTotalEssentialMonthlyExpensesAsync()
    {
        var essentialExpenses = await GetEssentialAsync();
        return essentialExpenses.Sum(e => e.MonthlyAmount);
    }

    public async Task<int> GetCountAsync()
    {
        var db = await _databaseService.GetConnectionAsync();
        return await db.Table<ExpenseEntity>().CountAsync();
    }

    public async Task<IReadOnlyDictionary<ExpenseCategory, decimal>> GetMonthlyTotalsByCategoryAsync()
    {
        var allExpenses = await GetAllAsync();
        return allExpenses
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.MonthlyAmount));
    }

    private static Expense MapToModel(ExpenseEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Amount = entity.Amount,
        Frequency = (ExpenseFrequency)entity.Frequency,
        Category = (ExpenseCategory)entity.Category,
        IsEssential = entity.IsEssential,
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
        ContinuesAfterPurchase = model.ContinuesAfterPurchase,
        Notes = model.Notes,
        CreatedAt = model.CreatedAt,
        UpdatedAt = model.UpdatedAt
    };
}
