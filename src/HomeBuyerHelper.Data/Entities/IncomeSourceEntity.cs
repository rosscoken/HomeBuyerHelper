using SQLite;

namespace HomeBuyerHelper.Data.Entities;

/// <summary>
/// SQLite entity for IncomeSources table.
/// </summary>
[Table("IncomeSources")]
public class IncomeSourceEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public decimal GrossAmount { get; set; }

    public decimal? NetAmount { get; set; }

    public int Frequency { get; set; }

    public int IncomeType { get; set; }

    public bool IsReliable { get; set; }

    public int? PaymentMonth { get; set; }

    public decimal Probability { get; set; } = 100m;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
