using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Tax impact analysis for down payment funding sources
/// (design spec section 4.2, P3-TAX-001).
/// </summary>
public interface ITaxImpactService
{
    /// <summary>
    /// Gets the user's marginal federal income tax rate (percent) from their
    /// filing status and estimated taxable income.
    /// </summary>
    decimal GetMarginalFederalRate(TaxFilingStatus filingStatus, decimal taxableIncome);

    /// <summary>
    /// Gets the long-term capital gains rate (percent) for the user's income.
    /// </summary>
    decimal GetLongTermCapitalGainsRate(TaxFilingStatus filingStatus, decimal taxableIncome);

    /// <summary>
    /// Calculates the estimated tax for a single funding source.
    /// </summary>
    FundingSourceTaxResult CalculateTax(FundingSource source, UserPreferences preferences);

    /// <summary>
    /// Builds the full funding plan summary: per-source tax, totals, and
    /// an optimal liquidation ordering (lowest effective tax rate first).
    /// </summary>
    FundingPlanSummary BuildPlan(IEnumerable<FundingSource> sources, UserPreferences preferences);
}

/// <summary>
/// Tax estimate for a single funding source.
/// </summary>
public class FundingSourceTaxResult
{
    public required FundingSource Source { get; init; }

    /// <summary>Gross amount drawn from the source.</summary>
    public decimal GrossAmount { get; init; }

    /// <summary>Ordinary income tax portion.</summary>
    public decimal IncomeTax { get; init; }

    /// <summary>Capital gains tax portion.</summary>
    public decimal CapitalGainsTax { get; init; }

    /// <summary>Early-withdrawal penalty portion.</summary>
    public decimal Penalty { get; init; }

    /// <summary>Total estimated tax.</summary>
    public decimal TotalTax => IncomeTax + CapitalGainsTax + Penalty;

    /// <summary>Net amount after taxes.</summary>
    public decimal NetAmount => GrossAmount - TotalTax;

    /// <summary>Effective tax rate on this source (percent).</summary>
    public decimal EffectiveRate => GrossAmount > 0 ? Math.Round(TotalTax / GrossAmount * 100, 1) : 0;

    /// <summary>Explanation of the tax treatment.</summary>
    public string? Explanation { get; init; }
}

/// <summary>
/// Complete funding plan with tax totals (spec 4.4.4/4.4.5).
/// </summary>
public class FundingPlanSummary
{
    public IReadOnlyList<FundingSourceTaxResult> Sources { get; init; } = Array.Empty<FundingSourceTaxResult>();

    public decimal TotalGross { get; init; }
    public decimal TotalTax { get; init; }
    public decimal TotalNet { get; init; }

    /// <summary>
    /// Sources ordered by effective tax rate ascending — the cheapest money
    /// to use first (spec 4.2.2 "optimal ordering of source liquidation").
    /// </summary>
    public IReadOnlyList<FundingSourceTaxResult> OptimalOrder { get; init; } = Array.Empty<FundingSourceTaxResult>();
}
