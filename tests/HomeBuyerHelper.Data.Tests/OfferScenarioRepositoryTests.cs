using FluentAssertions;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Data.Repositories;
using Xunit;

namespace HomeBuyerHelper.Data.Tests;

/// <summary>
/// Integration tests for the offer scenario repository.
/// </summary>
public sealed class OfferScenarioRepositoryTests : IDisposable
{
    private readonly string _dbPath;
    private readonly DatabaseService _databaseService;
    private readonly OfferScenarioRepository _repository;

    public OfferScenarioRepositoryTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"hbh_offer_test_{Guid.NewGuid():N}.db");
        _databaseService = new DatabaseService(_dbPath);
        _repository = new OfferScenarioRepository(_databaseService);
    }

    public void Dispose()
    {
        if (File.Exists(_dbPath))
        {
            File.Delete(_dbPath);
        }
    }

    private static OfferScenario SampleOffer(int propertyId = 1) => new()
    {
        PropertyId = propertyId,
        Name = "Aggressive",
        OfferPrice = 512_000m,
        EscalationMaxPrice = 530_000m,
        DownPaymentPercent = 25m,
        InterestRate = 6.875m,
        TermYears = 30,
        DiscountPoints = 1.5m,
        SellerCredit = 5_000m,
        LenderCredit = 1_500m,
        EarnestMoney = 15_000m,
        WaiveInspection = true,
        WaiveFinancing = false,
        WaiveAppraisal = true,
        ClosingDays = 21,
        Notes = "Escalate in 2.5k steps"
    };

    [Fact]
    public async Task RoundTripsAllOfferFields()
    {
        var id = await _repository.CreateAsync(SampleOffer());

        var loaded = await _repository.GetByIdAsync(id);

        loaded.Should().NotBeNull();
        loaded!.Name.Should().Be("Aggressive");
        loaded.OfferPrice.Should().Be(512_000m);
        loaded.EscalationMaxPrice.Should().Be(530_000m);
        loaded.DownPaymentPercent.Should().Be(25m);
        loaded.InterestRate.Should().Be(6.875m);
        loaded.TermYears.Should().Be(30);
        loaded.DiscountPoints.Should().Be(1.5m);
        loaded.SellerCredit.Should().Be(5_000m);
        loaded.LenderCredit.Should().Be(1_500m);
        loaded.EarnestMoney.Should().Be(15_000m);
        loaded.WaiveInspection.Should().BeTrue();
        loaded.WaiveFinancing.Should().BeFalse();
        loaded.WaiveAppraisal.Should().BeTrue();
        loaded.ClosingDays.Should().Be(21);
        loaded.Notes.Should().Be("Escalate in 2.5k steps");
    }

    [Fact]
    public async Task FiltersAndDeletesByProperty()
    {
        await _repository.CreateAsync(SampleOffer(propertyId: 1));
        await _repository.CreateAsync(SampleOffer(propertyId: 1));
        await _repository.CreateAsync(SampleOffer(propertyId: 2));

        (await _repository.GetByPropertyIdAsync(1)).Should().HaveCount(2);
        (await _repository.GetAllAsync()).Should().HaveCount(3);

        await _repository.DeleteByPropertyIdAsync(1);

        (await _repository.GetByPropertyIdAsync(1)).Should().BeEmpty();
        (await _repository.GetByPropertyIdAsync(2)).Should().HaveCount(1);
    }

    [Fact]
    public async Task UpdatePersistsChanges()
    {
        var id = await _repository.CreateAsync(SampleOffer());
        var offer = await _repository.GetByIdAsync(id);

        offer!.OfferPrice = 520_000m;
        offer.WaiveInspection = false;
        await _repository.UpdateAsync(offer);

        var reloaded = await _repository.GetByIdAsync(id);
        reloaded!.OfferPrice.Should().Be(520_000m);
        reloaded.WaiveInspection.Should().BeFalse();
    }
}
