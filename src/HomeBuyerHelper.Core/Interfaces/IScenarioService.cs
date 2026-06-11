using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// "What if" scenario planning (P5-SCN-001/002/003).
/// </summary>
public interface IScenarioService
{
    /// <summary>
    /// Evaluates a purchase scenario: monthly payment, total cost,
    /// cash to close, and affordability.
    /// </summary>
    ScenarioResult Evaluate(PurchaseScenario scenario, IReadOnlyList<IncomeSource> incomeSources);

    /// <summary>
    /// Builds a "wait N months" variant of a scenario: rent paid in the
    /// meantime is a sunk cost, monthly savings grow the down payment, and
    /// the price moves with the assumed market change.
    /// </summary>
    PurchaseScenario BuildWaitScenario(
        PurchaseScenario baseScenario,
        int monthsToWait,
        decimal monthlySavings,
        decimal annualMarketChangePercent,
        decimal currentMonthlyRent);
}

/// <summary>
/// A named purchase scenario (P5-SCN-001).
/// </summary>
public class PurchaseScenario
{
    public required string Name { get; set; }

    /// <summary>Purchase price (can differ from asking).</summary>
    public decimal PurchasePrice { get; set; }

    /// <summary>Down payment percent.</summary>
    public decimal DownPaymentPercent { get; set; } = 20m;

    /// <summary>Extra cash applied to the down payment beyond the percent.</summary>
    public decimal AdditionalDownPayment { get; set; }

    /// <summary>Mortgage rate percent.</summary>
    public decimal InterestRate { get; set; } = 7.0m;

    /// <summary>Mortgage term in years.</summary>
    public int MortgageTermYears { get; set; } = 30;

    /// <summary>Income scenario used for affordability.</summary>
    public IncomeScenario IncomeScenario { get; set; } = IncomeScenario.Realistic;

    /// <summary>Sunk costs accrued before buying (e.g., rent paid while waiting).</summary>
    public decimal SunkCosts { get; set; }

    /// <summary>Free-form description (e.g., "Wait 6 months").</summary>
    public string? Description { get; set; }
}

/// <summary>
/// Evaluated scenario metrics (P5-SCN-002 columns).
/// </summary>
public class ScenarioResult
{
    public required PurchaseScenario Scenario { get; init; }

    public decimal DownPayment { get; init; }
    public decimal LoanAmount { get; init; }
    public decimal MonthlyPayment { get; init; }
    public decimal TotalLoanCost { get; init; }
    public decimal TotalInterest { get; init; }
    public decimal CashToClose { get; init; }

    /// <summary>Housing percentage of gross income under the scenario's income assumption.</summary>
    public decimal HousingPercentage { get; init; }

    public AffordabilityZone AffordabilityZone { get; init; }

    /// <summary>Total cost including sunk costs (for wait scenarios).</summary>
    public decimal TotalCostIncludingSunk => TotalLoanCost + CashToClose + Scenario.SunkCosts;
}
