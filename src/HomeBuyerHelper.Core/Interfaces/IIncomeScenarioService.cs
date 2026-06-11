using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Calculates monthly income under the three planning scenarios
/// (design spec section 3.2).
/// </summary>
public interface IIncomeScenarioService
{
    /// <summary>
    /// Gets the amount a single income source contributes in a specific
    /// calendar month under a scenario, honoring payment timing
    /// (bonus month, quarterly vest cycle) and start/end dates.
    /// </summary>
    decimal GetAmountForMonth(IncomeSource source, DateTime month, IncomeScenario scenario);

    /// <summary>
    /// Gets the total income across all sources for a specific month.
    /// </summary>
    decimal GetTotalForMonth(IEnumerable<IncomeSource> sources, DateTime month, IncomeScenario scenario);

    /// <summary>
    /// Gets the smoothed average monthly income (annual total / 12) under a
    /// scenario. Used for affordability ratios where lumpy income
    /// (bonuses, vests) should be annualized.
    /// </summary>
    decimal GetAverageMonthlyIncome(IEnumerable<IncomeSource> sources, IncomeScenario scenario);

    /// <summary>
    /// Calculates the net cash value of a single RSU vest after tax withholding.
    /// </summary>
    decimal CalculateRsuNetPerVest(decimal sharesPerVest, decimal estimatedSharePrice, decimal taxWithholdingPercent);
}
