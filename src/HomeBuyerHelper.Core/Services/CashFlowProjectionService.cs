using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Services;

/// <summary>
/// Builds month-by-month cash flow projections with emergency fund tracking.
///
/// Emergency fund rules (design spec section 3.6 / P2-EMF-002):
/// - Deficit months draw from the fund automatically.
/// - Surplus months replenish the fund up to its target before counting
///   toward cumulative savings.
/// - A warning is flagged whenever the fund drops below 3 months of expenses.
/// </summary>
public class CashFlowProjectionService : ICashFlowProjectionService
{
    private const int WarningThresholdMonths = 3;

    private readonly IIncomeScenarioService _incomeScenarioService;

    public CashFlowProjectionService(IIncomeScenarioService incomeScenarioService)
    {
        _incomeScenarioService = incomeScenarioService;
    }

    public IReadOnlyList<MonthlyProjection> Project(CashFlowProjectionInput input)
    {
        var start = input.StartMonth ?? DateTime.Today;
        var firstMonth = new DateTime(start.Year, start.Month, 1);

        var fixedMonthly = input.Expenses.Where(e => !e.IsVariable).Sum(e => e.MonthlyAmount);
        var variableMonthly = input.Expenses.Where(e => e.IsVariable).Sum(e => e.MonthlyAmount);

        // Warning threshold uses typical recurring monthly spend (incl. housing).
        var typicalMonthlyExpenses = fixedMonthly + variableMonthly + input.MonthlyHousingCost;
        var warningThreshold = typicalMonthlyExpenses * WarningThresholdMonths;
        var fundTarget = input.EmergencyFund != null
            ? typicalMonthlyExpenses * input.EmergencyFund.TargetMonths
            : 0;

        var projections = new List<MonthlyProjection>(input.Months);
        var cumulative = 0m;
        var fundBalance = input.EmergencyFund?.CurrentBalance ?? 0;

        for (var i = 0; i < input.Months; i++)
        {
            var month = firstMonth.AddMonths(i);

            var eventsThisMonth = input.OneTimeEvents
                .Where(e => e.Date.Year == month.Year && e.Date.Month == month.Month)
                .ToList();

            var projection = new MonthlyProjection
            {
                Month = month,
                TotalIncome = Math.Round(
                    _incomeScenarioService.GetTotalForMonth(input.IncomeSources, month, input.Scenario), 2),
                FixedExpenses = Math.Round(fixedMonthly, 2),
                VariableExpenses = Math.Round(variableMonthly, 2),
                OneTimeExpenses = Math.Round(eventsThisMonth.Sum(e => e.Amount), 2),
                HousingCost = Math.Round(input.MonthlyHousingCost, 2),
                EventNames = eventsThisMonth.Select(e => e.Name).ToList()
            };

            cumulative += projection.Surplus;
            projection.CumulativeSurplus = Math.Round(cumulative, 2);

            if (input.EmergencyFund != null)
            {
                if (projection.Surplus < 0)
                {
                    // Deficit: draw from the fund (never below zero).
                    fundBalance = Math.Max(0, fundBalance + projection.Surplus);
                }
                else if (fundBalance < fundTarget)
                {
                    // Surplus: replenish toward target first.
                    var contribution = Math.Min(projection.Surplus, fundTarget - fundBalance);
                    fundBalance += contribution;
                }

                projection.EmergencyFundBalance = Math.Round(fundBalance, 2);
                projection.EmergencyFundWarning = warningThreshold > 0 && fundBalance < warningThreshold;
            }

            projections.Add(projection);
        }

        return projections;
    }
}
