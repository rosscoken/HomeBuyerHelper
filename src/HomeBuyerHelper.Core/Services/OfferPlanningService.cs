using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Services;

/// <summary>
/// Evaluates alternative offer structures for a property: what each one
/// costs per month, what it takes to close, and how competitive it reads
/// from the seller's side.
/// </summary>
public class OfferPlanningService : IOfferPlanningService
{
    /// <summary>
    /// Rate reduction per discount point purchased. Industry rule of thumb;
    /// each point costs 1% of the loan amount.
    /// </summary>
    public const decimal RateReductionPerPoint = 0.25m;

    /// <summary>
    /// Annual PMI rate applied to the loan when down payment is under 20%.
    /// Mirrors CalculationService.CalculateMonthlyHousingCost.
    /// </summary>
    public const decimal AnnualPmiRate = 0.005m;

    private readonly ICalculationService _calculationService;

    public OfferPlanningService(ICalculationService calculationService)
    {
        _calculationService = calculationService;
    }

    public OfferEvaluation Evaluate(
        OfferScenario offer,
        Property property,
        decimal defaultPropertyTaxRatePercent = 0.96m,
        decimal defaultMonthlyInsurance = 125m)
    {
        var price = Math.Max(0, offer.OfferPrice);
        var downPaymentPercent = Math.Clamp(offer.DownPaymentPercent, 0m, 100m);
        var downPayment = Math.Round(price * downPaymentPercent / 100, 2);
        var loanAmount = Math.Max(0, price - downPayment);
        var isFinanced = loanAmount > 0;

        var effectiveRate = Math.Max(0, offer.InterestRate - offer.DiscountPoints * RateReductionPerPoint);
        var monthlyPI = _calculationService.CalculateMonthlyMortgagePayment(
            loanAmount, effectiveRate, offer.TermYears);

        var monthlyPMI = isFinanced && downPaymentPercent < 20
            ? Math.Round(loanAmount * AnnualPmiRate / 12, 2)
            : 0;

        var monthlyTax = property.AnnualPropertyTax is decimal tax
            ? Math.Round(tax / 12, 2)
            : Math.Round(price * defaultPropertyTaxRatePercent / 100 / 12, 2);
        var monthlyInsurance = property.AnnualInsurance is decimal insurance
            ? Math.Round(insurance / 12, 2)
            : defaultMonthlyInsurance;

        var closingCosts = EstimateClosingCosts(price, isFinanced);
        var pointsCost = isFinanced ? Math.Round(loanAmount * offer.DiscountPoints / 100, 2) : 0;
        var totalSettlementCosts = closingCosts + pointsCost;

        // Conventional loans cap seller concessions by down payment tier;
        // beyond the cap (or beyond actual costs) a credit is just money the
        // seller keeps, so it can't reduce cash to close.
        var concessionCap = isFinanced
            ? Math.Round(price * SellerConcessionCapPercent(downPaymentPercent) / 100, 2)
            : decimal.MaxValue;
        var lenderCreditApplied = Math.Min(Math.Max(0, offer.LenderCredit), totalSettlementCosts);
        var sellerCreditApplied = Math.Min(
            Math.Min(Math.Max(0, offer.SellerCredit), concessionCap),
            totalSettlementCosts - lenderCreditApplied);

        var cashToClose = Math.Round(
            downPayment + totalSettlementCosts - sellerCreditApplied - lenderCreditApplied, 2);

        var totalPayments = monthlyPI * offer.TermYears * 12;
        var totalInterest = Math.Max(0, Math.Round(totalPayments - loanAmount, 0));

        return new OfferEvaluation
        {
            Offer = offer,
            DownPayment = downPayment,
            LoanAmount = loanAmount,
            EffectiveInterestRate = effectiveRate,
            MonthlyPrincipalAndInterest = monthlyPI,
            MonthlyPMI = monthlyPMI,
            MonthlyPropertyTax = monthlyTax,
            MonthlyInsurance = monthlyInsurance,
            MonthlyHOA = property.MonthlyHOA,
            EstimatedClosingCosts = closingCosts,
            PointsCost = pointsCost,
            SellerCreditApplied = sellerCreditApplied,
            SellerCreditWasCapped = sellerCreditApplied < offer.SellerCredit,
            LenderCreditApplied = lenderCreditApplied,
            CashToClose = cashToClose,
            TotalInterest = totalInterest,
            TotalCostOfPurchase = Math.Round(cashToClose + totalPayments, 0),
            PointsBreakevenMonths = CalculatePointsBreakeven(
                offer, loanAmount, pointsCost, monthlyPI),
            VsAskingAmount = price - property.AskingPrice,
            VsAskingPercent = property.AskingPrice > 0
                ? Math.Round((price - property.AskingPrice) / property.AskingPrice * 100, 1)
                : 0,
            StrengthScore = CalculateStrengthScore(offer, property, isFinanced)
        };
    }

