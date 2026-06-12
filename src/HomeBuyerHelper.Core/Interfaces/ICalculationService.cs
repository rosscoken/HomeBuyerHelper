using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Service interface for financial calculations.
/// </summary>
public interface ICalculationService
{
    /// <summary>
    /// Calculates the monthly mortgage payment.
    /// </summary>
    /// <param name="principal">Loan amount.</param>
    /// <param name="annualInterestRate">Annual interest rate as percentage (e.g., 7.0 for 7%).</param>
    /// <param name="termYears">Loan term in years.</param>
    /// <returns>Monthly payment amount.</returns>
    decimal CalculateMonthlyMortgagePayment(decimal principal, decimal annualInterestRate, int termYears);

    /// <summary>
    /// Calculates the total monthly housing cost.
    /// </summary>
    MonthlyCostBreakdown CalculateMonthlyHousingCost(
        decimal purchasePrice,
        decimal downPaymentPercent,
        decimal annualInterestRate,
        int termYears,
        decimal annualPropertyTax,
        decimal annualInsurance,
        decimal monthlyHOA);

    /// <summary>
    /// Calculates the down payment amount.
    /// </summary>
    decimal CalculateDownPayment(decimal purchasePrice, decimal downPaymentPercent);

    /// <summary>
    /// Calculates estimated closing costs.
    /// </summary>
    ClosingCostEstimate CalculateClosingCosts(decimal purchasePrice, string? state = null);

    /// <summary>
    /// Calculates the debt-to-income ratio.
    /// </summary>
    /// <param name="monthlyDebt">Total monthly debt payments.</param>
    /// <param name="monthlyIncome">Gross monthly income.</param>
    /// <returns>DTI as percentage.</returns>
    decimal CalculateDTI(decimal monthlyDebt, decimal monthlyIncome);

    /// <summary>
    /// Calculates how much house the user can afford.
    /// </summary>
    AffordabilityResult CalculateAffordability(
        decimal monthlyIncome,
        decimal monthlyDebts,
        decimal downPaymentAmount,
        decimal annualInterestRate,
        int termYears,
        decimal maxDTI = 43m);

    /// <summary>
    /// Calculates the amortization schedule.
    /// </summary>
    IReadOnlyList<AmortizationEntry> CalculateAmortizationSchedule(
        decimal principal,
        decimal annualInterestRate,
        int termYears);
}

/// <summary>
/// Breakdown of monthly housing costs.
/// </summary>
public class MonthlyCostBreakdown
{
    public decimal Principal { get; init; }
    public decimal Interest { get; init; }
    public decimal PropertyTax { get; init; }
    public decimal Insurance { get; init; }
    public decimal HOA { get; init; }
    public decimal PMI { get; init; }
    public decimal Total => Principal + Interest + PropertyTax + Insurance + HOA + PMI;
}

/// <summary>
/// Estimated closing costs.
/// </summary>
public class ClosingCostEstimate
{
    public decimal LoanOriginationFee { get; init; }
    public decimal AppraisalFee { get; init; }
    public decimal TitleInsurance { get; init; }
    public decimal TitleSearch { get; init; }
    public decimal EscrowFees { get; init; }
    public decimal RecordingFees { get; init; }
    public decimal PrepaidInterest { get; init; }
    public decimal HomeInspection { get; init; }
    public decimal OtherFees { get; init; }
    public decimal Total => LoanOriginationFee + AppraisalFee + TitleInsurance +
                           TitleSearch + EscrowFees + RecordingFees +
                           PrepaidInterest + HomeInspection + OtherFees;
    public decimal LowEstimate { get; init; }
    public decimal HighEstimate { get; init; }
}

/// <summary>
/// Result of affordability calculation.
/// </summary>
public class AffordabilityResult
{
    public decimal MaxPurchasePrice { get; init; }
    public decimal MaxMonthlyPayment { get; init; }
    public decimal RequiredDownPayment { get; init; }
    public decimal EstimatedMonthlyPayment { get; init; }
    public decimal DTIWithMaxPrice { get; init; }
    public bool MeetsConventionalGuidelines { get; init; }
}

/// <summary>
/// Single entry in an amortization schedule.
/// </summary>
public class AmortizationEntry
{
    public int PaymentNumber { get; init; }
    public DateTime PaymentDate { get; init; }
    public decimal Payment { get; init; }
    public decimal Principal { get; init; }
    public decimal Interest { get; init; }
    public decimal RemainingBalance { get; init; }
    public decimal TotalInterestPaid { get; init; }
    public decimal TotalPrincipalPaid { get; init; }
}
