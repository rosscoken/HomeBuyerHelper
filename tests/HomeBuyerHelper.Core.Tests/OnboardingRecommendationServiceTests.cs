using FluentAssertions;
using HomeBuyerHelper.Core.Data;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Core.Services;
using Xunit;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for OnboardingRecommendationService (M5 onboarding-lite).
/// </summary>
public class OnboardingRecommendationServiceTests
{
    private readonly OnboardingRecommendationService _service = new();

    [Fact]
    public void Recommend_InvestmentGoal_RecommendsInvestmentFocused()
    {
        var result = _service.Recommend(
            BuyingGoal.ActivelySearching,
            WorkArrangement.FullyOnsite,
            hasChildren: false,
            PriorityFocus.Price,
            isInvestmentGoal: true);

        result.TemplateKey.Should().Be("InvestmentFocused");
        CommonCriteria.Templates.Should().ContainKey(result.TemplateKey);
        result.Reason.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Recommend_FullyRemoteWork_RecommendsRemoteWorker()
    {
        var result = _service.Recommend(
            BuyingGoal.Exploring,
            WorkArrangement.FullyRemote,
            hasChildren: false,
            PriorityFocus.Condition);

        result.TemplateKey.Should().Be("RemoteWorker");
        CommonCriteria.Templates.Should().ContainKey(result.TemplateKey);
    }

    [Fact]
    public void Recommend_HasChildren_RecommendsFamilyFocused()
    {
        var result = _service.Recommend(
            BuyingGoal.WithinYear,
            WorkArrangement.Hybrid,
            hasChildren: true,
            PriorityFocus.Location);

        result.TemplateKey.Should().Be("FamilyFocused");
        CommonCriteria.Templates.Should().ContainKey(result.TemplateKey);
    }

    [Fact]
    public void Recommend_NoSignals_RecommendsFirstTimeBuyer()
    {
        var result = _service.Recommend(
            BuyingGoal.Exploring,
            WorkArrangement.Hybrid,
            hasChildren: false,
            PriorityFocus.Space);

        result.TemplateKey.Should().Be("FirstTimeBuyer");
        CommonCriteria.Templates.Should().ContainKey(result.TemplateKey);
    }

    [Fact]
    public void Recommend_InvestmentGoal_TakesPrecedenceOverRemoteAndChildren()
    {
        var result = _service.Recommend(
            BuyingGoal.ActivelySearching,
            WorkArrangement.FullyRemote,
            hasChildren: true,
            PriorityFocus.Location,
            isInvestmentGoal: true);

        result.TemplateKey.Should().Be("InvestmentFocused");
    }

    [Fact]
    public void Recommend_FullyRemote_TakesPrecedenceOverChildren()
    {
        var result = _service.Recommend(
            BuyingGoal.Exploring,
            WorkArrangement.FullyRemote,
            hasChildren: true,
            PriorityFocus.Condition);

        result.TemplateKey.Should().Be("RemoteWorker");
    }

    [Fact]
    public void Recommend_AlwaysReturnsAValidTemplateKey()
    {
        foreach (BuyingGoal goal in Enum.GetValues<BuyingGoal>())
        {
            foreach (WorkArrangement work in Enum.GetValues<WorkArrangement>())
            {
                foreach (var hasChildren in new[] { true, false })
                {
                    foreach (PriorityFocus focus in Enum.GetValues<PriorityFocus>())
                    {
                        var result = _service.Recommend(goal, work, hasChildren, focus);
                        CommonCriteria.Templates.Should().ContainKey(result.TemplateKey);
                    }
                }
            }
        }
    }
}
