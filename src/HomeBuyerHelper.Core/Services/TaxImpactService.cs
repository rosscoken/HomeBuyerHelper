using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Services;

/// <summary>
/// Estimates tax impact of down payment funding sources using 2024 federal
/// brackets. Estimates only — not tax advice; state tax uses the user's
/// manually entered marginal rate.
///
/// Rules per design spec section 4.1:
/// - Savings/checking/gift: no tax to recipient.
/// - Brokerage: gains taxed at ordinary (short-term) or LTCG (long-term) rates.
/// - Traditional IRA: ordinary income + 10% penalty under 59.5 unless the
///   first-time buyer exemption covers up to $10,000.
/// - Inherited IRA: ordinary income, no penalty.
/// - Roth IRA: contributions free; earnings taxed + penalized unless the
///   account is 5+ years old AND (59.5+ or first-time buyer up to $10,000).
/// - 401(k) loan: no tax if repaid. Hardship withdrawal: income tax + 10%
///   penalty under 59.5.
/// </summary>
public class TaxImpactService : ITaxImpactService
{
    private const decimal FirstTimeBuyerExemption = 10_000m;
    private const decimal EarlyWithdrawalPenaltyRate = 0.10m;
    private const decimal PenaltyFreeAge = 59.5m;

    // 2024 federal marginal brackets: (upper bound, rate %).
    private static readonly (decimal UpTo, decimal Rate)[] SingleBrackets =
    {
        (11_600m, 10), (47_150m, 12), (100_525m, 22), (191_950m, 24),
        (243_725m, 32), (609_350m, 35), (decimal.MaxValue, 37)
    };

    private static readonly (decimal UpTo, decimal Rate)[] MarriedJointBrackets =
    {
        (23_200m, 10), (94_300m, 12), (201_050m, 22), (383_900m, 24),
        (487_450m, 32), (731_200m, 35), (decimal.MaxValue, 37)
    };

    private static readonly (decimal UpTo, decimal Rate)[] MarriedSeparateBrackets =
    {
        (11_600m, 10), (47_150m, 12), (100_525m, 22), (191_950m, 24),
        (243_725m, 32), (365_600m, 35), (decimal.MaxValue, 37)
    };

    private static readonly (decimal UpTo, decimal Rate)[] HeadOfHouseholdBrackets =
    {
        (16_550m, 10), (63_100m, 12), (100_500m, 22), (191_950m, 24),
        (243_700m, 32), (609_350m, 35), (decimal.MaxValue, 37)
    };

    // 2024 long-term capital gains thresholds: (upper bound, rate %).
    private static readonly (decimal UpTo, decimal Rate)[] LtcgSingle =
    {
        (47_025m, 0), (518_900m, 15), (decimal.MaxValue, 20)
    };

    private static readonly (decimal UpTo, decimal Rate)[] LtcgMarriedJoint =
    {
        (94_050m, 0), (583_750m, 15), (decimal.MaxValue, 20)
    };

    private static readonly (decimal UpTo, decimal Rate)[] LtcgMarriedSeparate =
    {
        (47_025m, 0), (291_850m, 15), (decimal.MaxValue, 20)
    };

    private static readonly (decimal UpTo, decimal Rate)[] LtcgHeadOfHousehold =
    {
        (63_000m, 0), (551_350m, 15), (decimal.MaxValue, 20)
    };

    public decimal GetMarginalFederalRate(TaxFilingStatus filingStatus, decimal taxableIncome)
    {
        var brackets = filingStatus switch
        {
            TaxFilingStatus.MarriedFilingJointly => MarriedJointBrackets,
            TaxFilingStatus.MarriedFilingSeparately => MarriedSeparateBrackets,
            TaxFilingStatus.HeadOfHousehold => HeadOfHouseholdBrackets,
            _ => SingleBrackets
        };

        return FindRate(brackets, taxableIncome);
    }

    public decimal GetLongTermCapitalGainsRate(TaxFilingStatus filingStatus, decimal taxableIncome)
    {
        var brackets = filingStatus switch
        {
            TaxFilingStatus.MarriedFilingJointly => LtcgMarriedJoint,
            TaxFilingStatus.MarriedFilingSeparately => LtcgMarriedSeparate,
            TaxFilingStatus.HeadOfHousehold => LtcgHeadOfHousehold,
            _ => LtcgSingle
        };

        return FindRate(brackets, taxableIncome);
    }

    private static decimal FindRate((decimal UpTo, decimal Rate)[] brackets, decimal income)
    {
        foreach (var (upTo, rate) in brackets)
        {
            if (income <= upTo)
            {
                return rate;
            }
        }
        return brackets[^1].Rate;
    }

    public FundingSourceTaxResult CalculateTax(FundingSource source, UserPreferences preferences)
    {
        var amount = source.EffectiveAmount;
        var ordinaryRate = (GetMarginalFederalRate(preferences.FilingStatus, preferences.EstimatedTaxableIncome)
            + preferences.StateMarginalTaxRate) / 100m;
        var ltcgRate = (GetLongTermCapitalGainsRate(preferences.FilingStatus, preferences.EstimatedTaxableIncome)
            + preferences.StateMarginalTaxRate) / 100m;

        return source.FundingType switch
        {
            FundingType.Savings or FundingType.Checking or FundingType.HomeSaleProceeds
                or FundingType.DownPaymentAssistance or FundingType.EmployerAssistance =>
                NoTax(source, amount, "Already-taxed funds. No additional tax."),

            FundingType.Gift or FundingType.Inheritance =>
                NoTax(source, amount,
                    "No tax to recipient. Lender requires a gift letter; funds typically need 60 days of seasoning."),

            FundingType.Investment => BrokerageTax(source, amount, ordinaryRate, ltcgRate),

            FundingType.RetirementIRA => TraditionalIraTax(source, amount, ordinaryRate),

            FundingType.InheritedIRA => new FundingSourceTaxResult
            {
                Source = source,
                GrossAmount = amount,
                IncomeTax = Math.Round(amount * ordinaryRate, 2),
                Explanation = "Taxed as ordinary income, no early-withdrawal penalty. " +
                              "Inherited IRAs must be fully distributed within 10 years."
            },

            FundingType.RothIRA => RothIraTax(source, amount, ordinaryRate),

            FundingType.Retirement401k => FourOhOneKTax(source, amount, ordinaryRate),

            _ => NoTax(source, amount, "No estimated tax impact.")
        };
    }

