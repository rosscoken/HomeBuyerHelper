using System.Text.Json.Serialization;

namespace HomeBuyerHelper.Core.Models;

/// <summary>
/// Represents a source of income for budget planning.
/// </summary>
public class IncomeSource
{
    public int Id { get; set; }

    /// <summary>
    /// Name of the income source (e.g., "Primary Job", "Side Hustle").
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gross income amount before taxes.
    /// </summary>
    public decimal GrossAmount { get; set; }

    /// <summary>
    /// Net income amount after taxes (if known).
    /// </summary>
    public decimal? NetAmount { get; set; }

    /// <summary>
    /// Frequency of the income.
    /// </summary>
    public IncomeFrequency Frequency { get; set; } = IncomeFrequency.Monthly;

    /// <summary>
    /// Type of income source.
    /// </summary>
    public IncomeType IncomeType { get; set; } = IncomeType.Employment;

    /// <summary>
    /// Whether this income is reliable/guaranteed.
    /// </summary>
    public bool IsReliable { get; set; } = true;

    /// <summary>
    /// For non-monthly income (bonus, annual/quarterly vests): the calendar
    /// month (1-12) the payment lands in. For quarterly frequency this is the
    /// first month of the cycle. Null uses sensible defaults (Dec for annual,
    /// Mar for quarterly).
    /// </summary>
    public int? PaymentMonth { get; set; }

    /// <summary>
    /// Likelihood (0-100) of receiving this income at the expected amount.
    /// Used by the Realistic planning scenario for variable income.
    /// Defaults to 70 for variable income semantics per the design spec.
    /// </summary>
    public decimal Probability { get; set; } = 100m;

    /// <summary>
    /// When this income begins (e.g., partner contribution starting later).
    /// Null means already active.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// When this income ends. Null means indefinite.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Optional notes about this income source.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// When the income source was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the income source was last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Calculates the monthly gross income.
    /// </summary>
    [JsonIgnore]
    public decimal MonthlyGrossIncome => Frequency switch
    {
        IncomeFrequency.Weekly => GrossAmount * 52 / 12,
        IncomeFrequency.BiWeekly => GrossAmount * 26 / 12,
        IncomeFrequency.SemiMonthly => GrossAmount * 2,
        IncomeFrequency.Monthly => GrossAmount,
        IncomeFrequency.Quarterly => GrossAmount / 3,
        IncomeFrequency.Annually => GrossAmount / 12,
        _ => GrossAmount
    };

    /// <summary>
    /// Calculates the annual gross income.
    /// </summary>
    [JsonIgnore]
    public decimal AnnualGrossIncome => MonthlyGrossIncome * 12;
}

/// <summary>
/// Frequency options for income.
/// </summary>
public enum IncomeFrequency
{
    Weekly,
    BiWeekly,
    SemiMonthly,
    Monthly,
    Quarterly,
    Annually
}

/// <summary>
/// Types of income sources.
/// </summary>
public enum IncomeType
{
    Employment,
    SelfEmployment,
    Rental,
    Investment,
    Retirement,
    SocialSecurity,
    Disability,
    Alimony,
    ChildSupport,
    Other,
    Bonus,
    RSU,
    PartnerContribution
}
