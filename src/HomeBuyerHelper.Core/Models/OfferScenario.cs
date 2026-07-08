namespace HomeBuyerHelper.Core.Models;

/// <summary>
/// A named offer structure for a specific property. Captures the levers a
/// buyer can pull when shaping an offer — price, escalation, financing,
/// rate buydown points, credits, earnest money, and contingencies — so
/// alternatives can be compared side by side.
/// </summary>
public class OfferScenario
{
    public int Id { get; set; }

    /// <summary>
    /// The property this offer is for.
    /// </summary>
    public int PropertyId { get; set; }

    /// <summary>
    /// Short label for the structure (e.g., "Aggressive cash-heavy", "Full ask + credits").
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Offer price presented to the seller.
    /// </summary>
    public decimal OfferPrice { get; set; }

    /// <summary>
    /// Maximum price under an escalation clause, if the offer includes one.
    /// </summary>
    public decimal? EscalationMaxPrice { get; set; }

    /// <summary>
    /// Down payment as a percent of the offer price. 100 models an all-cash offer.
    /// </summary>
    public decimal DownPaymentPercent { get; set; } = 20m;

    /// <summary>
    /// Quoted mortgage rate percent before any point buydown.
    /// </summary>
    public decimal InterestRate { get; set; } = 7.0m;

    /// <summary>
    /// Mortgage term in years.
    /// </summary>
    public int TermYears { get; set; } = 30;

    /// <summary>
    /// Discount points purchased at closing. Each point costs 1% of the loan
    /// and lowers the rate (see OfferPlanningService.RateReductionPerPoint).
    /// </summary>
    public decimal DiscountPoints { get; set; }

    /// <summary>
    /// Seller credit toward closing costs, negotiated as part of the offer.
    /// </summary>
    public decimal SellerCredit { get; set; }

    /// <summary>
    /// Lender or broker credit toward closing costs.
    /// </summary>
    public decimal LenderCredit { get; set; }

    /// <summary>
    /// Earnest money deposit committed with the offer. Credited toward the
    /// down payment at closing, but at risk if the buyer walks without a
    /// contingency to lean on.
    /// </summary>
    public decimal EarnestMoney { get; set; }

    /// <summary>
    /// Whether the offer waives the inspection contingency.
    /// </summary>
    public bool WaiveInspection { get; set; }

    /// <summary>
    /// Whether the offer waives the financing contingency.
    /// </summary>
    public bool WaiveFinancing { get; set; }

    /// <summary>
    /// Whether the offer waives the appraisal contingency.
    /// </summary>
    public bool WaiveAppraisal { get; set; }

    /// <summary>
    /// Days to close offered to the seller. Shorter closes read stronger.
    /// </summary>
    public int ClosingDays { get; set; } = 30;

    /// <summary>
    /// Free-form notes (negotiation context, agent advice, etc.).
    /// </summary>
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Whether this offer is financed (anything under a 100% down payment).
    /// </summary>
    public bool IsFinanced => DownPaymentPercent < 100m;
}

/// <summary>
/// Evaluated financial picture of one offer structure.
/// </summary>
public class OfferEvaluation
{
    public required OfferScenario Offer { get; init; }

    // --- Purchase structure ---

    /// <summary>Down payment in dollars (before earnest money is credited).</summary>
    public decimal DownPayment { get; init; }

    /// <summary>Amount financed.</summary>
    public decimal LoanAmount { get; init; }

    /// <summary>Rate after the discount point buydown.</summary>
    public decimal EffectiveInterestRate { get; init; }

    // --- Monthly costs ---

    public decimal MonthlyPrincipalAndInterest { get; init; }
    public decimal MonthlyPMI { get; init; }
    public decimal MonthlyPropertyTax { get; init; }
    public decimal MonthlyInsurance { get; init; }
    public decimal MonthlyHOA { get; init; }

    public decimal TotalMonthlyCost =>
        MonthlyPrincipalAndInterest + MonthlyPMI + MonthlyPropertyTax + MonthlyInsurance + MonthlyHOA;

    // --- Cash needed ---

    /// <summary>Estimated closing costs before credits and points.</summary>
    public decimal EstimatedClosingCosts { get; init; }

    /// <summary>Cost of the discount points purchased.</summary>
    public decimal PointsCost { get; init; }

    /// <summary>Seller credit actually usable after financing concession caps.</summary>
    public decimal SellerCreditApplied { get; init; }

    /// <summary>Whether the requested seller credit was reduced by concession caps.</summary>
    public bool SellerCreditWasCapped { get; init; }

    /// <summary>Lender credit applied.</summary>
    public decimal LenderCreditApplied { get; init; }

    /// <summary>Total cash due at closing: down payment + closing costs + points − credits.</summary>
    public decimal CashToClose { get; init; }

    // --- Lifetime costs ---

    /// <summary>Interest paid over the full term at the effective rate.</summary>
    public decimal TotalInterest { get; init; }

    /// <summary>Cash to close + all P&I payments over the term.</summary>
    public decimal TotalCostOfPurchase { get; init; }

    // --- Points analysis ---

    /// <summary>
    /// Months for the point buydown to pay for itself via the lower payment.
    /// Null when no points are purchased or they don't reduce the payment.
    /// </summary>
    public int? PointsBreakevenMonths { get; init; }

    // --- Competitiveness vs asking ---

    /// <summary>Offer price minus asking price (negative = below ask).</summary>
    public decimal VsAskingAmount { get; init; }

    /// <summary>Offer price as a percent above/below asking.</summary>
    public decimal VsAskingPercent { get; init; }

    /// <summary>
    /// Heuristic 0–100 competitiveness score from the seller's point of view.
    /// </summary>
    public int StrengthScore { get; init; }

    /// <summary>Label bucket for the strength score.</summary>
    public string StrengthLabel =>
        StrengthScore >= 80 ? "Very Strong" :
        StrengthScore >= 60 ? "Strong" :
        StrengthScore >= 40 ? "Competitive" :
        StrengthScore >= 20 ? "Modest" :
        "Weak";
}
