using System.Text.Json.Serialization;

namespace HomeBuyerHelper.Core.Models;

/// <summary>
/// Represents a source of funds for down payment and closing costs.
/// </summary>
public class FundingSource
{
    public int Id { get; set; }

    /// <summary>
    /// Name of the funding source (e.g., "Savings Account", "Gift from Parents").
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Current available amount.
    /// </summary>
    public decimal CurrentAmount { get; set; }

    /// <summary>
    /// Expected amount at time of purchase (if growing).
    /// </summary>
    public decimal? ExpectedAmount { get; set; }

    /// <summary>
    /// Type of funding source.
    /// </summary>
    public FundingType FundingType { get; set; } = FundingType.Savings;

    /// <summary>
    /// Whether this source is liquid/readily available.
    /// </summary>
    public bool IsLiquid { get; set; } = true;

    /// <summary>
    /// Whether documentation exists for this source (important for mortgage approval).
    /// </summary>
    public bool IsDocumented { get; set; } = true;

    /// <summary>
    /// Monthly contribution being added to this source.
    /// </summary>
    public decimal MonthlyContribution { get; set; }

    /// <summary>
    /// Cost basis for investment/brokerage sources (what was originally paid).
    /// </summary>
    public decimal? CostBasis { get; set; }

    /// <summary>
    /// Whether investments were held more than one year (long-term capital
    /// gains treatment).
    /// </summary>
    public bool IsLongTermHolding { get; set; } = true;

    /// <summary>
    /// Account owner's age, used for early-withdrawal penalty rules.
    /// </summary>
    public int? OwnerAge { get; set; }

    /// <summary>
    /// Whether the buyer qualifies as a first-time home buyer
    /// (IRS definition: no home owned in the past 2 years).
    /// </summary>
    public bool IsFirstTimeBuyer { get; set; } = true;

    /// <summary>
    /// For Roth IRA: the portion of the withdrawal coming from contributions
    /// (always tax/penalty free). The remainder is treated as earnings.
    /// </summary>
    public decimal? RothContributionPortion { get; set; }

    /// <summary>
    /// For Roth IRA: whether the account has been open 5+ years.
    /// </summary>
    public bool IsRothAccount5Years { get; set; }

    /// <summary>
    /// For 401(k): true when taking a loan (repaid, no tax) rather than a
    /// hardship withdrawal (taxed + penalty).
    /// </summary>
    public bool Is401kLoan { get; set; } = true;

    /// <summary>
    /// For gifts: who the gift is from (used for the gift letter).
    /// </summary>
    public string? DonorName { get; set; }

    /// <summary>
    /// Optional notes about this funding source.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// When the funding source was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the funding source was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the effective amount (expected if set, otherwise current).
    /// </summary>
    [JsonIgnore]
    public decimal EffectiveAmount => ExpectedAmount ?? CurrentAmount;
}

/// <summary>
/// Types of funding sources.
/// </summary>
public enum FundingType
{
    Savings,
    Checking,
    Investment,
    Retirement401k,
    RetirementIRA,
    Gift,
    Inheritance,
    HomeSaleProceeds,
    DownPaymentAssistance,
    EmployerAssistance,
    Other,
    RothIRA,
    InheritedIRA
}
