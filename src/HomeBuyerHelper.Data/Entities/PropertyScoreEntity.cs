using SQLite;

namespace HomeBuyerHelper.Data.Entities;

/// <summary>
/// SQLite entity for PropertyScores table.
/// </summary>
[Table("PropertyScores")]
public class PropertyScoreEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int PropertyId { get; set; }

    [Indexed]
    public int CriterionId { get; set; }

    public int Score { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
