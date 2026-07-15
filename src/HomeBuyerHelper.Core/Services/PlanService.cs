using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Services;

/// <summary>
/// Resolves the user's designated plan (one property + one offer scenario)
/// into an integrated <see cref="PlanSnapshot"/> and manages the underlying
/// preference ids. Tolerates dangling ids by self-healing (see
/// <see cref="GetSnapshotAsync"/>).
/// </summary>
public class PlanService : IPlanService
{
    private readonly IUserPreferencesRepository _preferences;
    private readonly IPropertyRepository _properties;
    private readonly IOfferScenarioRepository _offers;
    private readonly IFundingRepository _funding;
    private readonly IIncomeRepository _income;
    private readonly IOfferPlanningService _offerPlanning;
    private readonly ITaxImpactService _taxImpact;
    private readonly IAffordabilityService _affordability;

    public PlanService(
        IUserPreferencesRepository preferences,
        IPropertyRepository properties,
        IOfferScenarioRepository offers,
        IFundingRepository funding,
        IIncomeRepository income,
        IOfferPlanningService offerPlanning,
        ITaxImpactService taxImpact,
        IAffordabilityService affordability)
    {
        _preferences = preferences;
        _properties = properties;
        _offers = offers;
        _funding = funding;
        _income = income;
        _offerPlanning = offerPlanning;
        _taxImpact = taxImpact;
        _affordability = affordability;
    }

    public async Task<PlanSnapshot?> GetSnapshotAsync()
    {
        var preferences = await _preferences.GetAsync();

        if (preferences.TargetPropertyId is not int propertyId ||
            preferences.TargetOfferScenarioId is not int offerId)
        {
            return null;
        }

        var property = await _properties.GetByIdAsync(propertyId);
        var offer = await _offers.GetByIdAsync(offerId);

        // Self-heal dangling ids: property/offer deleted, or offer no longer
        // belongs to the target property.
        if (property == null || offer == null || offer.PropertyId != property.Id)
        {
            preferences.TargetPropertyId = null;
            preferences.TargetOfferScenarioId = null;
            await _preferences.SaveAsync(preferences);
            return null;
        }

        var evaluation = _offerPlanning.Evaluate(
            offer,
            property,
            preferences.DefaultPropertyTaxRate,
            preferences.DefaultMonthlyInsurance);

        var fundingSources = await _funding.GetAllAsync();
        var fundingPlan = _taxImpact.BuildPlan(fundingSources, preferences);
        var netAvailable = fundingPlan.TotalNet;

        var incomeSources = await _income.GetAllAsync();
        var affordability = _affordability.AssessAllScenarios(
            evaluation.TotalMonthlyCost, incomeSources);

        return new PlanSnapshot
        {
            Property = property,
            Offer = offer,
            Evaluation = evaluation,
            FundingNetAvailable = netAvailable,
            FundingSurplus = netAvailable - evaluation.CashToClose,
            TargetPurchaseMonth = preferences.TargetPurchaseMonth,
            Affordability = affordability
        };
    }

    public async Task SetPlanAsync(int propertyId, int offerScenarioId)
    {
        var preferences = await _preferences.GetAsync();
        preferences.TargetPropertyId = propertyId;
        preferences.TargetOfferScenarioId = offerScenarioId;
        await _preferences.SaveAsync(preferences);
    }

    public async Task ClearPlanAsync()
    {
        var preferences = await _preferences.GetAsync();
        preferences.TargetPropertyId = null;
        preferences.TargetOfferScenarioId = null;
        await _preferences.SaveAsync(preferences);
    }

    public async Task SetPurchaseMonthAsync(DateTime? month)
    {
        var preferences = await _preferences.GetAsync();
        preferences.TargetPurchaseMonth = month;
        await _preferences.SaveAsync(preferences);
    }
}
