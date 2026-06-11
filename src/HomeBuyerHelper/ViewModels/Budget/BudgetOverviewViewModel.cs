using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Budget;

/// <summary>
/// ViewModel for the budget overview hub (P2-INC-001 entry point).
/// Shows scenario income, expense totals, and emergency fund status with
/// navigation to the detailed setup pages.
/// </summary>
public partial class BudgetOverviewViewModel : BaseViewModel
{
    private readonly IIncomeRepository _incomeRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IOneTimeEventRepository _eventRepository;
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly IIncomeScenarioService _incomeScenarioService;

    [ObservableProperty]
    private decimal _conservativeIncome;

    [ObservableProperty]
    private decimal _realisticIncome;

    [ObservableProperty]
    private decimal _expectedIncome;

    [ObservableProperty]
    private decimal _monthlyExpenses;

    [ObservableProperty]
    private int _incomeSourceCount;

    [ObservableProperty]
    private int _expenseCount;

    [ObservableProperty]
    private int _eventCount;

    [ObservableProperty]
    private decimal _emergencyFundBalance;

    [ObservableProperty]
    private int _emergencyFundTargetMonths = 6;

    [ObservableProperty]
    private decimal _emergencyFundTarget;

    [ObservableProperty]
    private string _emergencyFundStatus = string.Empty;

    [ObservableProperty]
    private Color _emergencyFundColor = Colors.Gray;

    public BudgetOverviewViewModel(
        IIncomeRepository incomeRepository,
        IExpenseRepository expenseRepository,
        IOneTimeEventRepository eventRepository,
        IUserPreferencesRepository preferencesRepository,
        IIncomeScenarioService incomeScenarioService)
    {
        _incomeRepository = incomeRepository;
        _expenseRepository = expenseRepository;
        _eventRepository = eventRepository;
        _preferencesRepository = preferencesRepository;
        _incomeScenarioService = incomeScenarioService;
        Title = "Budget";
    }

    public override async Task OnAppearingAsync()
    {
        await LoadDataAsync();
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

            IncomeSourceCount = incomeSources.Count;
            ExpenseCount = expenses.Count;
            EventCount = events.Count;

            ConservativeIncome = Math.Round(
                _incomeScenarioService.GetAverageMonthlyIncome(incomeSources, IncomeScenario.Conservative), 0);
            RealisticIncome = Math.Round(
                _incomeScenarioService.GetAverageMonthlyIncome(incomeSources, IncomeScenario.Realistic), 0);
            ExpectedIncome = Math.Round(
                _incomeScenarioService.GetAverageMonthlyIncome(incomeSources, IncomeScenario.Expected), 0);

            MonthlyExpenses = Math.Round(expenses.Sum(e => e.MonthlyAmount), 0);

            EmergencyFundBalance = preferences.EmergencyFundBalance;
            EmergencyFundTargetMonths = preferences.EmergencyFundTargetMonths;
            EmergencyFundTarget = MonthlyExpenses * preferences.EmergencyFundTargetMonths;

            UpdateEmergencyFundStatus();
        });
    }

    private void UpdateEmergencyFundStatus()
    {
        if (MonthlyExpenses <= 0)
        {
            EmergencyFundStatus = "Add expenses to calculate your target";
            EmergencyFundColor = Colors.Gray;
            return;
        }

        var monthsCovered = EmergencyFundBalance / MonthlyExpenses;
        EmergencyFundStatus = $"{monthsCovered:F1} months of expenses covered";
        EmergencyFundColor = monthsCovered >= EmergencyFundTargetMonths ? Colors.Green
            : monthsCovered >= 3 ? Colors.Orange
            : Colors.Red;
    }

    [RelayCommand]
    private async Task SaveEmergencyFundAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var preferences = await _preferencesRepository.GetAsync();
            preferences.EmergencyFundBalance = EmergencyFundBalance;
            preferences.EmergencyFundTargetMonths = EmergencyFundTargetMonths;
            await _preferencesRepository.SaveAsync(preferences);

            EmergencyFundTarget = MonthlyExpenses * EmergencyFundTargetMonths;
            UpdateEmergencyFundStatus();
        });
    }

    [RelayCommand]
    private async Task NavigateToIncomeAsync()
    {
        await Shell.Current.GoToAsync("IncomeSetup");
    }

    [RelayCommand]
    private async Task NavigateToExpensesAsync()
    {
        await Shell.Current.GoToAsync("ExpenseSetup");
    }

    [RelayCommand]
    private async Task NavigateToEventsAsync()
    {
        await Shell.Current.GoToAsync("OneTimeEvents");
    }

    [RelayCommand]
    private async Task NavigateToCashFlowAsync()
    {
        await Shell.Current.GoToAsync("CashFlowTimeline");
    }
}
