using FluentAssertions;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Core.Services;
using Xunit;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for OfferPlanningService — offer structure evaluation and comparison.
/// </summary>
public class OfferPlanningServiceTests
{
    private readonly CalculationService _calculationService = new();
    private readonly OfferPlanningService _service;

    public OfferPlanningServiceTests()
    {
        _service = new OfferPlanningService(_calculationService);
    }

    private static Property House(decimal asking = 500_000m) => new()
    {
        Nickname = "Test House",
        AskingPrice = asking
    };

    private static OfferScenario Offer(decimal price = 500_000m) => new()
    {
        Name = "Base",
        PropertyId = 1,
        OfferPrice = price,
        DownPaymentPercent = 20m,
        InterestRate = 7.0m,
        TermYears = 30
    };

    [Fact]
    public void Evaluate_CalculatesCoreStructure()
    {
        var result = _service.Evaluate(Offer(), House());

        result.DownPayment.Should().Be(100_000m);
        result.LoanAmount.Should().Be(400_000m);
        result.EffectiveInterestRate.Should().Be(7.0m);
        result.MonthlyPrincipalAndInterest.Should().BeApproximately(2_661m, 2m);
        result.MonthlyPMI.Should().Be(0);
        result.CashToClose.Should().BeGreaterThan(result.DownPayment);
        result.TotalInterest.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Evaluate_LowDownPayment_AddsPMI()
    {
        var offer = Offer();
        offer.DownPaymentPercent = 10m;

        var result = _service.Evaluate(offer, House());

        // 450k loan × 0.5%/yr ÷ 12
        result.MonthlyPMI.Should().Be(187.50m);
    }

    [Fact]
    public void Evaluate_DiscountPoints_LowerRateAndReportBreakeven()
    {
        var offer = Offer();
        offer.DiscountPoints = 2m;

        var result = _service.Evaluate(offer, House());

        result.EffectiveInterestRate.Should().Be(6.5m);
        result.PointsCost.Should().Be(8_000m); // 2% of 400k loan

        var paymentWithoutPoints = _calculationService.CalculateMonthlyMortgagePayment(400_000m, 7.0m, 30);
        var expectedBreakeven = (int)Math.Ceiling(
            8_000m / (paymentWithoutPoints - result.MonthlyPrincipalAndInterest));
        result.PointsBreakevenMonths.Should().Be(expectedBreakeven);
    }

    [Fact]
    public void Evaluate_NoPoints_HasNoBreakeven()
    {
        _service.Evaluate(Offer(), House()).PointsBreakevenMonths.Should().BeNull();
    }

    [Fact]
    public void Evaluate_SellerCredit_ReducesCashToClose()
    {
        var plain = _service.Evaluate(Offer(), House());

        var withCredit = Offer();
        withCredit.SellerCredit = 10_000m;
        var result = _service.Evaluate(withCredit, House());

        result.SellerCreditApplied.Should().Be(10_000m);
        result.SellerCreditWasCapped.Should().BeFalse();
        result.CashToClose.Should().Be(plain.CashToClose - 10_000m);
    }

    [Fact]
    public void Evaluate_SellerCredit_CappedByConcessionLimit()
    {
        // Under 10% down, conventional concessions cap at 3% of price. Use a
        // price low enough that fixed closing fees exceed the cap ($3k on
        // $100k) so the concession limit is what binds.
        var offer = Offer(100_000m);
        offer.DownPaymentPercent = 5m;
        offer.SellerCredit = 10_000m;

        var result = _service.Evaluate(offer, House(100_000m));

        result.EstimatedClosingCosts.Should().BeGreaterThan(3_000m);
        result.SellerCreditApplied.Should().Be(3_000m);
        result.SellerCreditWasCapped.Should().BeTrue();
    }

    [Fact]
    public void Evaluate_CreditsCannotExceedSettlementCosts()
    {
        // 25%+ down allows 9% concessions (45k), but credits can never push
        // cash to close below the down payment itself.
        var offer = Offer();
        offer.DownPaymentPercent = 30m;
        offer.SellerCredit = 45_000m;

        var result = _service.Evaluate(offer, House());

        result.SellerCreditApplied.Should().Be(result.EstimatedClosingCosts);
        result.CashToClose.Should().Be(result.DownPayment);
    }

    [Fact]
    public void Evaluate_AllCash_HasNoLoanCostsAndSkipsLoanFees()
    {
        var cash = Offer();
        cash.DownPaymentPercent = 100m;

        var financed = _service.Evaluate(Offer(), House());
        var result = _service.Evaluate(cash, House());

        result.LoanAmount.Should().Be(0);
        result.MonthlyPrincipalAndInterest.Should().Be(0);
        result.MonthlyPMI.Should().Be(0);
        result.TotalInterest.Should().Be(0);
        result.EstimatedClosingCosts.Should().BeLessThan(financed.EstimatedClosingCosts);
        result.CashToClose.Should().Be(500_000m + result.EstimatedClosingCosts);
    }

    [Fact]
    public void Evaluate_UsesPropertyTaxWhenSet_OtherwiseDefaultRate()
    {
        var house = House();
        house.AnnualPropertyTax = 6_000m;
        _service.Evaluate(Offer(), house).MonthlyPropertyTax.Should().Be(500m);

        // No tax on the property → 0.96% of price / 12 = 400.
        _service.Evaluate(Offer(), House()).MonthlyPropertyTax.Should().Be(400m);
    }

    [Fact]
    public void Evaluate_ReportsPositionVsAsking()
    {
        var result = _service.Evaluate(Offer(490_000m), House(500_000m));

        result.VsAskingAmount.Should().Be(-10_000m);
        result.VsAskingPercent.Should().Be(-2.0m);
    }

    [Fact]
    public void StrengthScore_RewardsCompetitiveStructures()
    {
        var weak = Offer(475_000m);
        weak.DownPaymentPercent = 5m;
        weak.SellerCredit = 10_000m;
        weak.ClosingDays = 60;

        var strong = Offer(510_000m);
        strong.DownPaymentPercent = 100m;
        strong.WaiveInspection = true;
        strong.WaiveAppraisal = true;
        strong.EarnestMoney = 20_000m;
        strong.ClosingDays = 14;

        var house = House();
        var weakScore = _service.Evaluate(weak, house).StrengthScore;
        var strongScore = _service.Evaluate(strong, house).StrengthScore;

        strongScore.Should().BeGreaterThan(weakScore);
        strongScore.Should().BeGreaterThanOrEqualTo(80);
        weakScore.Should().BeLessThan(40);
    }

    [Fact]
    public void StrengthScore_EscalationClauseCountsTowardPrice()
    {
        var flat = Offer(500_000m);
        var escalating = Offer(500_000m);
        escalating.EscalationMaxPrice = 525_000m;

        var house = House();
        _service.Evaluate(escalating, house).StrengthScore
            .Should().BeGreaterThan(_service.Evaluate(flat, house).StrengthScore);
    }

    [Fact]
    public void Compare_OrdersStrongestFirst()
    {
        var lowball = Offer(450_000m);
        lowball.Name = "Lowball";
        var atAsk = Offer(500_000m);
        atAsk.Name = "At ask";
        var aggressive = Offer(515_000m);
        aggressive.Name = "Aggressive";
        aggressive.WaiveInspection = true;

        var results = _service.Compare(new[] { lowball, aggressive, atAsk }, House());

        results.Select(r => r.Offer.Name).Should().ContainInOrder("Aggressive", "At ask", "Lowball");
    }

    [Fact]
    public void BuildDefaultOffer_UsesPreferencesAndAskingPrice()
    {
        var house = House(650_000m);
        house.Id = 42;
        var preferences = new UserPreferences
        {
            DefaultDownPaymentPercent = 25m,
            DefaultInterestRate = 6.25m,
            DefaultMortgageTerm = 15
        };

        var offer = _service.BuildDefaultOffer(house, preferences);

        offer.PropertyId.Should().Be(42);
        offer.OfferPrice.Should().Be(650_000m);
        offer.DownPaymentPercent.Should().Be(25m);
        offer.InterestRate.Should().Be(6.25m);
        offer.TermYears.Should().Be(15);
        offer.EarnestMoney.Should().Be(6_500m); // 1% of price
    }
}
