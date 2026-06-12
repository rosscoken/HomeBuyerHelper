using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// True Total Cost = housing + utilities + commute time value
/// (design spec section 2.6, P3-TTC-001).
/// </summary>
public interface ITrueTotalCostService
{
    /// <summary>
    /// Calculates the complete monthly and 30-year cost picture for a property.
    /// </summary>
    TrueTotalCost Calculate(Property property, UserPreferences preferences);
}

/// <summary>
/// True total cost breakdown for a property.
/// </summary>
public class TrueTotalCost
{
    /// <summary>Monthly housing cost (P&amp;I, taxes, insurance, HOA, PMI).</summary>
    public decimal MonthlyHousing { get; init; }

    /// <summary>Monthly utilities estimate.</summary>
    public decimal MonthlyUtilities { get; init; }

    /// <summary>Monetized monthly commute time value.</summary>
    public decimal MonthlyCommuteValue { get; init; }

    /// <summary>Complete monthly cost of choosing this property.</summary>
    public decimal MonthlyTotal => MonthlyHousing + MonthlyUtilities + MonthlyCommuteValue;

    /// <summary>30-year total (mortgage lifetime).</summary>
    public decimal ThirtyYearTotal => MonthlyTotal * 12 * 30;

    /// <summary>The commute analysis backing the commute component.</summary>
    public CommuteAnalysis? Commute { get; init; }
}
