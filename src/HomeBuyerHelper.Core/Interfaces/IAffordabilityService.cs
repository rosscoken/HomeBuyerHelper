using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Housing affordability analysis (design spec section 3.4).
/// </summary>
public interface IAffordabilityService
{
    /// <summary>
    /// Calculates housing cost as a percentage of gross monthly income.
    /// </summary>
    decimal CalculateHousingPercentage(decimal monthlyHousingCost, decimal grossMonthlyIncome);

    /// <summary>
    /// Maps a housing percentage to its affordability zone.
    /// </summary>
    AffordabilityZone GetZone(decimal housingPercentage);

    /// <summary>
    /// Assesses a housing cost against income sources for all three scenarios.
    /// </summary>
    IReadOnlyList<AffordabilityAssessment> AssessAllScenarios(
        decimal monthlyHousingCost,
        IEnumerable<IncomeSource> incomeSources);
}

/// <summary>
/// Affordability zones per design spec section 3.4.2.
/// </summary>
public enum AffordabilityZone
{
    /// <summary>Below 28% of gross income.</summary>
    Comfortable,

    /// <summary>28-36% of gross income.</summary>
    Stretching,

    /// <summary>36-43% of gross income.</summary>
    Aggressive,

    /// <summary>Above 43% of gross income.</summary>
    Risky
}

/// <summary>
/// Affordability result for one income scenario.
/// </summary>
public class AffordabilityAssessment
{
    public IncomeScenario Scenario { get; init; }
    public decimal GrossMonthlyIncome { get; init; }
    public decimal MonthlyHousingCost { get; init; }
    public decimal HousingPercentage { get; init; }
    public AffordabilityZone Zone { get; init; }

    /// <summary>
    /// Display color for the zone (green/yellow/orange/red).
    /// </summary>
    public string ZoneColor => Zone switch
    {
        AffordabilityZone.Comfortable => "#4CAF50",
        AffordabilityZone.Stretching => "#FFC107",
        AffordabilityZone.Aggressive => "#FF9800",
        _ => "#F44336"
    };

    /// <summary>
    /// Human-readable zone label.
    /// </summary>
    public string ZoneLabel => Zone switch
    {
        AffordabilityZone.Comfortable => "Comfortable",
        AffordabilityZone.Stretching => "Stretching",
        AffordabilityZone.Aggressive => "Aggressive",
        _ => "Risky"
    };
}
