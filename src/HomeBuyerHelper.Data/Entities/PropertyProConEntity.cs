using SQLite;

namespace HomeBuyerHelper.Data.Entities;

/// <summary>
/// SQLite entity for PropertyProsCons table.
/// </summary>
[Table("PropertyProsCons")]
public class PropertyProConEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int PropertyId { get; set; }

    public bool IsPro { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
}
