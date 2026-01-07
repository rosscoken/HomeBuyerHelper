using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Onboarding;

/// <summary>
/// ViewModel for the goal selection screen (P1-ONB-002).
/// </summary>
public partial class GoalSelectionViewModel : BaseViewModel
{
    private readonly IOnboardingStateService _stateService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private bool _isFirstHomeSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private bool _isUpgradingSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private bool _isDownsizingSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private bool _isRelocatingSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private bool _isInvestmentSelected;

    public bool HasSelection =>
        IsFirstHomeSelected || IsUpgradingSelected || IsDownsizingSelected ||
        IsRelocatingSelected || IsInvestmentSelected;

    public GoalSelectionViewModel(IOnboardingStateService stateService)
    {
        _stateService = stateService;
        Title = "Your Situation";
        LoadState();
    }

    private void LoadState()
    {
        var state = _stateService.GetState();
        switch (state.BuyingSituation)
        {
            case BuyingSituation.FirstHome:
                IsFirstHomeSelected = true;
                break;
            case BuyingSituation.Upgrading:
                IsUpgradingSelected = true;
                break;
            case BuyingSituation.Downsizing:
                IsDownsizingSelected = true;
                break;
            case BuyingSituation.Relocating:
                IsRelocatingSelected = true;
                break;
            case BuyingSituation.InvestmentProperty:
                IsInvestmentSelected = true;
                break;
        }
    }

    private BuyingSituation? GetSelectedSituation()
    {
        if (IsFirstHomeSelected) return BuyingSituation.FirstHome;
        if (IsUpgradingSelected) return BuyingSituation.Upgrading;
        if (IsDownsizingSelected) return BuyingSituation.Downsizing;
        if (IsRelocatingSelected) return BuyingSituation.Relocating;
        if (IsInvestmentSelected) return BuyingSituation.InvestmentProperty;
        return null;
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task ContinueAsync()
    {
        var state = _stateService.GetState();
        state.BuyingSituation = GetSelectedSituation();
        state.CurrentStep = 2;
        _stateService.SaveState(state);

        await Shell.Current.GoToAsync("PropertyCount");
    }
}
