using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Data;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Onboarding;

/// <summary>
/// ViewModel for the criteria review screen (P1-ONB-008).
/// </summary>
public partial class CriteriaReviewViewModel : BaseViewModel
{
    private readonly IOnboardingStateService _stateService;

    [ObservableProperty]
    private readonly ObservableCollection<CriterionItem> _criteria = new();

    [ObservableProperty]
    private decimal _totalWeight;

    [ObservableProperty]
    private readonly Color _totalWeightColor = Colors.Green;

    public bool CanContinue => Criteria.Any(c => c.IsIncluded) &&
                                Math.Abs(TotalWeight - 100) < 1;

    public CriteriaReviewViewModel(IOnboardingStateService stateService)
    {
        _stateService = stateService;
        Title = "Review Criteria";
        LoadSuggestedCriteria();
    }

    private void LoadSuggestedCriteria()
    {
        var state = _stateService.GetState();

        var suggestions = CommonCriteria.GetSuggestedCriteria(
            state.BuyingSituation,
            state.HouseholdType,
            state.WorkArrangement,
            state.Pets,
            state.LocationPriorities,
            state.HomePriorities);

        var criteriaItems = suggestions.Select(suggestion => new CriterionItem(
            suggestion.Name,
            suggestion.SuggestedWeight,
            suggestion.SuggestionReason ?? "Based on your preferences",
            this));

        foreach (var item in criteriaItems)
        {
            Criteria.Add(item);
        }

        UpdateTotalWeight();
    }

    public void OnCriterionChanged()
    {
        UpdateTotalWeight();
        OnPropertyChanged(nameof(CanContinue));
    }

    private void UpdateTotalWeight()
    {
        TotalWeight = Criteria.Where(c => c.IsIncluded).Sum(c => c.Weight);
        TotalWeightColor = Math.Abs(TotalWeight - 100) < 1 ? Colors.Green :
                          TotalWeight > 100 ? Colors.Red : Colors.Orange;
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
        state.SelectedCriteria = Criteria
            .Where(c => c.IsIncluded)
            .Select(c => new CriterionSelection
            {
                Name = c.Name,
                Weight = c.Weight,
                SuggestionReason = c.Reason
            })
            .ToList();
        state.CurrentStep = 7;
        _stateService.SaveState(state);

        await Shell.Current.GoToAsync("OnboardingComplete");
    }
}

/// <summary>
/// A criterion item for the review screen.
/// </summary>
public partial class CriterionItem : ObservableObject
{
    private readonly CriteriaReviewViewModel _viewModel;

    public string Name { get; }
    public string Reason { get; }

    [ObservableProperty]
    private readonly bool _isIncluded = true;

    [ObservableProperty]
    private decimal _weight;

    public CriterionItem(string name, decimal weight, string reason, CriteriaReviewViewModel viewModel)
    {
        Name = name;
        Weight = weight;
        Reason = reason;
        _viewModel = viewModel;
    }

    partial void OnIsIncludedChanged(bool value)
    {
        _viewModel.OnCriterionChanged();
    }

    partial void OnWeightChanged(decimal value)
    {
        _viewModel.OnCriterionChanged();
    }
}
