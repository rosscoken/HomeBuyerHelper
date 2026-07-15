namespace HomeBuyerHelper.Core.Models;

/// <summary>
/// Stores user preferences and app settings.
/// </summary>
public class UserPreferences
{
    public int Id { get; set; }

    /// <summary>
    /// Whether the user has completed onboarding.
    /// </summary>
    public bool HasCompletedOnboarding { get; set; }

    /// <summary>
    /// User's buying goal/timeline.
    /// </summary>
    public BuyingGoal BuyingGoal { get; set; } = BuyingGoal.Exploring;

    /// <summary>
    /// Number of properties the user plans to compare (1, 2-5, 5+).
    /// </summary>
    public PropertyCountRange PropertyCountRange { get; set; } = PropertyCountRange.TwoToFive;

    /// <summary>
    /// Household size for space requirements.
    /// </summary>
    public int HouseholdSize { get; set; } = 2;

    /// <summary>
    /// Whether there are children in the household.
    /// </summary>
    public bool HasChildren { get; set; }

    /// <summary>
    /// Whether there are pets in the household.
    /// </summary>
    public bool HasPets { get; set; }

    /// <summary>
    /// Work arrangement affecting commute needs.
    /// </summary>
    public WorkArrangement WorkArrangement { get; set; } = WorkArrangement.Hybrid;

    /// <summary>
    /// Whether the user prioritizes location.
    /// </summary>
    public bool PrioritizesLocation { get; set; }

    /// <summary>
    /// Whether the user prioritizes size.
    /// </summary>
    public bool PrioritizesSize { get; set; }

    /// <summary>
    /// Whether the user prioritizes condition/updates.
    /// </summary>
    public bool PrioritizesCondition { get; set; }

    /// <summary>
    /// Whether the user prioritizes price.
    /// </summary>
    public bool PrioritizesPrice { get; set; }

    /// <summary>
    /// Preferred currency for display.
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Whether to use dark mode (legacy; superseded by ThemePreference).
    /// </summary>
    public bool UseDarkMode { get; set; }

    /// <summary>
    /// Theme selection: 0 = follow system, 1 = light, 2 = dark (P4-DRK-002).
    /// </summary>
    public int ThemePreference { get; set; }

    /// <summary>
    /// Whether to enable notifications.
    /// </summary>
    public bool EnableNotifications { get; set; } = true;

    /// <summary>
    /// Default down payment percentage for calculations.
    /// </summary>
    public decimal DefaultDownPaymentPercent { get; set; } = 20m;

    /// <summary>
    /// Default mortgage interest rate for calculations.
    /// </summary>
    public decimal DefaultInterestRate { get; set; } = 7.0m;

    /// <summary>
    /// Default mortgage term in years.
    /// </summary>
    public int DefaultMortgageTerm { get; set; } = 30;

    /// <summary>
    /// Default annual property tax rate (percentage).
    /// </summary>
    public decimal DefaultPropertyTaxRate { get; set; } = 0.96m;

    /// <summary>
    /// Default monthly insurance estimate.
    /// </summary>
    public decimal DefaultMonthlyInsurance { get; set; } = 125m;

    /// <summary>
    /// Current emergency fund balance for budget projections.
    /// </summary>
    public decimal EmergencyFundBalance { get; set; }

    /// <summary>
    /// Emergency fund target expressed in months of expenses. Default 6.
    /// </summary>
    public int EmergencyFundTargetMonths { get; set; } = 6;

    /// <summary>
    /// Primary commute destination (work address) for commute analysis.
    /// </summary>
    public string? WorkAddress { get; set; }

    /// <summary>
    /// What the user values their time at, per hour. Default $100 (spec 2.5.2).
    /// </summary>
    public decimal TimeValueHourlyRate { get; set; } = 100m;

    /// <summary>
    /// Working days per month for commute calculations. Default 22.
    /// </summary>
    public int WorkdaysPerMonth { get; set; } = 22;

    /// <summary>
    /// Tax filing status for funding strategy calculations.
    /// </summary>
    public TaxFilingStatus FilingStatus { get; set; } = TaxFilingStatus.Single;

    /// <summary>
    /// Estimated taxable income, used to determine the marginal bracket.
    /// </summary>
    public decimal EstimatedTaxableIncome { get; set; }

    /// <summary>
    /// State marginal income tax rate (percent). Entered manually since
    /// state schedules vary; 0 for no-income-tax states.
    /// </summary>
    public decimal StateMarginalTaxRate { get; set; }

    /// <summary>
    /// The property the user has designated as their plan target, if any.
    /// Resolved by PlanService into the integrated "My Plan" picture.
    /// </summary>
    public int? TargetPropertyId { get; set; }

    /// <summary>
    /// The offer scenario the user has designated as their plan, if any.
    /// Belongs to <see cref="TargetPropertyId"/>.
    /// </summary>
    public int? TargetOfferScenarioId { get; set; }

    /// <summary>
    /// The month the user expects to close on the target property. Drives the
    /// "rent until then, own after" cash flow projection. Null defaults to next month.
    /// </summary>
    public DateTime? TargetPurchaseMonth { get; set; }

    /// <summary>
    /// When preferences were created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When preferences were last modified.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Federal tax filing status.
/// </summary>
public enum TaxFilingStatus
{
    Single,
    MarriedFilingJointly,
    MarriedFilingSeparately,
    HeadOfHousehold
}

/// <summary>
/// User's home buying goal/timeline.
/// </summary>
public enum BuyingGoal
{
    Exploring,           // "Just exploring, no timeline"
    WithinYear,          // "Looking to buy within a year"
    ActivelySearching,   // "Actively searching now"
    MadeOffer,           // "Already made an offer"
    UnderContract        // "Under contract"
}

/// <summary>
/// Range of properties the user plans to compare.
/// </summary>
public enum PropertyCountRange
{
    One,        // Evaluating a single property
    TwoToFive,  // Comparing 2-5 properties
    MoreThanFive // Comparing 5+ properties
}

/// <summary>
/// User's work arrangement affecting commute needs.
/// </summary>
public enum WorkArrangement
{
    FullyRemote,    // No commute needed
    Hybrid,         // Some commute days
    FullyOnsite,    // Daily commute
    Retired,        // Not working
    Other
}
