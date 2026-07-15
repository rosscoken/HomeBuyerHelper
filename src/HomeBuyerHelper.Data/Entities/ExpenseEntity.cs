using SQLite;

namespace HomeBuyerHelper.Data.Entities;

/// <summary>
/// SQLite entity for Expenses table.
/// </summary>
[Table("Expenses")]
public class ExpenseEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public int Frequency { get; set; }

    public int Category { get; set; }

    public bool IsEssential { get; set; }

    public bool IsVariable { get; set; }

    public bool ContinuesAfterPurchase { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
