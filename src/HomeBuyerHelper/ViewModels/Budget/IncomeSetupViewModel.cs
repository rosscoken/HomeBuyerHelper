using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Budget;

/// <summary>
/// ViewModel for the income source list (P2-INC-001).
/// </summary>
public partial class IncomeSetupViewModel : BaseViewModel
{
    private readonly IIncomeRepository _incomeRepository;
    private readonly IIncomeScenarioService _incomeScenarioService;

    [ObservableProperty]
    private ObservableCollection<IncomeSource> _incomeSources = new();

    [ObservableProperty]
    private decimal _conservativeMonthly;

    [ObservableProperty]
    private decimal _realisticMonthly;

    [ObservableProperty]
    private decimal _expectedMonthly;

    public IncomeSetupViewModel(
        IIncomeRepository incomeRepository,
        IIncomeScenarioService incomeScenarioService)
    {
        _incomeRepository = incomeRepository;
        _incomeScenarioService = incomeScenarioService;
        Title = "Income Sources";
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
            var sources = await _incomeRepository.GetAllAsync();
            IncomeSources = new ObservableCollection<IncomeSource>(sources);

            ConservativeMonthly = Math.Round(
                _incomeScenarioService.GetAverageMonthlyIncome(sources, IncomeScenario.Conservative), 0);
            RealisticMonthly = Math.Round(
                _incomeScenarioService.GetAverageMonthlyIncome(sources, IncomeScenario.Realistic), 0);
            ExpectedMonthly = Math.Round(
                _incomeScenarioService.GetAverageMonthlyIncome(sources, IncomeScenario.Expected), 0);
        });
    }

    [RelayCommand]
    private async Task AddNewAsync()
    {
        await Shell.Current.GoToAsync("IncomeEdit?id=0");
    }

    [RelayCommand]
    private async Task EditSourceAsync(IncomeSource source)
    {
        await Shell.Current.GoToAsync($"IncomeEdit?id={source.Id}");
    }

    [RelayCommand]
    private async Task DeleteSourceAsync(IncomeSource source)
    {
        var confirmed = await Shell.Current.DisplayAlert(
            "Delete Income Source",
            $"Delete '{source.Name}'?",
            "Delete",
            "Cancel");

        if (!confirmed) return;

        await ExecuteBusyAsync(async () =>
        {
            await _incomeRepository.DeleteAsync(source.Id);
        });
        await LoadDataAsync();
    }
}
