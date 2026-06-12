using SQLite;

namespace HomeBuyerHelper.Data.Entities;

/// <summary>
/// SQLite entity for PropertyPhotos table.
/// </summary>
[Table("PropertyPhotos")]
public class PropertyPhotoEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int PropertyId { get; set; }

    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Caption { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
}
