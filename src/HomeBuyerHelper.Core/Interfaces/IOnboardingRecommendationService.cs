using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Recommends a starter criteria template for the web "onboarding-lite"
/// quiz (M5). A small, pure mapping — heavier personalization stays in
/// <see cref="CommonCriteria.GetSuggestedCriteria"/> for the full MAUI
/// onboarding flow.
/// </summary>
public interface IOnboardingRecommendationService
{
    /// <summary>
    /// Recommends a template key from <see cref="Data.CommonCriteria.Templates"/>
    /// based on four quiz answers. Precedence: an investment goal wins over
    /// everything else, then fully-remote work, then having children, else a
    /// first-time-buyer default.
    /// </summary>
    /// <param name="goal">The buyer's stated timeline/situation.</param>
    /// <param name="work">The buyer's work arrangement.</param>
    /// <param name="hasChildren">Whether there are children in the household.</param>
    /// <param name="focus">What the buyer says matters most.</param>
    /// <param name="isInvestmentGoal">
    /// Whether the buyer is shopping for an investment property. Kept as a
    /// separate flag rather than a <see cref="BuyingGoal"/> value because
    /// that enum only models timeline/stage (Exploring, WithinYear,
    /// ActivelySearching, MadeOffer, UnderContract) and has no "investment"
    /// case.
    /// </param>
    TemplateRecommendation Recommend(
        BuyingGoal goal,
        WorkArrangement work,
        bool hasChildren,
        PriorityFocus focus,
        bool isInvestmentGoal = false);
}

/// <summary>
/// The four top-level priorities offered on the onboarding-lite quiz,
/// mirroring <see cref="UserPreferences.PrioritizesLocation"/>,
/// <see cref="UserPreferences.PrioritizesSize"/>,
/// <see cref="UserPreferences.PrioritizesCondition"/>, and
/// <see cref="UserPreferences.PrioritizesPrice"/>.
/// </summary>
public enum PriorityFocus
{
    Location,
    Space,
    Condition,
    Price
}

/// <summary>
/// A recommended starter template plus a one-sentence explanation of why.
/// </summary>
public class TemplateRecommendation
{
    /// <summary>Key into <see cref="Data.CommonCriteria.Templates"/>.</summary>
    public required string TemplateKey { get; init; }

    /// <summary>Human-readable reason shown next to the recommendation.</summary>
    public required string Reason { get; init; }
}
