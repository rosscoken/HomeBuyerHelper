using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Budget;

/// <summary>
/// ViewModel for the month-by-month cash flow timeline (P2-CFP-002).
/// </summary>
public partial class CashFlowTimelineViewModel : BaseViewModel
{
    private readonly IIncomeRepository _incomeRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IOneTimeEventRepository _eventRepository;
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly ICashFlowProjectionService _projectionService;

    [ObservableProperty]
    private ObservableCollection<MonthlyProjection> _projections = new();

    [ObservableProperty]
    private int _scenarioIndex = 1; // Default to Realistic

    [ObservableProperty]
    private int _crunchMonthCount;

    [ObservableProperty]
    private decimal _endingCumulative;

    [ObservableProperty]
    private bool _hasData;

    public IReadOnlyList<string> ScenarioNames { get; } = new[] { "Conservative", "Realistic", "Expected" };

    public CashFlowTimelineViewModel(
        IIncomeRepository incomeRepository,
        IExpenseRepository expenseRepository,
        IOneTimeEventRepository eventRepository,
        IUserPreferencesRepository preferencesRepository,
        ICashFlowProjectionService projectionService)
    {
        _incomeRepository = incomeRepository;
        _expenseRepository = expenseRepository;
        _eventRepository = eventRepository;
        _preferencesRepository = preferencesRepository;
        _projectionService = projectionService;
        Title = "Cash Flow";
    }

    public override async Task OnAppearingAsync()
    {
        await LoadDataAsync();
    }

    partial void OnScenarioIndexChanged(int value)
    {
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var incomeSources = await _incomeRepository.GetAllAsync();
            var expenses = await _expenseRepository.GetAllAsync();
            var events = await _eventRepository.GetAllAsync();
            var preferences = await _preferencesRepository.GetAsync();

            HasData = incomeSources.Count > 0 || expenses.Count > 0;

            var scenario = ScenarioIndex switch
            {
                0 => IncomeScenario.Conservative,
                2 => IncomeScenario.Expected,
                _ => IncomeScenario.Realistic
            };

            var result = _projectionService.Project(new CashFlowProjectionInput
            {
                IncomeSources = incomeSources,
                Expenses = expenses,
                OneTimeEvents = events,
                Scenario = scenario,
                EmergencyFund = new EmergencyFundConfig
                {
                    TargetMonths = preferences.EmergencyFundTargetMonths,
                    CurrentBalance = preferences.EmergencyFundBalance
                }
            });

            Projections = new ObservableCollection<MonthlyProjection>(result);
            CrunchMonthCount = result.Count(m => m.IsCrunchMonth);
            EndingCumulative = result.Count > 0 ? result[^1].CumulativeSurplus : 0;
        });
    }
}
