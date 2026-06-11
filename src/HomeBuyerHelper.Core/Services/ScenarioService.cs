using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Services;

/// <summary>
/// Evaluates "what if" purchase scenarios (P5-SCN-001/003).
/// </summary>
public class ScenarioService : IScenarioService
{
    private readonly ICalculationService _calculationService;
    private readonly IIncomeScenarioService _incomeScenarioService;
    private readonly IAffordabilityService _affordabilityService;

    public ScenarioService(
        ICalculationService calculationService,
        IIncomeScenarioService incomeScenarioService,
        IAffordabilityService affordabilityService)
    {
        _calculationService = calculationService;
        _incomeScenarioService = incomeScenarioService;
        _affordabilityService = affordabilityService;
    }

    public ScenarioResult Evaluate(PurchaseScenario scenario, IReadOnlyList<IncomeSource> incomeSources)
    {
        var percentDown = _calculationService.CalculateDownPayment(
            scenario.PurchasePrice, scenario.DownPaymentPercent);
        var downPayment = Math.Min(scenario.PurchasePrice, percentDown + scenario.AdditionalDownPayment);
        var loanAmount = Math.Max(0, scenario.PurchasePrice - downPayment);
        var monthlyPayment = _calculationService.CalculateMonthlyMortgagePayment(
            loanAmount, scenario.InterestRate, scenario.MortgageTermYears);
        var totalLoanCost = monthlyPayment * scenario.MortgageTermYears * 12;
        var closingCosts = scenario.PurchasePrice > 0
            ? _calculationService.CalculateClosingCosts(scenario.PurchasePrice).Total
            : 0;

        var income = _incomeScenarioService.GetAverageMonthlyIncome(incomeSources, scenario.IncomeScenario);
        var housingPercent = _affordabilityService.CalculateHousingPercentage(monthlyPayment, income);

        return new ScenarioResult
        {
            Scenario = scenario,
            DownPayment = Math.Round(downPayment, 0),
            LoanAmount = Math.Round(loanAmount, 0),
            MonthlyPayment = monthlyPayment,
            TotalLoanCost = Math.Round(totalLoanCost, 0),
            TotalInterest = Math.Round(totalLoanCost - loanAmount, 0),
            CashToClose = Math.Round(downPayment + closingCosts, 0),
            HousingPercentage = housingPercent,
            AffordabilityZone = income > 0
                ? _affordabilityService.GetZone(housingPercent)
                : AffordabilityZone.Risky
        };
    }

    public PurchaseScenario BuildWaitScenario(
        PurchaseScenario baseScenario,
        int monthsToWait,
        decimal monthlySavings,
        decimal annualMarketChangePercent,
        decimal currentMonthlyRent)
    {
        var marketFactor = (decimal)Math.Pow(
            1 + (double)(annualMarketChangePercent / 100), monthsToWait / 12.0);

        return new PurchaseScenario
        {
            Name = $"{baseScenario.Name} +{monthsToWait}mo",
            Description = $"Wait {monthsToWait} months: save {monthlySavings:C0}/mo, " +
                          $"market {annualMarketChangePercent:+0.#;-0.#}%/yr, rent {currentMonthlyRent:C0}/mo meanwhile",
            PurchasePrice = Math.Round(baseScenario.PurchasePrice * marketFactor, 0),
            DownPaymentPercent = baseScenario.DownPaymentPercent,
            AdditionalDownPayment = baseScenario.AdditionalDownPayment + monthlySavings * monthsToWait,
            InterestRate = baseScenario.InterestRate,
            MortgageTermYears = baseScenario.MortgageTermYears,
            IncomeScenario = baseScenario.IncomeScenario,
            SunkCosts = baseScenario.SunkCosts + currentMonthlyRent * monthsToWait
        };
    }
}