    public FundingPlanSummary BuildPlan(IEnumerable<FundingSource> sources, UserPreferences preferences)
    {
        var results = sources.Select(source => CalculateTax(source, preferences)).ToList();

        return new FundingPlanSummary
        {
            Sources = results,
            TotalGross = results.Sum(r => r.GrossAmount),
            TotalTax = results.Sum(r => r.TotalTax),
            TotalNet = results.Sum(r => r.NetAmount),
            OptimalOrder = results.OrderBy(r => r.EffectiveRate).ThenByDescending(r => r.GrossAmount).ToList()
        };
    }

    private static FundingSourceTaxResult NoTax(FundingSource source, decimal amount, string explanation) => new()
    {
        Source = source,
        GrossAmount = amount,
        Explanation = explanation
    };

    private static FundingSourceTaxResult BrokerageTax(
        FundingSource source, decimal amount, decimal ordinaryRate, decimal ltcgRate)
    {
        var gain = Math.Max(0, amount - (source.CostBasis ?? amount));
        var rate = source.IsLongTermHolding ? ltcgRate : ordinaryRate;

        return new FundingSourceTaxResult
        {
            Source = source,
            GrossAmount = amount,
            CapitalGainsTax = Math.Round(gain * rate, 2),
            Explanation = source.IsLongTermHolding
                ? "Long-term gains taxed at preferential capital gains rates."
                : "Short-term gains taxed as ordinary income."
        };
    }

    private static FundingSourceTaxResult TraditionalIraTax(
        FundingSource source, decimal amount, decimal ordinaryRate)
    {
        var incomeTax = amount * ordinaryRate;
        var penalty = 0m;
        var age = source.OwnerAge ?? 0;

        if (age < PenaltyFreeAge)
        {
            var penalizedAmount = source.IsFirstTimeBuyer
                ? Math.Max(0, amount - FirstTimeBuyerExemption)
                : amount;
            penalty = penalizedAmount * EarlyWithdrawalPenaltyRate;
        }

        return new FundingSourceTaxResult
        {
            Source = source,
            GrossAmount = amount,
            IncomeTax = Math.Round(incomeTax, 2),
            Penalty = Math.Round(penalty, 2),
            Explanation = "Taxed as ordinary income. First-time buyers are exempt from the 10% " +
                          "penalty on the first $10,000; age 59½+ is fully penalty-free."
        };
    }

    private static FundingSourceTaxResult RothIraTax(
        FundingSource source, decimal amount, decimal ordinaryRate)
    {
        var contributions = Math.Min(amount, source.RothContributionPortion ?? amount);
        var earnings = amount - contributions;

        var incomeTax = 0m;
        var penalty = 0m;
        var age = source.OwnerAge ?? 0;

        if (earnings > 0)
        {
            var qualifies = source.IsRothAccount5Years &&
                (age >= PenaltyFreeAge || source.IsFirstTimeBuyer);

            if (qualifies)
            {
                // First-time buyer exemption covers up to $10k of earnings.
                var exemptEarnings = age >= PenaltyFreeAge
                    ? earnings
                    : Math.Min(earnings, FirstTimeBuyerExemption);
                var taxableEarnings = earnings - exemptEarnings;
                incomeTax = taxableEarnings * ordinaryRate;
                penalty = taxableEarnings * EarlyWithdrawalPenaltyRate;
            }
            else
            {
                incomeTax = earnings * ordinaryRate;
                if (age < PenaltyFreeAge)
                {
                    penalty = earnings * EarlyWithdrawalPenaltyRate;
                }
            }
        }

        return new FundingSourceTaxResult
        {
            Source = source,
            GrossAmount = amount,
            IncomeTax = Math.Round(incomeTax, 2),
            Penalty = Math.Round(penalty, 2),
            Explanation = "Contributions are always tax- and penalty-free. Earnings are tax-free " +
                          "when the account is 5+ years old and you qualify (59½+ or first-time buyer up to $10,000)."
        };
    }

    private static FundingSourceTaxResult FourOhOneKTax(
        FundingSource source, decimal amount, decimal ordinaryRate)
    {
        if (source.Is401kLoan)
        {
            return new FundingSourceTaxResult
            {
                Source = source,
                GrossAmount = amount,
                Explanation = "401(k) loans have no tax impact if repaid on schedule. " +
                              "Leaving your employer may accelerate repayment."
            };
        }

        var age = source.OwnerAge ?? 0;
        var penalty = age < PenaltyFreeAge ? amount * EarlyWithdrawalPenaltyRate : 0;

        return new FundingSourceTaxResult
        {
            Source = source,
            GrossAmount = amount,
            IncomeTax = Math.Round(amount * ordinaryRate, 2),
            Penalty = Math.Round(penalty, 2),
            Explanation = "Hardship withdrawals are taxed as ordinary income plus a 10% penalty under age 59½."
        };
    }
}
