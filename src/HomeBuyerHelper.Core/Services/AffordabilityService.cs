using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Services;

/// <summary>
/// Housing affordability analysis with color-coded zones.
/// Zones (design spec 3.4.2): Green &lt;28%, Yellow 28-36%, Orange 36-43%, Red &gt;43%.
/// </summary>
public class AffordabilityService : IAffordabilityService
{
    private readonly IIncomeScenarioService _incomeScenarioService;

    public AffordabilityService(IIncomeScenarioService incomeScenarioService)
    {
        _incomeScenarioService = incomeScenarioService;
    }

    public decimal CalculateHousingPercentage(decimal monthlyHousingCost, decimal grossMonthlyIncome)
    {
        if (grossMonthlyIncome <= 0)
        {
            return 0;
        }

        return Math.Round(monthlyHousingCost / grossMonthlyIncome * 100, 2);
    }

    public AffordabilityZone GetZone(decimal housingPercentage)
    {
        return housingPercentage switch
        {
            < 28 => AffordabilityZone.Comfortable,
            <= 36 => AffordabilityZone.Stretching,
            <= 43 => AffordabilityZone.Aggressive,
            _ => AffordabilityZone.Risky
        };
    }

    public IReadOnlyList<AffordabilityAssessment> AssessAllScenarios(
        decimal monthlyHousingCost,
        IEnumerable<IncomeSource> incomeSources)
    {
        var sources = incomeSources.ToList();
        var results = new List<AffordabilityAssessment>();

        foreach (var scenario in new[] { IncomeScenario.Conservative, IncomeScenario.Realistic, IncomeScenario.Expected })
        {
            var income = _incomeScenarioService.GetAverageMonthlyIncome(sources, scenario);
            var percentage = CalculateHousingPercentage(monthlyHousingCost, income);

            results.Add(new AffordabilityAssessment
            {
                Scenario = scenario,
                GrossMonthlyIncome = Math.Round(income, 2),
                MonthlyHousingCost = monthlyHousingCost,
                HousingPercentage = percentage,
                Zone = income <= 0 ? AffordabilityZone.Risky : GetZone(percentage)
            });
        }

        return results;
    }
}
