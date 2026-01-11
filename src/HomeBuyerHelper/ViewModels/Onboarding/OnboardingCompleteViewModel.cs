using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Onboarding;

/// <summary>
/// ViewModel for the onboarding complete screen (P1-ONB-012).
/// </summary>
public partial class OnboardingCompleteViewModel : BaseViewModel
{
    private readonly IOnboardingStateService _stateService;
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly ICriteriaRepository _criteriaRepository;

    [ObservableProperty]
    private int _criteriaCount;

    [ObservableProperty]
    private string _topPriority = string.Empty;

    [ObservableProperty]
    private string _situation = string.Empty;

    public OnboardingCompleteViewModel(
        IOnboardingStateService stateService,
        IUserPreferencesRepository preferencesRepository,
        ICriteriaRepository criteriaRepository)
    {
        _stateService = stateService;
        _preferencesRepository = preferencesRepository;
        _criteriaRepository = criteriaRepository;
        Title = "Setup Complete";
        LoadSummary();
    }

    private void LoadSummary()
    {
        var state = _stateService.GetState();

        CriteriaCount = state.SelectedCriteria.Count;

        TopPriority = state.SelectedCriteria
            .OrderByDescending(c => c.Weight)
            .FirstOrDefault()?.Name ?? "None";

        Situation = state.BuyingSituation switch
        {
            BuyingSituation.FirstHome => "First-time buyer",
            BuyingSituation.Upgrading => "Upgrading",
            BuyingSituation.Downsizing => "Downsizing",
            BuyingSituation.Relocating => "Relocating",
            BuyingSituation.InvestmentProperty => "Investment",
            _ => "Not specified"
        };
    }

    [RelayCommand]
    private async Task AddPropertyAsync()
    {
        await SaveAndCompleteOnboardingAsync();
        await Shell.Current.GoToAsync("//Dashboard/PropertyEntry");
    }

    [RelayCommand]
    private async Task GoToDashboardAsync()
    {
        await SaveAndCompleteOnboardingAsync();
        await Shell.Current.GoToAsync("//Dashboard");
    }

    private async Task SaveAndCompleteOnboardingAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var state = _stateService.GetState();

            // Save criteria to database
            var criteria = state.SelectedCriteria.Select((c, index) => new EvaluationCriterion
            {
                Name = c.Name,
                Weight = (int)Math.Round(c.Weight),
                Category = c.Category,
                IsSystemSuggested = true,
                DisplayOrder = index
            }).ToList();

            if (criteria.Count > 0)
            {
                await _criteriaRepository.CreateManyAsync(criteria);
            }

            // Update user preferences
            var preferences = await _preferencesRepository.GetAsync();
            preferences.HasCompletedOnboarding = true;
            preferences.BuyingGoal = state.BuyingSituation switch
            {
                BuyingSituation.FirstHome => BuyingGoal.Exploring,
                BuyingSituation.Upgrading => BuyingGoal.ActivelySearching,
                BuyingSituation.Downsizing => BuyingGoal.ActivelySearching,
                BuyingSituation.Relocating => BuyingGoal.ActivelySearching,
                BuyingSituation.InvestmentProperty => BuyingGoal.ActivelySearching,
                _ => BuyingGoal.Exploring
            };
            preferences.WorkArrangement = state.WorkArrangement ?? WorkArrangement.Hybrid;
            preferences.HasPets = state.Pets != PetType.None;
            preferences.HasChildren = state.HouseholdType == HouseholdType.FamilyWithKids;

            await _preferencesRepository.SaveAsync(preferences);

            // Clear onboarding state
            _stateService.ClearState();
        });
    }
}
