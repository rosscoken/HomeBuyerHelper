using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Data;
using HomeBuyerHelper.Core.Interfaces;

namespace HomeBuyerHelper.ViewModels.Onboarding;

/// <summary>
/// ViewModel for the home priorities screen (P1-ONB-006).
/// </summary>
public partial class HomePrioritiesViewModel : BaseViewModel
{
    private const int MaxSelections = 3;
    private readonly IOnboardingStateService _stateService;

    [ObservableProperty]
    private ObservableCollection<PriorityOptionItem> _options = new();

    [ObservableProperty]
    private string _selectionHint = "Select up to 3 priorities";

    public bool HasSelection => Options.Any(o => o.IsSelected);

    public HomePrioritiesViewModel(IOnboardingStateService stateService)
    {
        _stateService = stateService;
        Title = "Home Priorities";
        LoadOptions();
    }

    private void LoadOptions()
    {
        var state = _stateService.GetState();

        var optionItems = CommonCriteria.HomePriorities.Select(priority => new PriorityOptionItem(
            priority.Key,
            priority.DisplayName,
            state.HomePriorities.Contains(priority.Key),
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
        state.HomePriorities = Options
            .Where(o => o.IsSelected)
            .Select(o => o.Key)
            .ToList();
        state.CurrentStep = 6;
        _stateService.SaveState(state);

        await Shell.Current.GoToAsync("CriteriaReview");
    }
}
