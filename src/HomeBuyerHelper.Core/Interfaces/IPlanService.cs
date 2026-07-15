using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Resolves the user's designated "plan" — one target property plus one offer
/// scenario — into an integrated snapshot the whole app reflects (M4). Budget,
/// Funding, Offers, and the Dashboard all read from this.
/// </summary>
public interface IPlanService
{
    /// <summary>
    /// Resolves the current plan into a <see cref="PlanSnapshot"/>: the offer
    /// evaluation, funding coverage against cash-to-close, and affordability
    /// zones per income scenario. Returns null when no plan is set. If the
    /// target property or offer has been deleted, self-heals by clearing the
    /// dangling ids and returns null.
    /// </summary>
    Task<PlanSnapshot?> GetSnapshotAsync();

    /// <summary>Designates a property + offer scenario as the plan.</summary>
    Task SetPlanAsync(int propertyId, int offerScenarioId);

    /// <summary>Clears the plan.</summary>
    Task ClearPlanAsync();

    /// <summary>Sets (or clears) the expected purchase month.</summary>
    Task SetPurchaseMonthAsync(DateTime? month);
}

/// <summary>
/// The integrated picture of the user's plan: the target property and offer,
/// its full evaluation, funding coverage, and affordability zones.
/// </summary>
public class PlanSnapshot
{
    /// <summary>The target property.</summary>
    public required Property Property { get; init; }

    /// <summary>The chosen offer scenario.</summary>
    public required OfferScenario Offer { get; init; }

    /// <summary>The full offer evaluation (monthly cost, cash to close, etc.).</summary>
    public required OfferEvaluation Evaluation { get; init; }

    /// <summary>Net-of-tax funds available across all funding sources.</summary>
    public decimal FundingNetAvailable { get; init; }

    /// <summary>
    /// Net available minus the offer's cash to close. Negative is a shortfall.
    /// </summary>
    public decimal FundingSurplus { get; init; }

    /// <summary>Expected purchase month, if set.</summary>
    public DateTime? TargetPurchaseMonth { get; init; }

    /// <summary>
    /// Affordability zone / housing percentage under each income scenario
    /// (Conservative, Realistic, Expected), using the offer's total monthly cost.
    /// </summary>
    public IReadOnlyList<AffordabilityAssessment> Affordability { get; init; } =
        Array.Empty<AffordabilityAssessment>();

    /// <summary>The Realistic-scenario assessment, the headline number.</summary>
    public AffordabilityAssessment? RealisticAffordability =>
        Affordability.FirstOrDefault(a => a.Scenario == IncomeScenario.Realistic);
}
