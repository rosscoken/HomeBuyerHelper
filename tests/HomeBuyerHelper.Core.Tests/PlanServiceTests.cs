using FluentAssertions;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Core.Services;
using NSubstitute;
using Xunit;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for PlanService — the "My Plan" resolver (M4). Repositories are
/// substituted; the offer, tax, and affordability services are real so the
/// snapshot math is exercised end to end.
/// </summary>
public class PlanServiceTests
{
    private readonly IUserPreferencesRepository _preferences = Substitute.For<IUserPreferencesRepository>();
    private readonly IPropertyRepository _properties = Substitute.For<IPropertyRepository>();
    private readonly IOfferScenarioRepository _offers = Substitute.For<IOfferScenarioRepository>();
    private readonly IFundingRepository _funding = Substitute.For<IFundingRepository>();
    private readonly IIncomeRepository _income = Substitute.For<IIncomeRepository>();

    private readonly PlanService _service;

    private UserPreferences _prefs = new();

    public PlanServiceTests()
    {
        var incomeScenario = new IncomeScenarioService();
        _service = new PlanService(
            _preferences,
            _properties,
            _offers,
            _funding,
            _income,
            new OfferPlanningService(new CalculationService()),
            new TaxImpactService(),
            new AffordabilityService(incomeScenario));

        _preferences.GetAsync().Returns(_ => _prefs);
        _preferences.When(p => p.SaveAsync(Arg.Any<UserPreferences>()))
            .Do(ci => _prefs = ci.Arg<UserPreferences>());

        _funding.GetAllAsync().Returns(Array.Empty<FundingSource>());
        _income.GetAllAsync().Returns(Array.Empty<IncomeSource>());
    }

    private static Property MakeProperty(int id = 1) => new()
    {
        Id = id,
        Nickname = "Craftsman on 5th",
        AskingPrice = 500_000m,
        MonthlyHOA = 0m
    };

    private static OfferScenario MakeOffer(int id = 10, int propertyId = 1) => new()
    {
        Id = id,
        PropertyId = propertyId,
        Name = "Full ask",
        OfferPrice = 500_000m,
        DownPaymentPercent = 20m,
        InterestRate = 7.0m,
        TermYears = 30
    };

    [Fact]
    public async Task GetSnapshotAsync_NoPlan_ReturnsNull()
    {
        _prefs = new UserPreferences { TargetPropertyId = null, TargetOfferScenarioId = null };

        var snapshot = await _service.GetSnapshotAsync();

        snapshot.Should().BeNull();
    }

