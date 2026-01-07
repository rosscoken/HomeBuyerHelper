namespace HomeBuyerHelper.Core.Models;

/// <summary>
/// Represents a monthly budget projection for cash flow analysis.
/// </summary>
public class BudgetMonth
{
    public int Id { get; set; }

    /// <summary>
    /// The property this budget is calculated for.
    /// </summary>
    public int PropertyId { get; set; }

    /// <summary>
    /// The month and year of this budget projection.
    /// </summary>
    public DateTime Month { get; set; }

    /// <summary>
    /// Total gross income for the month.
    /// </summary>
    public decimal GrossIncome { get; set; }

    /// <summary>
    /// Estimated taxes withheld.
    /// </summary>
    public decimal TaxWithholding { get; set; }

    /// <summary>
    /// Net income after taxes.
    /// </summary>
    public decimal NetIncome => GrossIncome - TaxWithholding;

    /// <summary>
    /// Monthly mortgage payment (principal + interest).
    /// </summary>
    public decimal MortgagePayment { get; set; }

    /// <summary>
    /// Monthly property tax payment.
    /// </summary>
    public decimal PropertyTax { get; set; }

    /// <summary>
    /// Monthly insurance payment.
    /// </summary>
    public decimal Insurance { get; set; }

    /// <summary>
    /// Monthly HOA fees.
    /// </summary>
    public decimal HOAFees { get; set; }

    /// <summary>
    /// Total housing costs (mortgage + tax + insurance + HOA).
    /// </summary>
    public decimal TotalHousingCost => MortgagePayment + PropertyTax + Insurance + HOAFees;

    /// <summary>
    /// Monthly utilities estimate.
    /// </summary>
    public decimal Utilities { get; set; }

    /// <summary>
    /// Monthly maintenance reserve.
    /// </summary>
    public decimal MaintenanceReserve { get; set; }

    /// <summary>
    /// Total of all other recurring expenses.
    /// </summary>
    public decimal OtherExpenses { get; set; }

    /// <summary>
    /// Total monthly expenses.
    /// </summary>
    public decimal TotalExpenses => TotalHousingCost + Utilities + MaintenanceReserve + OtherExpenses;

    /// <summary>
    /// Net cash flow after all expenses.
    /// </summary>
    public decimal NetCashFlow => NetIncome - TotalExpenses;

    /// <summary>
    /// Housing cost as percentage of gross income (DTI front-end).
    /// </summary>
    public decimal HousingRatio => GrossIncome > 0 ? TotalHousingCost / GrossIncome * 100 : 0;

    /// <summary>
    /// Total debt payments as percentage of gross income (DTI back-end).
    /// </summary>
    public decimal TotalDebtRatio { get; set; }

    /// <summary>
    /// Whether this month has positive cash flow.
    /// </summary>
    public bool IsPositiveCashFlow => NetCashFlow > 0;

    /// <summary>
    /// Navigation property for the property.
    /// </summary>
    public Property? Property { get; set; }

    /// <summary>
    /// When this projection was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this projection was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
