using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Onboarding;

/// <summary>
/// ViewModel for the household composition screen (P1-ONB-004).
/// </summary>
public partial class HouseholdViewModel : BaseViewModel
{
    private readonly IOnboardingStateService _stateService;

    // Household type
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private bool _isJustMeSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private bool _isWithPartnerSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private bool _isFamilySelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private bool _isRoommatesSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private bool _isMultiGenSelected;

    // Work arrangement
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private bool _isFullyOnsiteSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private bool _isHybridSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private bool _isFullyRemoteSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private bool _isRetiredSelected;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private bool _isMixedSelected;

    // Pets
    [ObservableProperty]
    private bool _hasDogs;

    [ObservableProperty]
    private bool _hasCats;

    [ObservableProperty]
    private bool _hasOtherPets;

    public bool HasSelection =>
        HasHouseholdSelection && HasWorkSelection;

    private bool HasHouseholdSelection =>
        IsJustMeSelected || IsWithPartnerSelected || IsFamilySelected ||
        IsRoommatesSelected || IsMultiGenSelected;

    private bool HasWorkSelection =>
        IsFullyOnsiteSelected || IsHybridSelected || IsFullyRemoteSelected ||
        IsRetiredSelected || IsMixedSelected;

    public HouseholdViewModel(IOnboardingStateService stateService)
    {
        _stateService = stateService;
        Title = "Household";
        LoadState();
    }

    private void LoadState()
    {
        var state = _stateService.GetState();

        // Household type
        switch (state.HouseholdType)
        {
            case HouseholdType.JustMe:
                IsJustMeSelected = true;
                break;
            case HouseholdType.WithPartner:
                IsWithPartnerSelected = true;
                break;
            case HouseholdType.FamilyWithKids:
                IsFamilySelected = true;
                break;
            case HouseholdType.Roommates:
                IsRoommatesSelected = true;
                break;
            case HouseholdType.MultiGenerational:
                IsMultiGenSelected = true;
                break;
        }

        // Work arrangement
        switch (state.WorkArrangement)
        {
            case WorkArrangement.FullyOnsite:
                IsFullyOnsiteSelected = true;
                break;
            case WorkArrangement.Hybrid:
                IsHybridSelected = true;
                break;
            case WorkArrangement.FullyRemote:
                IsFullyRemoteSelected = true;
                break;
            case WorkArrangement.Retired:
                IsRetiredSelected = true;
                break;
            case WorkArrangement.Other:
                IsMixedSelected = true;
                break;
        }

        // Pets
        HasDogs = (state.Pets & PetType.Dogs) != 0;
        HasCats = (state.Pets & PetType.Cats) != 0;
        HasOtherPets = (state.Pets & PetType.Other) != 0;
    }

    private HouseholdType? GetSelectedHouseholdType()
    {
        if (IsJustMeSelected) return HouseholdType.JustMe;
        if (IsWithPartnerSelected) return HouseholdType.WithPartner;
        if (IsFamilySelected) return HouseholdType.FamilyWithKids;
        if (IsRoommatesSelected) return HouseholdType.Roommates;
        if (IsMultiGenSelected) return HouseholdType.MultiGenerational;
        return null;
    }

    private WorkArrangement? GetSelectedWorkArrangement()
    {
        if (IsFullyOnsiteSelected) return WorkArrangement.FullyOnsite;
        if (IsHybridSelected) return WorkArrangement.Hybrid;
        if (IsFullyRemoteSelected) return WorkArrangement.FullyRemote;
        if (IsRetiredSelected) return WorkArrangement.Retired;
        if (IsMixedSelected) return WorkArrangement.Other;
        return null;
    }

    private PetType GetSelectedPets()
    {
        var pets = PetType.None;
        if (HasDogs) pets |= PetType.Dogs;
        if (HasCats) pets |= PetType.Cats;
        if (HasOtherPets) pets |= PetType.Other;
        return pets;
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
        state.HouseholdType = GetSelectedHouseholdType();
        state.WorkArrangement = GetSelectedWorkArrangement();
        state.Pets = GetSelectedPets();
        state.CurrentStep = 4;
        _stateService.SaveState(state);

        await Shell.Current.GoToAsync("LocationPriorities");
    }
}
