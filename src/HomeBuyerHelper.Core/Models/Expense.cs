namespace HomeBuyerHelper.Core.Models;

/// <summary>
/// Represents a recurring expense for budget planning.
/// </summary>
public class Expense
{
    public int Id { get; set; }

    /// <summary>
    /// Name of the expense (e.g., "Car Payment", "Groceries").
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Amount of the expense.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Frequency of the expense.
    /// </summary>
    public ExpenseFrequency Frequency { get; set; } = ExpenseFrequency.Monthly;

    /// <summary>
    /// Category of the expense.
    /// </summary>
    public ExpenseCategory Category { get; set; } = ExpenseCategory.Other;

    /// <summary>
    /// Whether this expense is essential/non-discretionary.
    /// </summary>
    public bool IsEssential { get; set; } = true;

    /// <summary>
    /// Whether this expense will continue after home purchase.
    /// </summary>
    public bool ContinuesAfterPurchase { get; set; } = true;

    /// <summary>
    /// Optional notes about this expense.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// When the expense was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the expense was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Calculates the monthly expense amount.
    /// </summary>
    public decimal MonthlyAmount => Frequency switch
    {
        ExpenseFrequency.Weekly => Amount * 52 / 12,
        ExpenseFrequency.BiWeekly => Amount * 26 / 12,
        ExpenseFrequency.Monthly => Amount,
        ExpenseFrequency.Quarterly => Amount / 3,
        ExpenseFrequency.SemiAnnually => Amount / 6,
        ExpenseFrequency.Annually => Amount / 12,
        _ => Amount
    };

    /// <summary>
    /// Calculates the annual expense amount.
    /// </summary>
    public decimal AnnualAmount => MonthlyAmount * 12;
}

/// <summary>
/// Frequency options for expenses.
/// </summary>
public enum ExpenseFrequency
{
    Weekly,
    BiWeekly,
    Monthly,
    Quarterly,
    SemiAnnually,
    Annually
}

/// <summary>
/// Categories for grouping expenses.
/// </summary>
public enum ExpenseCategory
{
    Housing,
    Transportation,
    Food,
    Utilities,
    Insurance,
    Healthcare,
    DebtPayments,
    Entertainment,
    PersonalCare,
    Clothing,
    Education,
    Savings,
    Subscriptions,
    Childcare,
    PetCare,
    Other
}
