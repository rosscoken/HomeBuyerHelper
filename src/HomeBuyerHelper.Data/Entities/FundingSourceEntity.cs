using SQLite;

namespace HomeBuyerHelper.Data.Entities;

/// <summary>
/// SQLite entity for FundingSources table.
/// </summary>
[Table("FundingSources")]
public class FundingSourceEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public decimal CurrentAmount { get; set; }

    public decimal? ExpectedAmount { get; set; }

    public int FundingType { get; set; }

    public bool IsLiquid { get; set; }

    public bool IsDocumented { get; set; }

    public decimal MonthlyContribution { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
