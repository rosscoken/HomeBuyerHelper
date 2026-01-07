using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels;

/// <summary>
/// View model for the main dashboard page.
/// </summary>
public partial class DashboardViewModel : BaseViewModel
{
    private readonly IPropertyService _propertyService;
    private readonly IUserPreferencesRepository _preferencesRepository;

    [ObservableProperty]
    private IReadOnlyList<PropertyRanking> _propertyRankings = [];

    [ObservableProperty]
    private int _propertyCount;

    [ObservableProperty]
    private bool _hasCompletedOnboarding;

    [ObservableProperty]
    private PropertyRanking? _topProperty;

    public DashboardViewModel(
        IPropertyService propertyService,
        IUserPreferencesRepository preferencesRepository)
    {
        _propertyService = propertyService;
        _preferencesRepository = preferencesRepository;
        Title = "Dashboard";
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
            HasCompletedOnboarding = await _preferencesRepository.HasCompletedOnboardingAsync();

            var rankings = await _propertyService.GetPropertyRankingsAsync();
            PropertyRankings = rankings;
            PropertyCount = rankings.Count;
            TopProperty = rankings.FirstOrDefault();
        });
    }

    [RelayCommand]
    private async Task NavigateToOnboardingAsync()
    {
        await Shell.Current.GoToAsync(nameof(Pages.OnboardingPage));
    }

    [RelayCommand]
    private async Task NavigateToPropertyListAsync()
    {
        await Shell.Current.GoToAsync(nameof(Pages.PropertyListPage));
    }

    [RelayCommand]
    private async Task NavigateToAddPropertyAsync()
    {
        await Shell.Current.GoToAsync(nameof(Pages.PropertyDetailPage));
    }

    [RelayCommand]
    private async Task NavigateToComparisonAsync()
    {
        await Shell.Current.GoToAsync(nameof(Pages.ComparisonPage));
    }

    [RelayCommand]
    private async Task NavigateToDataManagementAsync()
    {
        await Shell.Current.GoToAsync("DataManagement");
    }

    [RelayCommand]
    private async Task NavigateToLoanSettingsAsync()
    {
        await Shell.Current.GoToAsync("LoanSettings");
    }
}