    [Fact]
    public async Task GetSnapshotAsync_ResolvesOfferEvaluation()
    {
        var property = MakeProperty();
        var offer = MakeOffer();
        _prefs = new UserPreferences { TargetPropertyId = 1, TargetOfferScenarioId = 10 };
        _properties.GetByIdAsync(1).Returns(property);
        _offers.GetByIdAsync(10).Returns(offer);

        var snapshot = await _service.GetSnapshotAsync();

        snapshot.Should().NotBeNull();
        snapshot!.Property.Should().BeSameAs(property);
        snapshot.Offer.Should().BeSameAs(offer);
        snapshot.Evaluation.CashToClose.Should().BeGreaterThan(0);
        snapshot.Evaluation.TotalMonthlyCost.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetSnapshotAsync_ComputesFundingSurplus_NetOfTax()
    {
        var property = MakeProperty();
        var offer = MakeOffer();
        _prefs = new UserPreferences
        {
            TargetPropertyId = 1,
            TargetOfferScenarioId = 10,
            FilingStatus = TaxFilingStatus.Single,
            EstimatedTaxableIncome = 150_000m
        };
        _properties.GetByIdAsync(1).Returns(property);
        _offers.GetByIdAsync(10).Returns(offer);

        // Savings has no tax, so net == gross for this source.
        _funding.GetAllAsync().Returns(new[]
        {
            new FundingSource { Name = "Savings", CurrentAmount = 130_000m, FundingType = FundingType.Savings }
        });

        var snapshot = await _service.GetSnapshotAsync();

        snapshot.Should().NotBeNull();
        snapshot!.FundingNetAvailable.Should().Be(130_000m);
        snapshot.FundingSurplus.Should().Be(130_000m - snapshot.Evaluation.CashToClose);
        snapshot.FundingSurplus.Should().BeGreaterThan(0); // 130k covers ~100k+ cash to close
    }

    [Fact]
    public async Task GetSnapshotAsync_FundingSurplus_NegativeWhenShort()
    {
        var property = MakeProperty();
        var offer = MakeOffer();
        _prefs = new UserPreferences { TargetPropertyId = 1, TargetOfferScenarioId = 10 };
        _properties.GetByIdAsync(1).Returns(property);
        _offers.GetByIdAsync(10).Returns(offer);
        _funding.GetAllAsync().Returns(new[]
        {
            new FundingSource { Name = "Savings", CurrentAmount = 10_000m, FundingType = FundingType.Savings }
        });

        var snapshot = await _service.GetSnapshotAsync();

        snapshot!.FundingSurplus.Should().BeLessThan(0);
    }

    [Fact]
    public async Task GetSnapshotAsync_ComputesAffordabilityZonesPerScenario()
    {
        var property = MakeProperty();
        var offer = MakeOffer();
        _prefs = new UserPreferences { TargetPropertyId = 1, TargetOfferScenarioId = 10 };
        _properties.GetByIdAsync(1).Returns(property);
        _offers.GetByIdAsync(10).Returns(offer);

        // Reliable $12,000/mo salary → same under every scenario.
        _income.GetAllAsync().Returns(new[]
        {
            new IncomeSource
            {
                Name = "Salary",
                GrossAmount = 12_000m,
                Frequency = IncomeFrequency.Monthly,
                IsReliable = true
            }
        });

        var snapshot = await _service.GetSnapshotAsync();

        snapshot!.Affordability.Should().HaveCount(3);
        var realistic = snapshot.RealisticAffordability!;
        realistic.GrossMonthlyIncome.Should().Be(12_000m);

        // Housing % should match the hand-computed ratio for the evaluation.
        var expectedPct = Math.Round(snapshot.Evaluation.TotalMonthlyCost / 12_000m * 100, 2);
        realistic.HousingPercentage.Should().Be(expectedPct);
        realistic.Zone.Should().Be(new AffordabilityService(new IncomeScenarioService()).GetZone(expectedPct));
    }

    [Fact]
    public async Task GetSnapshotAsync_ZoneIsRisky_WhenNoIncome()
    {
        var property = MakeProperty();
        var offer = MakeOffer();
        _prefs = new UserPreferences { TargetPropertyId = 1, TargetOfferScenarioId = 10 };
        _properties.GetByIdAsync(1).Returns(property);
        _offers.GetByIdAsync(10).Returns(offer);
        _income.GetAllAsync().Returns(Array.Empty<IncomeSource>());

        var snapshot = await _service.GetSnapshotAsync();

        snapshot!.RealisticAffordability!.Zone.Should().Be(AffordabilityZone.Risky);
        snapshot.RealisticAffordability!.GrossMonthlyIncome.Should().Be(0);
    }

    [Fact]
    public async Task GetSnapshotAsync_MissingProperty_SelfHeals()
    {
        _prefs = new UserPreferences { TargetPropertyId = 1, TargetOfferScenarioId = 10 };
        _properties.GetByIdAsync(1).Returns((Property?)null);
        _offers.GetByIdAsync(10).Returns(MakeOffer());

        var snapshot = await _service.GetSnapshotAsync();

        snapshot.Should().BeNull();
        _prefs.TargetPropertyId.Should().BeNull();
        _prefs.TargetOfferScenarioId.Should().BeNull();
        await _preferences.Received().SaveAsync(Arg.Any<UserPreferences>());
    }

    [Fact]
    public async Task GetSnapshotAsync_MissingOffer_SelfHeals()
    {
        _prefs = new UserPreferences { TargetPropertyId = 1, TargetOfferScenarioId = 10 };
        _properties.GetByIdAsync(1).Returns(MakeProperty());
        _offers.GetByIdAsync(10).Returns((OfferScenario?)null);

        var snapshot = await _service.GetSnapshotAsync();

        snapshot.Should().BeNull();
        _prefs.TargetPropertyId.Should().BeNull();
        _prefs.TargetOfferScenarioId.Should().BeNull();
    }

    [Fact]
    public async Task GetSnapshotAsync_OfferBelongsToOtherProperty_SelfHeals()
    {
        _prefs = new UserPreferences { TargetPropertyId = 1, TargetOfferScenarioId = 10 };
        _properties.GetByIdAsync(1).Returns(MakeProperty(1));
        _offers.GetByIdAsync(10).Returns(MakeOffer(10, propertyId: 2));

        var snapshot = await _service.GetSnapshotAsync();

        snapshot.Should().BeNull();
        _prefs.TargetPropertyId.Should().BeNull();
    }

    [Fact]
    public async Task SetPlanAsync_PersistsIds()
    {
        _prefs = new UserPreferences();

        await _service.SetPlanAsync(3, 7);

        _prefs.TargetPropertyId.Should().Be(3);
        _prefs.TargetOfferScenarioId.Should().Be(7);
    }

    [Fact]
    public async Task ClearPlanAsync_ClearsIds()
    {
        _prefs = new UserPreferences { TargetPropertyId = 3, TargetOfferScenarioId = 7 };

        await _service.ClearPlanAsync();

        _prefs.TargetPropertyId.Should().BeNull();
        _prefs.TargetOfferScenarioId.Should().BeNull();
    }

    [Fact]
    public async Task SetPurchaseMonthAsync_PersistsMonth()
    {
        _prefs = new UserPreferences();
        var month = new DateTime(2026, 9, 1);

        await _service.SetPurchaseMonthAsync(month);

        _prefs.TargetPurchaseMonth.Should().Be(month);
    }
}
