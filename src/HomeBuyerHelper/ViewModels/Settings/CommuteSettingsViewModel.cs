using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;

namespace HomeBuyerHelper.ViewModels.Settings;

/// <summary>
/// ViewModel for commute analysis configuration (P3-COM-001).
/// </summary>
public partial class CommuteSettingsViewModel : BaseViewModel
{
    private readonly IUserPreferencesRepository _preferencesRepository;

    [ObservableProperty]
    private string? _workAddress;

    [ObservableProperty]
    private decimal _timeValueHourlyRate = 100m;

    [ObservableProperty]
    private int _workdaysPerMonth = 22;

    public CommuteSettingsViewModel(IUserPreferencesRepository preferencesRepository)
    {
        _preferencesRepository = preferencesRepository;
        Title = "Commute Settings";
    }

    public override async Task OnAppearingAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var preferences = await _preferencesRepository.GetAsync();
            WorkAddress = preferences.WorkAddress;
            TimeValueHourlyRate = preferences.TimeValueHourlyRate;
            WorkdaysPerMonth = preferences.WorkdaysPerMonth;
        });
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (TimeValueHourlyRate < 0 || WorkdaysPerMonth is < 0 or > 31)
        {
            SetError("Please enter a valid hourly rate and workdays per month.");
            return;
        }

        await ExecuteBusyAsync(async () =>
        {
            var preferences = await _preferencesRepository.GetAsync();
            preferences.WorkAddress = string.IsNullOrWhiteSpace(WorkAddress) ? null : WorkAddress.Trim();
            preferences.TimeValueHourlyRate = TimeValueHourlyRate;
            preferences.WorkdaysPerMonth = WorkdaysPerMonth;
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
