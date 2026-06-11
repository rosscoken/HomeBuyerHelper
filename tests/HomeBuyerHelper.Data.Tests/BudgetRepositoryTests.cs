using Xunit;
using FluentAssertions;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Data.Repositories;

namespace HomeBuyerHelper.Data.Tests;

/// <summary>
/// Integration tests for the budget repositories (income, expenses, events).
/// </summary>
public sealed class BudgetRepositoryTests : IDisposable
{
    private readonly string _dbPath;
    private readonly DatabaseService _databaseService;
    private readonly IncomeRepository _incomeRepository;
    private readonly ExpenseRepository _expenseRepository;
    private readonly OneTimeEventRepository _eventRepository;

    public BudgetRepositoryTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"hbh_budget_test_{Guid.NewGuid():N}.db");
        _databaseService = new DatabaseService(_dbPath);
        _incomeRepository = new IncomeRepository(_databaseService);
        _expenseRepository = new ExpenseRepository(_databaseService);
        _eventRepository = new OneTimeEventRepository(_databaseService);
    }

    public void Dispose()
    {
        if (File.Exists(_dbPath))
        {
            File.Delete(_dbPath);
        }
    }

    [Fact]
    public async Task IncomeRepository_RoundTripsAllFields()
    {
        var source = new IncomeSource
        {
            Name = "Annual Bonus",
            GrossAmount = 25_000m,
            Frequency = IncomeFrequency.Annually,
            IncomeType = IncomeType.Bonus,
            IsReliable = false,
            PaymentMonth = 3,
            Probability = 75m,
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2028, 12, 31),
            Notes = "Target bonus"
        };

        var id = await _incomeRepository.CreateAsync(source);
        var loaded = await _incomeRepository.GetByIdAsync(id);

        loaded.Should().NotBeNull();
        loaded!.Name.Should().Be("Annual Bonus");
        loaded.IncomeType.Should().Be(IncomeType.Bonus);
        loaded.IsReliable.Should().BeFalse();
        loaded.PaymentMonth.Should().Be(3);
        loaded.Probability.Should().Be(75m);
        loaded.StartDate.Should().Be(new DateTime(2026, 1, 1));
        loaded.EndDate.Should().Be(new DateTime(2028, 12, 31));
    }

    [Fact]
    public async Task IncomeRepository_UpdateAndDelete()
    {
        var id = await _incomeRepository.CreateAsync(new IncomeSource
        {
            Name = "Salary",
            GrossAmount = 8_000m,
            Frequency = IncomeFrequency.Monthly
        });

        var loaded = await _incomeRepository.GetByIdAsync(id);
        loaded!.GrossAmount = 9_000m;
        await _incomeRepository.UpdateAsync(loaded);

        (await _incomeRepository.GetByIdAsync(id))!.GrossAmount.Should().Be(9_000m);

        await _incomeRepository.DeleteAsync(id);
        (await _incomeRepository.GetByIdAsync(id)).Should().BeNull();
    }

    [Fact]
    public async Task ExpenseRepository_RoundTripsVariableFlag()
    {
        var fixedId = await _expenseRepository.CreateAsync(new Expense
        {
            Name = "Car Payment",
            Amount = 450m,
            Category = ExpenseCategory.Transportation,
            IsVariable = false
        });
        var variableId = await _expenseRepository.CreateAsync(new Expense
        {
            Name = "Groceries",
            Amount = 800m,
            Category = ExpenseCategory.Food,
            IsVariable = true
        });

        var all = await _expenseRepository.GetAllAsync();

        all.Should().HaveCount(2);
        all.Single(e => e.Id == fixedId).IsVariable.Should().BeFalse();
        all.Single(e => e.Id == variableId).IsVariable.Should().BeTrue();
    }

    [Fact]
    public async Task OneTimeEventRepository_OrdersByDate()
    {
        await _eventRepository.CreateAsync(new OneTimeEvent
        {
            Name = "Later",
            Amount = 100m,
            Date = new DateTime(2026, 9, 1),
            Category = OneTimeEventCategory.Travel
        });
        await _eventRepository.CreateAsync(new OneTimeEvent
        {
            Name = "Sooner",
            Amount = 200m,
            Date = new DateTime(2026, 3, 1),
            Category = OneTimeEventCategory.Moving
        });

        var all = await _eventRepository.GetAllAsync();

        all.Should().HaveCount(2);
        all[0].Name.Should().Be("Sooner");
        all[1].Name.Should().Be("Later");
        all[0].Category.Should().Be(OneTimeEventCategory.Moving);
    }

    [Fact]
    public async Task UserPreferences_RoundTripsEmergencyFundFields()
    {
        var preferencesRepository = new UserPreferencesRepository(_databaseService);

        var preferences = await preferencesRepository.GetAsync();
        preferences.EmergencyFundTargetMonths.Should().Be(6);

        preferences.EmergencyFundBalance = 15_000m;
        preferences.EmergencyFundTargetMonths = 9;
        await preferencesRepository.SaveAsync(preferences);

        var reloaded = await preferencesRepository.GetAsync();
        reloaded.EmergencyFundBalance.Should().Be(15_000m);
        reloaded.EmergencyFundTargetMonths.Should().Be(9);
    }
}
