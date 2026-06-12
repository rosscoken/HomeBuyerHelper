using SQLite;

namespace HomeBuyerHelper.Data.Entities;

/// <summary>
/// SQLite entity for OneTimeEvents table.
/// </summary>
[Table("OneTimeEvents")]
public class OneTimeEventEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public int Category { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
