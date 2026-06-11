using CommunityToolkit.Maui;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Services;
using HomeBuyerHelper.Data;
using HomeBuyerHelper.Data.Repositories;
using HomeBuyerHelper.Pages;
using HomeBuyerHelper.Pages.Budget;
using HomeBuyerHelper.Pages.Funding;
using HomeBuyerHelper.Pages.Onboarding;
using HomeBuyerHelper.Pages.Settings;
using HomeBuyerHelper.ViewModels;
using HomeBuyerHelper.ViewModels.Budget;
using HomeBuyerHelper.ViewModels.Funding;
using HomeBuyerHelper.ViewModels.Settings;
using HomeBuyerHelper.ViewModels.Onboarding;
using Microsoft.Extensions.Logging;

namespace HomeBuyerHelper;

/// <summary>
/// MAUI application entry point and dependency injection configuration.
/// </summary>
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Register database service (singleton - shared across app)
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<IDatabaseService>(sp => sp.GetRequiredService<DatabaseService>());

        // Register repositories (singleton - one instance per type)
        builder.Services.AddSingleton<IPropertyRepository, PropertyRepository>();
        builder.Services.AddSingleton<ICriteriaRepository, CriteriaRepository>();
        builder.Services.AddSingleton<IScoreRepository, ScoreRepository>();
        builder.Services.AddSingleton<IUserPreferencesRepository, UserPreferencesRepository>();
        builder.Services.AddSingleton<IIncomeRepository, IncomeRepository>();
        builder.Services.AddSingleton<IExpenseRepository, ExpenseRepository>();
        builder.Services.AddSingleton<IOneTimeEventRepository, OneTimeEventRepository>();
        builder.Services.AddSingleton<IFundingRepository, FundingRepository>();
        builder.Services.AddSingleton<IPhotoRepository, PhotoRepository>();
        builder.Services.AddSingleton<IProConRepository, ProConRepository>();

        // Register platform abstractions
        builder.Services.AddSingleton<IKeyValueStore, Services.MauiPreferencesStore>();
        builder.Services.AddSingleton<IPhotoService, Services.PhotoService>();
        builder.Services.AddSingleton<ICloudSyncService, Services.ShareBackupSyncService>();

        // Register business services
        builder.Services.AddSingleton<IPropertyService, PropertyService>();
        builder.Services.AddSingleton<ICalculationService, CalculationService>();
        builder.Services.AddSingleton<IExportService, ExportService>();
        builder.Services.AddSingleton<IOnboardingStateService, OnboardingStateService>();
        builder.Services.AddSingleton<IWeightBalancingService, WeightBalancingService>();
        builder.Services.AddSingleton<IIncomeScenarioService, IncomeScenarioService>();
        builder.Services.AddSingleton<ICashFlowProjectionService, CashFlowProjectionService>();
        builder.Services.AddSingleton<IAffordabilityService, AffordabilityService>();
        builder.Services.AddSingleton<ICommuteValueService, CommuteValueService>();
        builder.Services.AddSingleton<ITrueTotalCostService, TrueTotalCostService>();
        builder.Services.AddSingleton<ITaxImpactService, TaxImpactService>();

        // Register main view models (transient - new instance per page)
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<PropertyListViewModel>();
        builder.Services.AddTransient<PropertyDetailViewModel>();
        builder.Services.AddTransient<CriteriaViewModel>();
        builder.Services.AddTransient<CriterionEditViewModel>();
        builder.Services.AddTransient<ComparisonViewModel>();
        builder.Services.AddTransient<ScoringWalkthroughViewModel>();
        builder.Services.AddTransient<LoanSettingsViewModel>();
        builder.Services.AddTransient<DataManagementViewModel>();

        // Register budget view models
        builder.Services.AddTransient<BudgetOverviewViewModel>();
        builder.Services.AddTransient<IncomeSetupViewModel>();
        builder.Services.AddTransient<IncomeEditViewModel>();
        builder.Services.AddTransient<ExpenseSetupViewModel>();
        builder.Services.AddTransient<ExpenseEditViewModel>();
        builder.Services.AddTransient<OneTimeEventsViewModel>();
        builder.Services.AddTransient<OneTimeEventEditViewModel>();
        builder.Services.AddTransient<CashFlowTimelineViewModel>();

        // Register funding and settings view models
        builder.Services.AddTransient<FundingSetupViewModel>();
        builder.Services.AddTransient<FundingEditViewModel>();
        builder.Services.AddTransient<TaxSettingsViewModel>();
        builder.Services.AddTransient<CommuteSettingsViewModel>();
        builder.Services.AddTransient<SyncSettingsViewModel>();

        // Register onboarding view models
        builder.Services.AddTransient<WelcomeViewModel>();
        builder.Services.AddTransient<GoalSelectionViewModel>();
        builder.Services.AddTransient<PropertyCountViewModel>();
        builder.Services.AddTransient<HouseholdViewModel>();
        builder.Services.AddTransient<LocationPrioritiesViewModel>();
        builder.Services.AddTransient<HomePrioritiesViewModel>();
        builder.Services.AddTransient<CriteriaReviewViewModel>();
        builder.Services.AddTransient<OnboardingCompleteViewModel>();

        // Register main pages (transient - new instance per navigation)
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<PropertyListPage>();
        builder.Services.AddTransient<PropertyDetailPage>();
        builder.Services.AddTransient<CriteriaPage>();
        builder.Services.AddTransient<CriterionEditPage>();
        builder.Services.AddTransient<ComparisonPage>();
        builder.Services.AddTransient<ScoringWalkthroughPage>();
        builder.Services.AddTransient<LoanSettingsPage>();
        builder.Services.AddTransient<DataManagementPage>();

        // Register budget pages
        builder.Services.AddTransient<BudgetPage>();
        builder.Services.AddTransient<IncomeSetupPage>();
        builder.Services.AddTransient<IncomeEditPage>();
        builder.Services.AddTransient<ExpenseSetupPage>();
        builder.Services.AddTransient<ExpenseEditPage>();
        builder.Services.AddTransient<OneTimeEventsPage>();
        builder.Services.AddTransient<OneTimeEventEditPage>();
        builder.Services.AddTransient<CashFlowTimelinePage>();

        // Register funding and settings pages
        builder.Services.AddTransient<FundingSetupPage>();
        builder.Services.AddTransient<FundingEditPage>();
        builder.Services.AddTransient<TaxSettingsPage>();
        builder.Services.AddTransient<CommuteSettingsPage>();
        builder.Services.AddTransient<SyncSettingsPage>();

        // Register onboarding pages
        builder.Services.AddTransient<WelcomePage>();
        builder.Services.AddTransient<GoalSelectionPage>();
        builder.Services.AddTransient<PropertyCountPage>();
        builder.Services.AddTransient<HouseholdPage>();
        builder.Services.AddTransient<LocationPrioritiesPage>();
        builder.Services.AddTransient<HomePrioritiesPage>();
        builder.Services.AddTransient<CriteriaReviewPage>();
        builder.Services.AddTransient<OnboardingCompletePage>();

        return builder.Build();
    }
}
