using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Analysis;

/// <summary>
/// ViewModel for "what if" scenario planning (P5-SCN-001/002/003).
/// </summary>
public partial class ScenariosViewModel : BaseViewModel
{
    private readonly IScenarioService _scenarioService;
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly IPropertyService _propertyService;
    private readonly IIncomeRepository _incomeRepository;

    private IReadOnlyList<IncomeSource> _incomeSources = [];

    [ObservableProperty]
    private ObservableCollection<ScenarioResult> _results = new();

    // Editable scenario form
    [ObservableProperty]
    private string _scenarioName = "Scenario";

    [ObservableProperty]
    private decimal _purchasePrice;

    [ObservableProperty]
    private decimal _downPaymentPercent = 20m;

    [ObservableProperty]
    private decimal _interestRate = 7.0m;

    [ObservableProperty]
    private int _mortgageTermYears = 30;

    // Wait scenario inputs (P5-SCN-003)
    [ObservableProperty]
    private int _monthsToWait = 6;

    [ObservableProperty]
    private decimal _monthlySavings = 1_000m;

    [ObservableProperty]
    private decimal _marketChangePercent = 3m;

    [ObservableProperty]
    private decimal _currentRent = 2_000m;

    public ScenariosViewModel(
        IScenarioService scenarioService,
        IUserPreferencesRepository preferencesRepository,
        IPropertyService propertyService,
        IIncomeRepository incomeRepository)
    {
        _scenarioService = scenarioService;
        _preferencesRepository = preferencesRepository;
        _propertyService = propertyService;
        _incomeRepository = incomeRepository;
        Title = "Scenarios";
    }

    public override async Task OnAppearingAsync()
    {
        if (Results.Count > 0) return;

        await ExecuteBusyAsync(async () =>
        {
            _incomeSources = await _incomeRepository.GetAllAsync();
            var preferences = await _preferencesRepository.GetAsync();

            DownPaymentPercent = preferences.DefaultDownPaymentPercent;
            InterestRate = preferences.DefaultInterestRate;
            MortgageTermYears = preferences.DefaultMortgageTerm;

            // Base Case from the top-ranked property and current settings.
            var rankings = await _propertyService.GetPropertyRankingsAsync();
            var top = rankings.FirstOrDefault()?.Property;
            if (top != null && top.EffectivePrice > 0)
            {
                PurchasePrice = top.EffectivePrice;
                var baseCase = new PurchaseScenario
                {
                    Name = "Base Case",
                    Description = top.Nickname,
                    PurchasePrice = top.EffectivePrice,
                    DownPaymentPercent = preferences.DefaultDownPaymentPercent,
                    InterestRate = preferences.DefaultInterestRate,
                    MortgageTermYears = preferences.DefaultMortgageTerm
                };
                Results.Add(_scenarioService.Evaluate(baseCase, _incomeSources));
            }
        });
    }

    [RelayCommand]
    private void AddScenario()
    {
        if (PurchasePrice <= 0)
        {
            SetError("Enter a purchase price.");
            return;
        }

        ClearError();

        var scenario = new PurchaseScenario
        {
            Name = string.IsNullOrWhiteSpace(ScenarioName) ? $"Scenario {Results.Count + 1}" : ScenarioName.Trim(),
            PurchasePrice = PurchasePrice,
            DownPaymentPercent = DownPaymentPercent,
            InterestRate = InterestRate,
            MortgageTermYears = MortgageTermYears
        };

        Results.Add(_scenarioService.Evaluate(scenario, _incomeSources));
    }

    /// <summary>
    /// "What if we wait N months?" built from the selected scenario (P5-SCN-003).
    /// </summary>
    [RelayCommand]
    private void AddWaitScenario(ScenarioResult baseResult)
    {
        ClearError();

        var wait = _scenarioService.BuildWaitScenario(
            baseResult.Scenario,
            Math.Clamp(MonthsToWait, 1, 60),
            MonthlySavings,
            MarketChangePercent,
            CurrentRent);

        Results.Add(_scenarioService.Evaluate(wait, _incomeSources));
    }

    [RelayCommand]
    private void DuplicateScenario(ScenarioResult result)
    {
        ScenarioName = $"{result.Scenario.Name} (copy)";
        PurchasePrice = result.Scenario.PurchasePrice;
        DownPaymentPercent = result.Scenario.DownPaymentPercent;
        InterestRate = result.Scenario.InterestRate;
        MortgageTermYears = result.Scenario.MortgageTermYears;
    }

    [RelayCommand]
    private void RemoveScenario(ScenarioResult result)
    {
        Results.Remove(result);
    }
}
