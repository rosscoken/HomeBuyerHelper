using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Settings;

/// <summary>
/// ViewModel for tax bracket configuration (P3-FUN-002).
/// </summary>
public partial class TaxSettingsViewModel : BaseViewModel
{
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly ITaxImpactService _taxImpactService;

    [ObservableProperty]
    private int _filingStatusIndex;

    [ObservableProperty]
    private decimal _estimatedTaxableIncome;

    [ObservableProperty]
    private decimal _stateMarginalTaxRate;

    [ObservableProperty]
    private decimal _federalMarginalRate;

    [ObservableProperty]
    private decimal _capitalGainsRate;

    public IReadOnlyList<string> FilingStatusNames { get; } = new[]
    {
        "Single",
        "Married Filing Jointly",
        "Married Filing Separately",
        "Head of Household"
    };

    public TaxSettingsViewModel(
        IUserPreferencesRepository preferencesRepository,
        ITaxImpactService taxImpactService)
    {
        _preferencesRepository = preferencesRepository;
        _taxImpactService = taxImpactService;
        Title = "Tax Settings";
    }

    public override async Task OnAppearingAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var preferences = await _preferencesRepository.GetAsync();
            FilingStatusIndex = (int)preferences.FilingStatus;
            EstimatedTaxableIncome = preferences.EstimatedTaxableIncome;
            StateMarginalTaxRate = preferences.StateMarginalTaxRate;
            UpdateRates();
        });
    }

    partial void OnFilingStatusIndexChanged(int value) => UpdateRates();
    partial void OnEstimatedTaxableIncomeChanged(decimal value) => UpdateRates();

    private void UpdateRates()
    {
        var status = (TaxFilingStatus)Math.Clamp(FilingStatusIndex, 0, 3);
        FederalMarginalRate = _taxImpactService.GetMarginalFederalRate(status, EstimatedTaxableIncome);
        CapitalGainsRate = _taxImpactService.GetLongTermCapitalGainsRate(status, EstimatedTaxableIncome);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var preferences = await _preferencesRepository.GetAsync();
            preferences.FilingStatus = (TaxFilingStatus)Math.Clamp(FilingStatusIndex, 0, 3);
            preferences.EstimatedTaxableIncome = EstimatedTaxableIncome;
            preferences.StateMarginalTaxRate = StateMarginalTaxRate;
            await _preferencesRepository.SaveAsync(preferences);

            await Shell.Current.GoToAsync("..");
        });
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
