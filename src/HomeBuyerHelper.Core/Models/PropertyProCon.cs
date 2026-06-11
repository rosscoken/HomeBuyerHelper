namespace HomeBuyerHelper.Core.Models;

/// <summary>
/// A pro or con item for a property (P3-NOT-002).
/// </summary>
public class PropertyProCon
{
    public int Id { get; set; }

    /// <summary>
    /// The property this item belongs to.
    /// </summary>
    public int PropertyId { get; set; }

    /// <summary>
    /// True for a pro, false for a con.
    /// </summary>
    public bool IsPro { get; set; }

    /// <summary>
    /// The pro/con text.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Display order within the list.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// When the item was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
