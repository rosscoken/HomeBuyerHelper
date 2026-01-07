using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;

namespace HomeBuyerHelper.ViewModels;

/// <summary>
/// View model for loan configuration settings.
/// </summary>
public partial class LoanSettingsViewModel : BaseViewModel
{
    private readonly IUserPreferencesRepository _preferencesRepository;

    [ObservableProperty]
    private decimal _downPaymentPercent = 20m;

    [ObservableProperty]
    private decimal _interestRate = 7.0m;

    [ObservableProperty]
    private int _mortgageTermYears = 30;

    [ObservableProperty]
    private decimal _propertyTaxRate = 0.96m;

    [ObservableProperty]
    private decimal _monthlyInsurance = 125m;

    public IReadOnlyList<int> TermOptions { get; } = new[] { 15, 20, 30 };

    public LoanSettingsViewModel(IUserPreferencesRepository preferencesRepository)
    {
        _preferencesRepository = preferencesRepository;
        Title = "Loan Settings";
    }

    public override async Task OnAppearingAsync()
    {
        await LoadSettingsAsync();
    }

    [RelayCommand]
    private async Task LoadSettingsAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var prefs = await _preferencesRepository.GetAsync();
            DownPaymentPercent = prefs.DefaultDownPaymentPercent;
            InterestRate = prefs.DefaultInterestRate;
            MortgageTermYears = prefs.DefaultMortgageTerm;
            PropertyTaxRate = prefs.DefaultPropertyTaxRate;
            MonthlyInsurance = prefs.DefaultMonthlyInsurance;
        });
    }

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        if (!ValidateInputs())
            return;

        await ExecuteBusyAsync(async () =>
        {
            var prefs = await _preferencesRepository.GetAsync();
            prefs.DefaultDownPaymentPercent = DownPaymentPercent;
            prefs.DefaultInterestRate = InterestRate;
            prefs.DefaultMortgageTerm = MortgageTermYears;
            prefs.DefaultPropertyTaxRate = PropertyTaxRate;
            prefs.DefaultMonthlyInsurance = MonthlyInsurance;
            await _preferencesRepository.SaveAsync(prefs);
            await Shell.Current.GoToAsync("..");
        });
    }

    [RelayCommand]
    private async Task ResetToDefaultsAsync()
    {
        DownPaymentPercent = 20m;
        InterestRate = 7.0m;
        MortgageTermYears = 30;
        PropertyTaxRate = 0.96m;
        MonthlyInsurance = 125m;
        ClearError();
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private bool ValidateInputs()
    {
        if (DownPaymentPercent is < 5 or > 100)
        {
            SetError("Down payment must be between 5% and 100%.");
            return false;
        }

        if (InterestRate is < 0 or > 20)
        {
            SetError("Interest rate must be between 0% and 20%.");
            return false;
        }

        if (!TermOptions.Contains(MortgageTermYears))
        {
            SetError("Please select a valid loan term.");
            return false;
        }

        if (PropertyTaxRate is < 0 or > 5)
        {
            SetError("Property tax rate must be between 0% and 5%.");
            return false;
        }

        if (MonthlyInsurance < 0)
        {
            SetError("Monthly insurance cannot be negative.");
            return false;
        }

        ClearError();
        return true;
    }
}
