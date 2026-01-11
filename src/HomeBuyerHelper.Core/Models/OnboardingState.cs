namespace HomeBuyerHelper.Core.Models;

/// <summary>
/// Tracks the state of the onboarding flow.
/// </summary>
public class OnboardingState
{
    public int Id { get; set; }

    /// <summary>
    /// Current step in the onboarding flow.
    /// </summary>
    public int CurrentStep { get; set; } = 1;

    /// <summary>
    /// User's situation/goal selection.
    /// </summary>
    public BuyingSituation? BuyingSituation { get; set; }

    /// <summary>
    /// Property count range being considered.
    /// </summary>
    public PropertyCountRange? PropertyCount { get; set; }

    /// <summary>
    /// Who will be living in the home.
    /// </summary>
    public HouseholdType? HouseholdType { get; set; }

    /// <summary>
    /// Work arrangement for the household.
    /// </summary>
    public WorkArrangement? WorkArrangement { get; set; }

    /// <summary>
    /// Pet types in the household (flags enum).
    /// </summary>
    public PetType Pets { get; set; } = PetType.None;

    /// <summary>
    /// Selected location priorities (up to 3).
    /// </summary>
    public List<string> LocationPriorities { get; set; } = new();

    /// <summary>
    /// Selected home priorities (up to 3).
    /// </summary>
    public List<string> HomePriorities { get; set; } = new();

    /// <summary>
    /// Combined and ranked priorities from location and home selections.
    /// </summary>
    public List<string> RankedPriorities { get; set; } = new();

    /// <summary>
    /// Selected criteria with their weights.
    /// </summary>
    public List<CriterionSelection> SelectedCriteria { get; set; } = new();

    /// <summary>
    /// When the onboarding was started.
    /// </summary>
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When the onboarding was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// What best describes the user's situation.
/// </summary>
public enum BuyingSituation
{
    FirstHome,              // "Buying my first home"
    Upgrading,              // "Upgrading to a bigger/better home"
    Downsizing,             // "Downsizing or simplifying"
    Relocating,             // "Relocating to a new area"
    InvestmentProperty      // "Buying an investment property"
}

/// <summary>
/// Household composition type.
/// </summary>
public enum HouseholdType
{
    JustMe,                 // "Just me"
    WithPartner,            // "Me and a partner/spouse"
    FamilyWithKids,         // "Family with kids"
    Roommates,              // "Roommates"
    MultiGenerational       // "Multi-generational household"
}

/// <summary>
/// Types of pets (flags for multi-select).
/// </summary>
[Flags]
public enum PetType
{
    None = 0,
    Dogs = 1,
    Cats = 2,
    Other = 4
}

/// <summary>
/// A criterion selected during onboarding with its weight.
/// </summary>
public class CriterionSelection
{
    /// <summary>
    /// Name of the criterion.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Category of the criterion.
    /// </summary>
    public CriterionCategory Category { get; set; }

    /// <summary>
    /// Weight percentage (1-100, should sum to 100).
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// Whether this weight is locked during auto-rebalancing.
    /// </summary>
    public bool IsLocked { get; set; }

    /// <summary>
    /// Description of what a score of 1-2 means.
    /// </summary>
    public string? ScoreAnchorLow { get; set; }

    /// <summary>
    /// Description of what a score of 9-10 means.
    /// </summary>
    public string? ScoreAnchorHigh { get; set; }

    /// <summary>
    /// Reason this criterion was suggested.
    /// </summary>
    public string? SuggestionReason { get; set; }
}
