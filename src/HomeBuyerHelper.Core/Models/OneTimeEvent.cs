namespace HomeBuyerHelper.Core.Models;

/// <summary>
/// A one-time expense event on the budget calendar (moving costs, trips, etc.).
/// </summary>
public class OneTimeEvent
{
    public int Id { get; set; }

    /// <summary>
    /// Name of the event (e.g., "Moving Costs", "Hawaii Trip").
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Expected cost of the event.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// The month the expense occurs (day component ignored).
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Type of one-time event.
    /// </summary>
    public OneTimeEventCategory Category { get; set; } = OneTimeEventCategory.Other;

    /// <summary>
    /// Optional notes about the event.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// When the event was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the event was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Categories for one-time budget events.
/// </summary>
public enum OneTimeEventCategory
{
    Moving,
    Travel,
    Purchase,
    Medical,
    HomeRepair,
    Taxes,
    Other
}
