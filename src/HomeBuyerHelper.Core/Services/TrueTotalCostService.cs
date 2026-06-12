using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Services;

/// <summary>
/// Combines housing costs, utilities, and commute time value into the
/// complete cost of choosing a property (design spec section 2.6).
/// </summary>
public class TrueTotalCostService : ITrueTotalCostService
{
    private readonly ICalculationService _calculationService;
    private readonly ICommuteValueService _commuteValueService;

    public TrueTotalCostService(
        ICalculationService calculationService,
        ICommuteValueService commuteValueService)
    {
        _calculationService = calculationService;
        _commuteValueService = commuteValueService;
    }

    public TrueTotalCost Calculate(Property property, UserPreferences preferences)
    {
        var price = property.EffectivePrice;
        var annualTax = property.AnnualPropertyTax
            ?? price * preferences.DefaultPropertyTaxRate / 100;
        var annualInsurance = property.AnnualInsurance
            ?? preferences.DefaultMonthlyInsurance * 12;

        var housing = price > 0
            ? _calculationService.CalculateMonthlyHousingCost(
                price,
                preferences.DefaultDownPaymentPercent,
                preferences.DefaultInterestRate,
                preferences.DefaultMortgageTerm,
                annualTax,
                annualInsurance,
                property.MonthlyHOA).Total
            : 0;

        var commute = _commuteValueService.Analyze(property, preferences);

        return new TrueTotalCost
        {
            MonthlyHousing = housing,
            MonthlyUtilities = property.MonthlyUtilities,
            MonthlyCommuteValue = commute.MonthlyValue,
            Commute = commute
        };
    }
}