    public IReadOnlyList<OfferEvaluation> Compare(
        IEnumerable<OfferScenario> offers,
        Property property,
        decimal defaultPropertyTaxRatePercent = 0.96m,
        decimal defaultMonthlyInsurance = 125m)
    {
        return offers
            .Select(o => Evaluate(o, property, defaultPropertyTaxRatePercent, defaultMonthlyInsurance))
            .OrderByDescending(e => e.StrengthScore)
            .ThenBy(e => e.CashToClose)
            .ToList();
    }

    public OfferScenario BuildDefaultOffer(Property property, UserPreferences preferences)
    {
        return new OfferScenario
        {
            PropertyId = property.Id,
            Name = "Asking price",
            OfferPrice = property.AskingPrice,
            DownPaymentPercent = preferences.DefaultDownPaymentPercent,
            InterestRate = preferences.DefaultInterestRate,
            TermYears = preferences.DefaultMortgageTerm,
            EarnestMoney = Math.Round(property.AskingPrice * 0.01m, 0)
        };
    }

    private decimal EstimateClosingCosts(decimal price, bool isFinanced)
    {
        if (price <= 0)
        {
            return 0;
        }

        var estimate = _calculationService.CalculateClosingCosts(price);

        // Cash purchases skip the loan-specific charges.
        return isFinanced
            ? estimate.Total
            : estimate.Total - estimate.LoanOriginationFee - estimate.PrepaidInterest;
    }

    /// <summary>
    /// Conventional-financing seller concession caps by down payment tier:
    /// 3% under 10% down, 6% under 25% down, 9% at 25%+ down.
    /// </summary>
    private static decimal SellerConcessionCapPercent(decimal downPaymentPercent) =>
        downPaymentPercent < 10 ? 3m :
        downPaymentPercent < 25 ? 6m :
        9m;

    private int? CalculatePointsBreakeven(
        OfferScenario offer, decimal loanAmount, decimal pointsCost, decimal monthlyPIWithPoints)
    {
        if (pointsCost <= 0 || loanAmount <= 0)
        {
            return null;
        }

        var paymentWithoutPoints = _calculationService.CalculateMonthlyMortgagePayment(
            loanAmount, offer.InterestRate, offer.TermYears);
        var monthlySavings = paymentWithoutPoints - monthlyPIWithPoints;

        return monthlySavings > 0
            ? (int)Math.Ceiling(pointsCost / monthlySavings)
            : null;
    }

    /// <summary>
    /// Heuristic competitiveness score (0–100) from the seller's perspective:
    /// up to 45 for price (escalation-aware), 20 for financing certainty,
    /// 24 for waived contingencies, 6 for earnest money, 5 for a fast close,
    /// minus a penalty for requested seller credits.
    /// </summary>
    private static int CalculateStrengthScore(OfferScenario offer, Property property, bool isFinanced)
    {
        var score = 0m;
        var price = Math.Max(0, offer.OfferPrice);
        var topPrice = Math.Max(price, offer.EscalationMaxPrice ?? 0);

        // Price vs asking: 95% of ask or below scores 0, ask scores 30,
        // 105%+ scores the full 45.
        if (property.AskingPrice > 0)
        {
            var ratio = topPrice / property.AskingPrice * 100;
            score += ratio switch
            {
                <= 95 => 0,
                <= 100 => (ratio - 95) / 5 * 30,
                <= 105 => 30 + (ratio - 100) / 5 * 15,
                _ => 45
            };
        }
        else
        {
            score += 30; // No asking price to compare against — neutral.
        }

        // Financing certainty.
        if (!isFinanced)
        {
            score += 20;
        }
        else
        {
            score += offer.DownPaymentPercent switch
            {
                >= 50 => 15,
                >= 25 => 12,
                >= 20 => 10,
                >= 10 => 6,
                _ => 3
            };
        }

        // Contingencies: 8 points each. Cash offers have no financing
        // contingency by definition.
        if (offer.WaiveInspection) score += 8;
        if (offer.WaiveFinancing || !isFinanced) score += 8;
        if (offer.WaiveAppraisal) score += 8;

        // Earnest money as percent of price.
        if (price > 0)
        {
            var earnestPercent = offer.EarnestMoney / price * 100;
            score += earnestPercent switch
            {
                >= 3 => 6,
                >= 2 => 4,
                >= 1 => 2,
                _ => 0
            };
        }

        // Close speed.
        score += offer.ClosingDays switch
        {
            <= 21 => 5,
            <= 30 => 3,
            <= 45 => 1,
            _ => 0
        };

        // Asking for seller credits cuts the seller's net proceeds.
        if (price > 0 && offer.SellerCredit > 0)
        {
            score -= Math.Min(10, offer.SellerCredit / price * 100 * 3);
        }

        return (int)Math.Clamp(Math.Round(score), 0, 100);
    }
}
