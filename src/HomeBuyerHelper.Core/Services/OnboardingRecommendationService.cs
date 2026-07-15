using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Services;

/// <summary>
/// Pure mapping from onboarding-lite quiz answers to a starter criteria
/// template (M5). Template keys match <see cref="Data.CommonCriteria.Templates"/>
/// exactly: "InvestmentFocused", "RemoteWorker", "FamilyFocused", "FirstTimeBuyer".
/// </summary>
public class OnboardingRecommendationService : IOnboardingRecommendationService
{
    public TemplateRecommendation Recommend(
        BuyingGoal goal,
        WorkArrangement work,
        bool hasChildren,
        PriorityFocus focus,
        bool isInvestmentGoal = false)
    {
        if (isInvestmentGoal)
        {
            return new TemplateRecommendation
            {
                TemplateKey = "InvestmentFocused",
                Reason = "You're buying for investment, so this template weights appreciation, " +
                         "value, and rental potential over personal comfort" + FocusClause(focus, forInvestment: true) + "."
            };
        }

        if (work == WorkArrangement.FullyRemote)
        {
            return new TemplateRecommendation
            {
                TemplateKey = "RemoteWorker",
                Reason = "You work fully remote, so this template weights home office space, " +
                         "internet quality, and quiet over commute time" + FocusClause(focus) + "."
            };
        }

        if (hasChildren)
        {
            return new TemplateRecommendation
            {
                TemplateKey = "FamilyFocused",
                Reason = "You have kids at home, so this template weights schools, yard space, " +
                         "and safety" + FocusClause(focus) + "."
            };
        }

        return new TemplateRecommendation
        {
            TemplateKey = "FirstTimeBuyer",
            Reason = $"{GoalClause(goal)}this balanced starter template weights budget, safety, " +
                     "and move-in readiness" + FocusClause(focus) + "."
        };
    }

    /// <summary>Appends a clause noting the selected top priority, when it isn't already implied.</summary>
    private static string FocusClause(PriorityFocus focus, bool forInvestment = false)
    {
        if (forInvestment && focus == PriorityFocus.Price)
        {
            return string.Empty; // already covered by "value" above
        }

        return focus switch
        {
            PriorityFocus.Location => " — with an extra nudge toward location, since that's what matters most to you",
            PriorityFocus.Space => " — with an extra nudge toward space and layout, since that's what matters most to you",
            PriorityFocus.Condition => " — with an extra nudge toward move-in condition, since that's what matters most to you",
            PriorityFocus.Price => " — with an extra nudge toward price, since that's what matters most to you",
            _ => string.Empty
        };
    }

    /// <summary>Phrases the buyer's stated timeline as a lead-in for the default recommendation.</summary>
    private static string GoalClause(BuyingGoal goal) => goal switch
    {
        BuyingGoal.WithinYear => "You're looking to buy within a year, so ",
        BuyingGoal.ActivelySearching => "You're actively searching now, so ",
        BuyingGoal.MadeOffer => "You've already made an offer, so ",
        BuyingGoal.UnderContract => "You're under contract, so ",
        _ => "You're just exploring for now, so "
    };
}
