using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Data;
using HomeBuyerHelper.Core.Interfaces;

namespace HomeBuyerHelper.ViewModels.Onboarding;

/// <summary>
/// ViewModel for the location priorities screen (P1-ONB-005).
/// </summary>
public partial class LocationPrioritiesViewModel : BaseViewModel
{
    private const int MaxSelections = 3;
    private readonly IOnboardingStateService _stateService;

    [ObservableProperty]
    private ObservableCollection<PriorityOptionItem> _options = new();

    [ObservableProperty]
    private string _selectionHint = "Select up to 3 priorities";

    public bool HasSelection => Options.Any(o => o.IsSelected);

    public LocationPrioritiesViewModel(IOnboardingStateService stateService)
    {
        _stateService = stateService;
        Title = "Location Priorities";
        LoadOptions();
    }

    private void LoadOptions()
    {
        var state = _stateService.GetState();

        var optionItems = CommonCriteria.LocationPriorities.Select(priority => new PriorityOptionItem(
            priority.Key,
            priority.DisplayName,
            state.LocationPriorities.Contains(priority.Key),
            this));

        foreach (var item in optionItems)
        {
            Options.Add(item);
        }

        UpdateHint();
    }

    public void OnOptionToggled(PriorityOptionItem item)
    {
        var selectedCount = Options.Count(o => o.IsSelected);

        if (item.IsSelected && selectedCount > MaxSelections)
        {
            item.IsSelected = false;
            return;
        }

        UpdateHint();
        OnPropertyChanged(nameof(HasSelection));
    }

    private void UpdateHint()
    {
        var selectedCount = Options.Count(o => o.IsSelected);
        var remaining = MaxSelections - selectedCount;

        SelectionHint = remaining switch
        {
            0 => "Maximum selections reached",
            1 => "1 selection remaining",
            _ => $"{remaining} selections remaining"
        };
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
        state.LocationPriorities = Options
            .Where(o => o.IsSelected)
            .Select(o => o.Key)
            .ToList();
        state.CurrentStep = 5;
        _stateService.SaveState(state);

        await Shell.Current.GoToAsync("HomePriorities");
    }
}

/// <summary>
/// A selectable priority option item.
/// </summary>
public partial class PriorityOptionItem : ObservableObject
{
    private readonly LocationPrioritiesViewModel? _locationViewModel;
    private readonly HomePrioritiesViewModel? _homeViewModel;

    public string Key { get; }
    public string DisplayName { get; }

    [ObservableProperty]
    private bool _isSelected;

    public PriorityOptionItem(string key, string displayName, bool isSelected, LocationPrioritiesViewModel viewModel)
    {
        Key = key;
        DisplayName = displayName;
        IsSelected = isSelected;
        _locationViewModel = viewModel;
    }

    public PriorityOptionItem(string key, string displayName, bool isSelected, HomePrioritiesViewModel viewModel)
    {
        Key = key;
        DisplayName = displayName;
        IsSelected = isSelected;
        _homeViewModel = viewModel;
    }

    [RelayCommand]
    private void Toggle()
    {
        IsSelected = !IsSelected;
        _locationViewModel?.OnOptionToggled(this);
        _homeViewModel?.OnOptionToggled(this);
    }
}
