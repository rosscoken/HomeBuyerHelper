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
    Other
}
