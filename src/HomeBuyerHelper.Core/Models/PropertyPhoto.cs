namespace HomeBuyerHelper.Core.Models;

/// <summary>
/// A photo attached to a property (P3-PHO-001).
/// </summary>
public class PropertyPhoto
{
    public int Id { get; set; }

    /// <summary>
    /// The property this photo belongs to.
    /// </summary>
    public int PropertyId { get; set; }

    /// <summary>
    /// Path to the stored image file in the app's local data folder.
    /// </summary>
    public required string FilePath { get; set; }

    /// <summary>
    /// Optional caption for the photo.
    /// </summary>
    public string? Caption { get; set; }

    /// <summary>
    /// Display order within the property's gallery.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// When the photo was added.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
