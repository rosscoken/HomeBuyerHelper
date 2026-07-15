using System.Globalization;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Services;
using HomeBuyerHelper.Web;
using HomeBuyerHelper.Web.Storage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

// WASM runs with invariant globalization, which renders "C0" amounts with
// the generic ¤ symbol. Dollar amounts are the app's lingua franca, so give
// the invariant culture a $ without shipping full ICU data.
var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
culture.NumberFormat.CurrencySymbol = "$";
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// localStorage-backed repositories (web preview stand-ins for SQLite)
builder.Services.AddSingleton<IPropertyRepository, LocalPropertyRepository>();
builder.Services.AddSingleton<ICriteriaRepository, LocalCriteriaRepository>();
builder.Services.AddSingleton<IScoreRepository, LocalScoreRepository>();
builder.Services.AddSingleton<IUserPreferencesRepository, LocalUserPreferencesRepository>();
builder.Services.AddSingleton<IIncomeRepository, LocalIncomeRepository>();
builder.Services.AddSingleton<IExpenseRepository, LocalExpenseRepository>();
builder.Services.AddSingleton<IOneTimeEventRepository, LocalOneTimeEventRepository>();
builder.Services.AddSingleton<IFundingRepository, LocalFundingRepository>();
builder.Services.AddSingleton<IPhotoRepository, LocalPhotoRepository>();
builder.Services.AddSingleton<IProConRepository, LocalProConRepository>();
builder.Services.AddSingleton<IOfferScenarioRepository, LocalOfferScenarioRepository>();
builder.Services.AddSingleton<WebBackupService>();

// Core business services (shared with the native app)
builder.Services.AddSingleton<IPropertyService, PropertyService>();
builder.Services.AddSingleton<ICalculationService, CalculationService>();
builder.Services.AddSingleton<IWeightBalancingService, WeightBalancingService>();
builder.Services.AddSingleton<IIncomeScenarioService, IncomeScenarioService>();
builder.Services.AddSingleton<ICashFlowProjectionService, CashFlowProjectionService>();
builder.Services.AddSingleton<IAffordabilityService, AffordabilityService>();
builder.Services.AddSingleton<ICommuteValueService, CommuteValueService>();
builder.Services.AddSingleton<ITrueTotalCostService, TrueTotalCostService>();
builder.Services.AddSingleton<ITaxImpactService, TaxImpactService>();
builder.Services.AddSingleton<IRentVsBuyService, RentVsBuyService>();
builder.Services.AddSingleton<IScenarioService, ScenarioService>();
builder.Services.AddSingleton<IOfferPlanningService, OfferPlanningService>();
builder.Services.AddSingleton<ICriteriaTemplateService, CriteriaTemplateService>();
builder.Services.AddSingleton<IPlanService, PlanService>();
builder.Services.AddSingleton<IOnboardingRecommendationService, OnboardingRecommendationService>();

await builder.Build().RunAsync();
