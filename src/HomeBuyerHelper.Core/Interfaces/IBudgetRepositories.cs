using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Repository interface for income source operations.
/// </summary>
public interface IIncomeRepository
{
    /// <summary>Gets an income source by ID.</summary>
    Task<IncomeSource?> GetByIdAsync(int id);

    /// <summary>Gets all income sources.</summary>
    Task<IReadOnlyList<IncomeSource>> GetAllAsync();

    /// <summary>Creates a new income source and returns its ID.</summary>
    Task<int> CreateAsync(IncomeSource source);

    /// <summary>Updates an existing income source.</summary>
    Task UpdateAsync(IncomeSource source);

    /// <summary>Deletes an income source.</summary>
    Task DeleteAsync(int id);
}

/// <summary>
/// Repository interface for expense operations.
/// </summary>
public interface IExpenseRepository
{
    /// <summary>Gets an expense by ID.</summary>
    Task<Expense?> GetByIdAsync(int id);

    /// <summary>Gets all expenses.</summary>
    Task<IReadOnlyList<Expense>> GetAllAsync();

    /// <summary>Creates a new expense and returns its ID.</summary>
    Task<int> CreateAsync(Expense expense);

    /// <summary>Updates an existing expense.</summary>
    Task UpdateAsync(Expense expense);

    /// <summary>Deletes an expense.</summary>
    Task DeleteAsync(int id);
}

/// <summary>
/// Repository interface for one-time budget event operations.
/// </summary>
public interface IOneTimeEventRepository
{
    /// <summary>Gets an event by ID.</summary>
    Task<OneTimeEvent?> GetByIdAsync(int id);

    /// <summary>Gets all events ordered by date.</summary>
    Task<IReadOnlyList<OneTimeEvent>> GetAllAsync();

    /// <summary>Creates a new event and returns its ID.</summary>
    Task<int> CreateAsync(OneTimeEvent oneTimeEvent);

    /// <summary>Updates an existing event.</summary>
    Task UpdateAsync(OneTimeEvent oneTimeEvent);

    /// <summary>Deletes an event.</summary>
    Task DeleteAsync(int id);
}
