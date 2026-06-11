using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Services;
using HomeBuyerHelper.Web;
using HomeBuyerHelper.Web.Storage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

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
builder.Services.AddSingleton<ICriteriaTemplateService, CriteriaTemplateService>();

await builder.Build().RunAsync();
