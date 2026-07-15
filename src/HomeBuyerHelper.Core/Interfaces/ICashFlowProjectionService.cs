using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Builds month-by-month cash flow projections (design spec sections 3.5/3.6).
/// </summary>
public interface ICashFlowProjectionService
{
    /// <summary>
    /// Projects monthly cash flow including cumulative surplus and emergency
    /// fund balance evolution.
    /// </summary>
    /// <param name="input">Income, expenses, events, and configuration.</param>
    /// <returns>One projection entry per month, in chronological order.</returns>
    IReadOnlyList<MonthlyProjection> Project(CashFlowProjectionInput input);
}

/// <summary>
/// Inputs for a cash flow projection run.
/// </summary>
public class CashFlowProjectionInput
{
    /// <summary>
    /// All configured income sources.
    /// </summary>
    public IReadOnlyList<IncomeSource> IncomeSources { get; init; } = Array.Empty<IncomeSource>();

    /// <summary>
    /// All recurring expenses (fixed and variable).
    /// </summary>
    public IReadOnlyList<Expense> Expenses { get; init; } = Array.Empty<Expense>();

    /// <summary>
    /// One-time events on the budget calendar.
    /// </summary>
    public IReadOnlyList<OneTimeEvent> OneTimeEvents { get; init; } = Array.Empty<OneTimeEvent>();

    /// <summary>
    /// Income scenario to project under.
    /// </summary>
    public IncomeScenario Scenario { get; init; } = IncomeScenario.Realistic;

    /// <summary>
    /// First month of the projection. Defaults to the current month.
    /// </summary>
    public DateTime? StartMonth { get; init; }

    /// <summary>
    /// Number of months to project. Default 24 per the Phase 2 plan.
    /// </summary>
    public int Months { get; init; } = 24;

    /// <summary>
    /// Additional monthly housing cost to layer in (e.g., a candidate
    /// property's estimated cost). Zero when projecting current finances.
    /// </summary>
    public decimal MonthlyHousingCost { get; init; }

    /// <summary>
    /// Emergency fund configuration; null disables fund tracking.
    /// </summary>
    public EmergencyFundConfig? EmergencyFund { get; init; }

    /// <summary>
    /// Optional planned home purchase to model. From its start month onward the
    /// projection adds the housing cost and drops expenses marked
    /// <see cref="Expense.EndsAtPurchase"/> (e.g. current rent). Null projects
    /// current finances unchanged.
    /// </summary>
    public PlannedHousing? PlannedHousing { get; init; }
}

/// <summary>
/// A planned home purchase layered into a cash flow projection: the monthly
/// housing cost that begins at <see cref="StartMonth"/>.
/// </summary>
public class PlannedHousing
{
    /// <summary>Monthly housing cost once the home is owned.</summary>
    public decimal MonthlyCost { get; init; }

    /// <summary>First month the housing cost applies (rounded to the month).</summary>
    public DateTime StartMonth { get; init; }
}
