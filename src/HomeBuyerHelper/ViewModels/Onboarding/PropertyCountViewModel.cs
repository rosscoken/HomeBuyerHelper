using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Onboarding;

/// <summary>
/// ViewModel for the property count screen (P1-ONB-003).
/// </summary>
public partial class PropertyCountViewModel : BaseViewModel
{
    private readonly IOnboardingStateService _stateService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private bool _isJustStartingSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private bool _isFewOptionsSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private bool _isSeveralSelected;

    public bool HasSelection =>
        IsJustStartingSelected || IsFewOptionsSelected || IsSeveralSelected;

    public PropertyCountViewModel(IOnboardingStateService stateService)
    {
        _stateService = stateService;
        Title = "Properties";
        LoadState();
    }

    private void LoadState()
    {
        var state = _stateService.GetState();
        switch (state.PropertyCount)
        {
            case PropertyCountRange.One:
                IsJustStartingSelected = true;
                break;
            case PropertyCountRange.TwoToFive:
                IsFewOptionsSelected = true;
                break;
            case PropertyCountRange.MoreThanFive:
                IsSeveralSelected = true;
                break;
        }
    }

    private PropertyCountRange? GetSelectedRange()
    {
        if (IsJustStartingSelected) return PropertyCountRange.One;
        if (IsFewOptionsSelected) return PropertyCountRange.TwoToFive;
        if (IsSeveralSelected) return PropertyCountRange.MoreThanFive;
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
        state.PropertyCount = GetSelectedRange();
        state.CurrentStep = 3;
        _stateService.SaveState(state);

        await Shell.Current.GoToAsync("Household");
    }
}
