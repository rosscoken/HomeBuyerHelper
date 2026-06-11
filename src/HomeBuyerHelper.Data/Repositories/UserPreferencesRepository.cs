using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Data.Entities;

namespace HomeBuyerHelper.Data.Repositories;

/// <summary>
/// SQLite implementation of IUserPreferencesRepository.
/// </summary>
public class UserPreferencesRepository : IUserPreferencesRepository
{
    private readonly DatabaseService _databaseService;

    public UserPreferencesRepository(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<UserPreferences> GetAsync()
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = await db.Table<UserPreferencesEntity>().FirstOrDefaultAsync();

        if (entity == null)
        {
            // Create default preferences
            var defaults = CreateDefaults();
            await SaveAsync(defaults);
            return defaults;
        }

        return MapToModel(entity);
    }

    public async Task SaveAsync(UserPreferences preferences)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = MapToEntity(preferences);
        entity.UpdatedAt = DateTime.UtcNow;

        var existing = await db.Table<UserPreferencesEntity>().FirstOrDefaultAsync();
        if (existing != null)
        {
            entity.Id = existing.Id;
            entity.CreatedAt = existing.CreatedAt;
            await db.UpdateAsync(entity);
        }
        else
        {
            entity.CreatedAt = DateTime.UtcNow;
            await db.InsertAsync(entity);
        }
    }

    public async Task ResetAsync()
    {
        var db = await _databaseService.GetConnectionAsync();
        await db.DeleteAllAsync<UserPreferencesEntity>();
        await SaveAsync(CreateDefaults());
    }

    public async Task<bool> HasCompletedOnboardingAsync()
    {
        var preferences = await GetAsync();
        return preferences.HasCompletedOnboarding;
    }

    public async Task SetOnboardingCompleteAsync()
    {
        var preferences = await GetAsync();
        preferences.HasCompletedOnboarding = true;
        await SaveAsync(preferences);
    }

    private static UserPreferences CreateDefaults() => new()
    {
        HasCompletedOnboarding = false,
        BuyingGoal = BuyingGoal.Exploring,
        PropertyCountRange = PropertyCountRange.TwoToFive,
        HouseholdSize = 2,
        HasChildren = false,
        HasPets = false,
        WorkArrangement = WorkArrangement.Hybrid,
        PrioritizesLocation = false,
        PrioritizesSize = false,
        PrioritizesCondition = false,
        PrioritizesPrice = false,
        Currency = "USD",
        UseDarkMode = false,
        EnableNotifications = true,
        DefaultDownPaymentPercent = 20m,
        DefaultInterestRate = 7.0m,
        DefaultMortgageTerm = 30,
        DefaultPropertyTaxRate = 0.96m,
        DefaultMonthlyInsurance = 125m,
        EmergencyFundBalance = 0m,
        EmergencyFundTargetMonths = 6,
        TimeValueHourlyRate = 100m,
        WorkdaysPerMonth = 22,
        FilingStatus = TaxFilingStatus.Single,
        EstimatedTaxableIncome = 0m,
        StateMarginalTaxRate = 0m
    };

    private static UserPreferences MapToModel(UserPreferencesEntity entity) => new()
    {
        Id = entity.Id,
        HasCompletedOnboarding = entity.HasCompletedOnboarding,
        BuyingGoal = (BuyingGoal)entity.BuyingGoal,
        PropertyCountRange = (PropertyCountRange)entity.PropertyCountRange,
        HouseholdSize = entity.HouseholdSize,
        HasChildren = entity.HasChildren,
        HasPets = entity.HasPets,
        WorkArrangement = (WorkArrangement)entity.WorkArrangement,
        PrioritizesLocation = entity.PrioritizesLocation,
        PrioritizesSize = entity.PrioritizesSize,
        PrioritizesCondition = entity.PrioritizesCondition,
        PrioritizesPrice = entity.PrioritizesPrice,
        Currency = entity.Currency,
        UseDarkMode = entity.UseDarkMode,
        EnableNotifications = entity.EnableNotifications,
        DefaultDownPaymentPercent = entity.DefaultDownPaymentPercent,
        DefaultInterestRate = entity.DefaultInterestRate,
        DefaultMortgageTerm = entity.DefaultMortgageTerm,
        DefaultPropertyTaxRate = entity.DefaultPropertyTaxRate,
        DefaultMonthlyInsurance = entity.DefaultMonthlyInsurance,
        EmergencyFundBalance = entity.EmergencyFundBalance,
        EmergencyFundTargetMonths = entity.EmergencyFundTargetMonths,
        WorkAddress = entity.WorkAddress,
        TimeValueHourlyRate = entity.TimeValueHourlyRate,
        WorkdaysPerMonth = entity.WorkdaysPerMonth,
        FilingStatus = (TaxFilingStatus)entity.FilingStatus,
        EstimatedTaxableIncome = entity.EstimatedTaxableIncome,
        StateMarginalTaxRate = entity.StateMarginalTaxRate,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    private static UserPreferencesEntity MapToEntity(UserPreferences model) => new()
    {
        Id = model.Id,
        HasCompletedOnboarding = model.HasCompletedOnboarding,
        BuyingGoal = (int)model.BuyingGoal,
        PropertyCountRange = (int)model.PropertyCountRange,
        HouseholdSize = model.HouseholdSize,
        HasChildren = model.HasChildren,
        HasPets = model.HasPets,
        WorkArrangement = (int)model.WorkArrangement,
        PrioritizesLocation = model.PrioritizesLocation,
        PrioritizesSize = model.PrioritizesSize,
        PrioritizesCondition = model.PrioritizesCondition,
        PrioritizesPrice = model.PrioritizesPrice,
        Currency = model.Currency,
        UseDarkMode = model.UseDarkMode,
        EnableNotifications = model.EnableNotifications,
        DefaultDownPaymentPercent = model.DefaultDownPaymentPercent,
        DefaultInterestRate = model.DefaultInterestRate,
        DefaultMortgageTerm = model.DefaultMortgageTerm,
        DefaultPropertyTaxRate = model.DefaultPropertyTaxRate,
        DefaultMonthlyInsurance = model.DefaultMonthlyInsurance,
        EmergencyFundBalance = model.EmergencyFundBalance,
        EmergencyFundTargetMonths = model.EmergencyFundTargetMonths,
        WorkAddress = model.WorkAddress,
        TimeValueHourlyRate = model.TimeValueHourlyRate,
        WorkdaysPerMonth = model.WorkdaysPerMonth,
        FilingStatus = (int)model.FilingStatus,
        EstimatedTaxableIncome = model.EstimatedTaxableIncome,
        StateMarginalTaxRate = model.StateMarginalTaxRate,
        CreatedAt = model.CreatedAt,
        UpdatedAt = model.UpdatedAt
    };
}
