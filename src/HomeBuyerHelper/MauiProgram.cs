using CommunityToolkit.Maui;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Services;
using HomeBuyerHelper.Data;
using HomeBuyerHelper.Data.Repositories;
using HomeBuyerHelper.Pages;
using HomeBuyerHelper.ViewModels;
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

        // Register business services
        builder.Services.AddSingleton<IPropertyService, PropertyService>();
        builder.Services.AddSingleton<ICalculationService, CalculationService>();
        builder.Services.AddSingleton<IExportService, ExportService>();

        // Register view models (transient - new instance per page)
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<PropertyListViewModel>();
        builder.Services.AddTransient<PropertyDetailViewModel>();
        builder.Services.AddTransient<OnboardingViewModel>();
        builder.Services.AddTransient<CriteriaViewModel>();
        builder.Services.AddTransient<ComparisonViewModel>();

        // Register pages (transient - new instance per navigation)
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<PropertyListPage>();
        builder.Services.AddTransient<PropertyDetailPage>();
        builder.Services.AddTransient<OnboardingPage>();
        builder.Services.AddTransient<CriteriaPage>();
        builder.Services.AddTransient<ComparisonPage>();

        return builder.Build();
    }
}
