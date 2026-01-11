using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Repository interface for Expense CRUD operations.
/// </summary>
public interface IExpenseRepository
{
    /// <summary>
    /// Gets an expense by its ID.
    /// </summary>
    Task<Expense?> GetByIdAsync(int id);

    /// <summary>
    /// Gets all expenses.
    /// </summary>
    Task<IReadOnlyList<Expense>> GetAllAsync();

    /// <summary>
    /// Gets expenses by category.
    /// </summary>
    Task<IReadOnlyList<Expense>> GetByCategoryAsync(ExpenseCategory category);

    /// <summary>
    /// Gets only essential expenses.
    /// </summary>
    Task<IReadOnlyList<Expense>> GetEssentialAsync();

    /// <summary>
    /// Gets expenses that will continue after home purchase.
    /// </summary>
    Task<IReadOnlyList<Expense>> GetContinuingExpensesAsync();

    /// <summary>
    /// Creates a new expense.
    /// </summary>
    /// <returns>The ID of the created expense.</returns>
    Task<int> CreateAsync(Expense expense);

    /// <summary>
    /// Updates an existing expense.
    /// </summary>
    Task UpdateAsync(Expense expense);

    /// <summary>
    /// Deletes an expense by its ID.
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Checks if an expense exists.
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Gets the total monthly expenses.
    /// </summary>
    Task<decimal> GetTotalMonthlyExpensesAsync();

    /// <summary>
    /// Gets the total monthly essential expenses.
    /// </summary>
    Task<decimal> GetTotalEssentialMonthlyExpensesAsync();

    /// <summary>
    /// Gets the count of all expenses.
    /// </summary>
    Task<int> GetCountAsync();

    /// <summary>
    /// Gets expenses grouped by category with monthly totals.
    /// </summary>
    Task<IReadOnlyDictionary<ExpenseCategory, decimal>> GetMonthlyTotalsByCategoryAsync();
}
