using Xunit;
using FluentAssertions;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Data.Repositories;

namespace HomeBuyerHelper.Data.Tests;

/// <summary>
/// Integration tests for Phase 3 repositories (funding, photos, pros/cons)
/// and new Phase 3 fields on existing tables.
/// </summary>
public sealed class Phase3RepositoryTests : IDisposable
{
    private readonly string _dbPath;
    private readonly DatabaseService _databaseService;

    public Phase3RepositoryTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"hbh_p3_test_{Guid.NewGuid():N}.db");
        _databaseService = new DatabaseService(_dbPath);
    }

    public void Dispose()
    {
        if (File.Exists(_dbPath))
        {
            File.Delete(_dbPath);
        }
    }

    [Fact]
    public async Task FundingRepository_RoundTripsTaxFields()
    {
        var repository = new FundingRepository(_databaseService);

        var id = await repository.CreateAsync(new FundingSource
        {
            Name = "Fidelity Brokerage",
            CurrentAmount = 110_000m,
            FundingType = FundingType.Investment,
            CostBasis = 55_000m,
            IsLongTermHolding = true,
            OwnerAge = 38,
            IsFirstTimeBuyer = true,
            RothContributionPortion = null,
            Is401kLoan = false,
            DonorName = null
        });

        var loaded = await repository.GetByIdAsync(id);

        loaded.Should().NotBeNull();
        loaded!.FundingType.Should().Be(FundingType.Investment);
        loaded.CostBasis.Should().Be(55_000m);
        loaded.IsLongTermHolding.Should().BeTrue();
        loaded.OwnerAge.Should().Be(38);
        loaded.IsFirstTimeBuyer.Should().BeTrue();
    }

    [Fact]
    public async Task PropertyRepository_RoundTripsCommuteAndUtilities()
    {
        var repository = new PropertyRepository(_databaseService);

        var id = await repository.CreateAsync(new Property
        {
            Nickname = "Commuter House",
            CommuteMinutesPrimary = 55,
            CommuteMinutesSecondary = 30,
            MonthlyUtilities = 275m
        });

        var loaded = await repository.GetByIdAsync(id);

        loaded!.CommuteMinutesPrimary.Should().Be(55);
        loaded.CommuteMinutesSecondary.Should().Be(30);
        loaded.MonthlyUtilities.Should().Be(275m);
    }

    [Fact]
    public async Task PreferencesRepository_RoundTripsCommuteAndTaxConfig()
    {
        var repository = new UserPreferencesRepository(_databaseService);

        var preferences = await repository.GetAsync();
        preferences.TimeValueHourlyRate.Should().Be(100m);
        preferences.WorkdaysPerMonth.Should().Be(22);

        preferences.WorkAddress = "100 Main St, Seattle WA";
        preferences.TimeValueHourlyRate = 85m;
        preferences.WorkdaysPerMonth = 20;
        preferences.FilingStatus = TaxFilingStatus.MarriedFilingJointly;
        preferences.EstimatedTaxableIncome = 180_000m;
        preferences.StateMarginalTaxRate = 4.5m;
        await repository.SaveAsync(preferences);

        var reloaded = await repository.GetAsync();
        reloaded.WorkAddress.Should().Be("100 Main St, Seattle WA");
        reloaded.TimeValueHourlyRate.Should().Be(85m);
        reloaded.WorkdaysPerMonth.Should().Be(20);
        reloaded.FilingStatus.Should().Be(TaxFilingStatus.MarriedFilingJointly);
        reloaded.EstimatedTaxableIncome.Should().Be(180_000m);
        reloaded.StateMarginalTaxRate.Should().Be(4.5m);
    }

    [Fact]
    public async Task PhotoRepository_CrudAndOrdering()
    {
        var repository = new PhotoRepository(_databaseService);

        await repository.CreateAsync(new PropertyPhoto { PropertyId = 1, FilePath = "/a.jpg", SortOrder = 1 });
        await repository.CreateAsync(new PropertyPhoto { PropertyId = 1, FilePath = "/b.jpg", SortOrder = 0 });
        await repository.CreateAsync(new PropertyPhoto { PropertyId = 2, FilePath = "/c.jpg", SortOrder = 0 });

        var photos = await repository.GetByPropertyIdAsync(1);
        photos.Should().HaveCount(2);
        photos[0].FilePath.Should().Be("/b.jpg");

        await repository.DeleteByPropertyIdAsync(1);
        (await repository.GetByPropertyIdAsync(1)).Should().BeEmpty();
        (await repository.GetByPropertyIdAsync(2)).Should().HaveCount(1);
    }

    [Fact]
    public async Task ProConRepository_CrudAndFiltering()
    {
        var repository = new ProConRepository(_databaseService);

        await repository.CreateAsync(new PropertyProCon { PropertyId = 1, IsPro = true, Description = "Great light" });
        await repository.CreateAsync(new PropertyProCon { PropertyId = 1, IsPro = false, Description = "Busy street" });

        var items = await repository.GetByPropertyIdAsync(1);
        items.Should().HaveCount(2);
        items.Count(i => i.IsPro).Should().Be(1);

        var con = items.Single(i => !i.IsPro);
        await repository.DeleteAsync(con.Id);
        (await repository.GetByPropertyIdAsync(1)).Should().ContainSingle(i => i.IsPro);
    }
}
