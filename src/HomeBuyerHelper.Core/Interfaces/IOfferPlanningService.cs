using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Evaluates and compares alternative offer structures for a property.
/// </summary>
public interface IOfferPlanningService
{
    /// <summary>
    /// Evaluates one offer structure against a property: monthly cost,
    /// cash to close, lifetime cost, point breakeven, and a competitiveness
    /// score. Property tax and insurance fall back to the supplied defaults
    /// when the property doesn't specify them.
    /// </summary>
    OfferEvaluation Evaluate(
        OfferScenario offer,
        Property property,
        decimal defaultPropertyTaxRatePercent = 0.96m,
        decimal defaultMonthlyInsurance = 125m);

    /// <summary>
    /// Evaluates a set of offers for the same property, ordered strongest first.
    /// </summary>
    IReadOnlyList<OfferEvaluation> Compare(
        IEnumerable<OfferScenario> offers,
        Property property,
        decimal defaultPropertyTaxRatePercent = 0.96m,
        decimal defaultMonthlyInsurance = 125m);

    /// <summary>
    /// Builds a sensible starting offer for a property from the user's
    /// default loan settings (at asking price, no credits or waivers).
    /// </summary>
    OfferScenario BuildDefaultOffer(Property property, UserPreferences preferences);
}

/// <summary>
/// Persistence for saved offer scenarios.
/// </summary>
public interface IOfferScenarioRepository
{
    /// <summary>Gets an offer scenario by ID.</summary>
    Task<OfferScenario?> GetByIdAsync(int id);

    /// <summary>Gets all offer scenarios for a property, oldest first.</summary>
    Task<IReadOnlyList<OfferScenario>> GetByPropertyIdAsync(int propertyId);

    /// <summary>Gets all offer scenarios.</summary>
    Task<IReadOnlyList<OfferScenario>> GetAllAsync();

    /// <summary>Creates a new offer scenario and returns its ID.</summary>
    Task<int> CreateAsync(OfferScenario offer);

    /// <summary>Updates an existing offer scenario.</summary>
    Task UpdateAsync(OfferScenario offer);

    /// <summary>Deletes an offer scenario.</summary>
    Task DeleteAsync(int id);

    /// <summary>Deletes all offer scenarios for a property.</summary>
    Task DeleteByPropertyIdAsync(int propertyId);
}
