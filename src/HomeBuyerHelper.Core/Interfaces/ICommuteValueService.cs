using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Commute time value analysis (design spec section 2.5, P3-COM-003).
/// </summary>
public interface ICommuteValueService
{
    /// <summary>
    /// Calculates the monetized monthly value of a commute.
    /// </summary>
    /// <param name="roundTripMinutes">Round-trip commute minutes per workday.</param>
    /// <param name="workdaysPerMonth">Working days per month.</param>
    /// <param name="hourlyRate">Value of the user's time per hour.</param>
    decimal CalculateMonthlyValue(int roundTripMinutes, int workdaysPerMonth, decimal hourlyRate);

    /// <summary>
    /// Builds the full commute analysis for a property using the user's
    /// commute configuration. Sums primary and secondary commutes.
    /// </summary>
    CommuteAnalysis Analyze(Property property, UserPreferences preferences);
}

/// <summary>
/// Commute zones based on round-trip length (P3-COM-004).
/// </summary>
public enum CommuteZone
{
    /// <summary>Under 30 minutes round trip.</summary>
    Excellent,

    /// <summary>30-60 minutes round trip.</summary>
    Moderate,

    /// <summary>60-90 minutes round trip.</summary>
    Long,

    /// <summary>Over 90 minutes round trip.</summary>
    VeryLong
}

/// <summary>
/// Commute time value analysis result (spec 2.5.3).
/// </summary>
public class CommuteAnalysis
{
    /// <summary>Total round-trip minutes per workday across commuters.</summary>
    public int TotalRoundTripMinutes { get; init; }

    /// <summary>Monetized commute time cost per month.</summary>
    public decimal MonthlyValue { get; init; }

    /// <summary>Monetized commute time cost per year.</summary>
    public decimal AnnualValue { get; init; }

    /// <summary>Monetized commute time cost over a 30-year mortgage.</summary>
    public decimal ThirtyYearValue { get; init; }

    /// <summary>Hours spent commuting per year.</summary>
    public decimal HoursPerYear { get; init; }

    /// <summary>Equivalent 8-hour days spent commuting per year.</summary>
    public decimal DaysPerYear { get; init; }

    /// <summary>Zone classification for the primary commute.</summary>
    public CommuteZone Zone { get; init; }

    /// <summary>Whether any commute data was entered.</summary>
    public bool HasCommuteData { get; init; }

    /// <summary>Display color for the zone.</summary>
    public string ZoneColor => Zone switch
    {
        CommuteZone.Excellent => "#4CAF50",
        CommuteZone.Moderate => "#FFC107",
        CommuteZone.Long => "#FF9800",
        _ => "#F44336"
    };

    /// <summary>Human-readable zone label.</summary>
    public string ZoneLabel => Zone switch
    {
        CommuteZone.Excellent => "Excellent",
        CommuteZone.Moderate => "Moderate",
        CommuteZone.Long => "Long",
        _ => "Very Long"
    };
}
