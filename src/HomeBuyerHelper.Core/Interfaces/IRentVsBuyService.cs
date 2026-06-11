using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Rent vs. buy comparison (P4-RVB-001).
/// </summary>
public interface IRentVsBuyService
{
    /// <summary>
    /// Simulates renting vs. buying year by year and finds the breakeven point.
    /// </summary>
    RentVsBuyResult Compare(RentVsBuyInput input);
}

/// <summary>
/// Inputs for the rent vs. buy comparison.
/// </summary>
public class RentVsBuyInput
{
    /// <summary>Purchase price of the home being considered.</summary>
    public decimal PurchasePrice { get; init; }

    /// <summary>Down payment percent (e.g., 20).</summary>
    public decimal DownPaymentPercent { get; init; } = 20m;

    /// <summary>Mortgage rate percent (e.g., 7.0).</summary>
    public decimal InterestRate { get; init; } = 7.0m;

    /// <summary>Mortgage term in years.</summary>
    public int MortgageTermYears { get; init; } = 30;

    /// <summary>Monthly ownership costs beyond P&amp;I (taxes, insurance, HOA, maintenance).</summary>
    public decimal MonthlyOwnershipCosts { get; init; }

    /// <summary>Current monthly rent.</summary>
    public decimal CurrentMonthlyRent { get; init; }

    /// <summary>Annual rent increase percent. Default 3%.</summary>
    public decimal AnnualRentIncreasePercent { get; init; } = 3m;

    /// <summary>Annual return on invested savings percent. Default 7%.</summary>
    public decimal InvestmentReturnPercent { get; init; } = 7m;

    /// <summary>Annual home appreciation percent. Default 3%.</summary>
    public decimal HomeAppreciationPercent { get; init; } = 3m;

    /// <summary>Years to simulate. Default 10.</summary>
    public int HorizonYears { get; init; } = 10;

    /// <summary>One-time purchase costs (closing costs). Estimated at 3% if zero.</summary>
    public decimal ClosingCosts { get; init; }

    /// <summary>Selling cost percent when computing equity (agent fees etc.). Default 6%.</summary>
    public decimal SellingCostPercent { get; init; } = 6m;
}

/// <summary>
/// One simulated year of the rent vs. buy comparison.
/// </summary>
public class RentVsBuyYear
{
    public int Year { get; init; }

    /// <summary>Net wealth if buying: home equity minus selling costs.</summary>
    public decimal BuyNetWealth { get; init; }

    /// <summary>Net wealth if renting: invested down payment + invested monthly savings.</summary>
    public decimal RentNetWealth { get; init; }

    /// <summary>Monthly rent during this year.</summary>
    public decimal MonthlyRent { get; init; }

    /// <summary>Monthly ownership cost (P&amp;I + other costs).</summary>
    public decimal MonthlyOwnershipCost { get; init; }

    public bool BuyingIsAhead => BuyNetWealth > RentNetWealth;
}

/// <summary>
/// Result of the rent vs. buy comparison.
/// </summary>
public class RentVsBuyResult
{
    public IReadOnlyList<RentVsBuyYear> Years { get; init; } = Array.Empty<RentVsBuyYear>();

    /// <summary>First year buying pulls ahead of renting; null if never within the horizon.</summary>
    public int? BreakevenYear { get; init; }

    /// <summary>Monthly cost of owning in year one (P&amp;I + other costs).</summary>
    public decimal InitialMonthlyOwnershipCost { get; init; }

    /// <summary>Net wealth difference (buy - rent) at the end of the horizon.</summary>
    public decimal FinalAdvantage { get; init; }
}
