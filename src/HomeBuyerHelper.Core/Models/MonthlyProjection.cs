namespace HomeBuyerHelper.Core.Models;

/// <summary>
/// Income scenarios for budget planning (design spec section 3.2).
/// </summary>
public enum IncomeScenario
{
    /// <summary>Salary + guaranteed income only. Stress testing.</summary>
    Conservative,

    /// <summary>Salary + probability-weighted variable income. Monthly budgeting.</summary>
    Realistic,

    /// <summary>All income at expected amounts. Long-term planning.</summary>
    Expected
}

/// <summary>
/// A single month in the cash flow projection (design spec section 3.5).
/// </summary>
public class MonthlyProjection
{
    /// <summary>
    /// The calendar month this projection covers (first of month).
    /// </summary>
    public DateTime Month { get; set; }

    /// <summary>
    /// Total income for the month under the selected scenario.
    /// </summary>
    public decimal TotalIncome { get; set; }

    /// <summary>
    /// Sum of fixed recurring expenses.
    /// </summary>
    public decimal FixedExpenses { get; set; }

    /// <summary>
    /// Sum of variable recurring expenses.
    /// </summary>
    public decimal VariableExpenses { get; set; }

    /// <summary>
    /// One-time events that land in this month.
    /// </summary>
    public decimal OneTimeExpenses { get; set; }

    /// <summary>
    /// Estimated monthly housing cost (when projecting against a property).
    /// </summary>
    public decimal HousingCost { get; set; }

    /// <summary>
    /// Total expenses for the month.
    /// </summary>
    public decimal TotalExpenses => FixedExpenses + VariableExpenses + OneTimeExpenses + HousingCost;

    /// <summary>
    /// Income minus all expenses for the month.
    /// </summary>
    public decimal Surplus => TotalIncome - TotalExpenses;

    /// <summary>
    /// Running total of surpluses/deficits up to and including this month.
    /// </summary>
    public decimal CumulativeSurplus { get; set; }

    /// <summary>
    /// A month where expenses exceed income, requiring an emergency fund draw.
    /// </summary>
    public bool IsCrunchMonth => Surplus < 0;

    /// <summary>
    /// Projected emergency fund balance at the end of this month.
    /// </summary>
    public decimal EmergencyFundBalance { get; set; }

    /// <summary>
    /// True when the emergency fund is below the warning threshold
    /// (3 months of expenses) at the end of this month.
    /// </summary>
    public bool EmergencyFundWarning { get; set; }

    /// <summary>
    /// Names of one-time events landing in this month, for display.
    /// </summary>
    public List<string> EventNames { get; set; } = new();
}

/// <summary>
/// Emergency fund configuration (design spec section 3.6).
/// </summary>
public class EmergencyFundConfig
{
    /// <summary>
    /// Target fund size expressed in months of expenses. Default 6.
    /// </summary>
    public int TargetMonths { get; set; } = 6;

    /// <summary>
    /// Current emergency fund balance.
    /// </summary>
    public decimal CurrentBalance { get; set; }
}
