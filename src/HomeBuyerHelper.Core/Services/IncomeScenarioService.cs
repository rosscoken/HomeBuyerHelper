using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Services;

/// <summary>
/// Calculates monthly income under the three planning scenarios.
///
/// Scenario rules (design spec sections 3.2 and P2-INC-003/006):
/// - Conservative Floor: reliable income only; variable income excluded.
/// - Realistic Planning: reliable income + variable income weighted by its
///   probability (default 70% per the spec's variable income factor).
/// - Expected Outcome: all income at full expected amounts.
/// </summary>
public class IncomeScenarioService : IIncomeScenarioService
{
    private const int DefaultAnnualPaymentMonth = 12;  // December
    private const int DefaultQuarterlyAnchorMonth = 3; // Mar/Jun/Sep/Dec cycle

    public decimal GetAmountForMonth(IncomeSource source, DateTime month, IncomeScenario scenario)
    {
        if (!IsActiveInMonth(source, month))
            return 0;

        var baseAmount = GetScheduledAmount(source, month);
        if (baseAmount == 0)
            return 0;

        return baseAmount * GetScenarioFactor(source, scenario);
    }

    public decimal GetTotalForMonth(IEnumerable<IncomeSource> sources, DateTime month, IncomeScenario scenario)
    {
        return sources.Sum(source => GetAmountForMonth(source, month, scenario));
    }

    public decimal GetAverageMonthlyIncome(IEnumerable<IncomeSource> sources, IncomeScenario scenario)
    {
        // Smooth lumpy income by averaging: ignore start/end windows here and
        // use each source's annualized monthly equivalent.
        return sources.Sum(source => source.MonthlyGrossIncome * GetScenarioFactor(source, scenario));
    }

    public decimal CalculateRsuNetPerVest(decimal sharesPerVest, decimal estimatedSharePrice, decimal taxWithholdingPercent)
    {
        if (sharesPerVest <= 0 || estimatedSharePrice <= 0)
            return 0;

        var gross = sharesPerVest * estimatedSharePrice;
        var withholding = Math.Clamp(taxWithholdingPercent, 0, 100);
        return Math.Round(gross * (1 - withholding / 100), 2);
    }

    private static decimal GetScenarioFactor(IncomeSource source, IncomeScenario scenario)
    {
        if (source.IsReliable)
            return 1m;

        return scenario switch
        {
            IncomeScenario.Conservative => 0m,
            IncomeScenario.Realistic => Math.Clamp(source.Probability, 0, 100) / 100m,
            IncomeScenario.Expected => 1m,
            _ => 1m
        };
    }

    private static bool IsActiveInMonth(IncomeSource source, DateTime month)
    {
        var monthStart = new DateTime(month.Year, month.Month, 1);

        if (source.StartDate is DateTime start &&
            new DateTime(start.Year, start.Month, 1) > monthStart)
        {
            return false;
        }

        if (source.EndDate is DateTime end &&
            new DateTime(end.Year, end.Month, 1) < monthStart)
        {
            return false;
        }

        return true;
    }

    private static decimal GetScheduledAmount(IncomeSource source, DateTime month)
    {
        switch (source.Frequency)
        {
            case IncomeFrequency.Weekly:
            case IncomeFrequency.BiWeekly:
            case IncomeFrequency.SemiMonthly:
            case IncomeFrequency.Monthly:
                // Regular cadence: use the smoothed monthly equivalent.
                return source.MonthlyGrossIncome;

            case IncomeFrequency.Quarterly:
            {
                var anchor = source.PaymentMonth ?? DefaultQuarterlyAnchorMonth;
                var offset = (month.Month - anchor) % 3;
                if (offset < 0) offset += 3;
                return offset == 0 ? source.GrossAmount : 0;
            }

            case IncomeFrequency.Annually:
            {
                var payMonth = source.PaymentMonth ?? DefaultAnnualPaymentMonth;
                return month.Month == payMonth ? source.GrossAmount : 0;
            }

            default:
                return source.MonthlyGrossIncome;
        }
    }
}
