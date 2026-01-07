using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels;

/// <summary>
/// View model for the onboarding flow.
/// </summary>
public partial class OnboardingViewModel : BaseViewModel
{
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly ICriteriaRepository _criteriaRepository;

    [ObservableProperty]
    private int _currentStep;

    [ObservableProperty]
    private int _totalSteps = 6;

    [ObservableProperty]
    private BuyingGoal _buyingGoal = BuyingGoal.Exploring;

    [ObservableProperty]
    private PropertyCountRange _propertyCountRange = PropertyCountRange.TwoToFive;

    [ObservableProperty]
    private int _householdSize = 2;

    [ObservableProperty]
    private bool _hasChildren;

    [ObservableProperty]
    private bool _hasPets;

    [ObservableProperty]
    private WorkArrangement _workArrangement = WorkArrangement.Hybrid;

    [ObservableProperty]
    private bool _prioritizesLocation;

    [ObservableProperty]
    private bool _prioritizesSize;

    [ObservableProperty]
    private bool _prioritizesCondition;

    [ObservableProperty]
    private bool _prioritizesPrice;

    public IReadOnlyList<BuyingGoal> BuyingGoals { get; } = Enum.GetValues<BuyingGoal>();
    public IReadOnlyList<PropertyCountRange> PropertyCountRanges { get; } = Enum.GetValues<PropertyCountRange>();
    public IReadOnlyList<WorkArrangement> WorkArrangements { get; } = Enum.GetValues<WorkArrangement>();

    public OnboardingViewModel(
        IUserPreferencesRepository preferencesRepository,
        ICriteriaRepository criteriaRepository)
    {
        _preferencesRepository = preferencesRepository;
        _criteriaRepository = criteriaRepository;
        Title = "Welcome";
        CurrentStep = 1;
    }

    [RelayCommand]
    private void NextStep()
    {
        if (CurrentStep < TotalSteps)
        {
            CurrentStep++;
            UpdateTitle();
        }
    }

    [RelayCommand]
    private void PreviousStep()
    {
        if (CurrentStep > 1)
        {
            CurrentStep--;
            UpdateTitle();
        }
    }

    [RelayCommand]
    private async Task CompleteOnboardingAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            // Save preferences
            var preferences = await _preferencesRepository.GetAsync();
            preferences.BuyingGoal = BuyingGoal;
            preferences.PropertyCountRange = PropertyCountRange;
            preferences.HouseholdSize = HouseholdSize;
            preferences.HasChildren = HasChildren;
            preferences.HasPets = HasPets;
            preferences.WorkArrangement = WorkArrangement;
            preferences.PrioritizesLocation = PrioritizesLocation;
            preferences.PrioritizesSize = PrioritizesSize;
            preferences.PrioritizesCondition = PrioritizesCondition;
            preferences.PrioritizesPrice = PrioritizesPrice;
            preferences.HasCompletedOnboarding = true;
            await _preferencesRepository.SaveAsync(preferences);

            // Create default criteria based on preferences
            await CreateDefaultCriteriaAsync();

            // Navigate to dashboard
            await Shell.Current.GoToAsync("//Dashboard");
        });
    }

    [RelayCommand]
    private async Task SkipOnboardingAsync()
    {
        await _preferencesRepository.SetOnboardingCompleteAsync();
        await Shell.Current.GoToAsync("//Dashboard");
    }

    private void UpdateTitle()
    {
        Title = CurrentStep switch
        {
            1 => "Welcome",
            2 => "Your Goal",
            3 => "Properties",
            4 => "Household",
            5 => "Work",
            6 => "Priorities",
            _ => "Welcome"
        };
    }

    private async Task CreateDefaultCriteriaAsync()
    {
        var existingCount = await _criteriaRepository.GetCountAsync();
        if (existingCount > 0) return;

        var criteria = new List<EvaluationCriterion>();

        // Location criteria
        if (PrioritizesLocation)
        {
            criteria.Add(new EvaluationCriterion { Name = "Neighborhood Safety", Category = CriterionCategory.Location, Weight = 8, IsSystemSuggested = true });
            criteria.Add(new EvaluationCriterion { Name = "Schools Quality", Category = CriterionCategory.Location, Weight = HasChildren ? 9 : 5, IsSystemSuggested = true });
            criteria.Add(new EvaluationCriterion { Name = "Walkability", Category = CriterionCategory.Location, Weight = 6, IsSystemSuggested = true });
        }

        // Size criteria
        if (PrioritizesSize)
        {
            criteria.Add(new EvaluationCriterion { Name = "Living Space", Category = CriterionCategory.Interior, Weight = 8, IsSystemSuggested = true });
            criteria.Add(new EvaluationCriterion { Name = "Bedroom Count", Category = CriterionCategory.Interior, Weight = 7, IsSystemSuggested = true });
            criteria.Add(new EvaluationCriterion { Name = "Storage Space", Category = CriterionCategory.Interior, Weight = 6, IsSystemSuggested = true });
        }

        // Condition criteria
        if (PrioritizesCondition)
        {
            criteria.Add(new EvaluationCriterion { Name = "Kitchen Condition", Category = CriterionCategory.Interior, Weight = 8, IsSystemSuggested = true });
            criteria.Add(new EvaluationCriterion { Name = "Bathroom Condition", Category = CriterionCategory.Interior, Weight = 7, IsSystemSuggested = true });
            criteria.Add(new EvaluationCriterion { Name = "Overall Maintenance", Category = CriterionCategory.Interior, Weight = 7, IsSystemSuggested = true });
        }

        // Work arrangement criteria
        if (WorkArrangement is WorkArrangement.Hybrid or WorkArrangement.FullyOnsite)
        {
            criteria.Add(new EvaluationCriterion { Name = "Commute Time", Category = CriterionCategory.Location, Weight = 8, IsSystemSuggested = true });
        }
        else if (WorkArrangement == WorkArrangement.FullyRemote)
        {
            criteria.Add(new EvaluationCriterion { Name = "Home Office Space", Category = CriterionCategory.Interior, Weight = 8, IsSystemSuggested = true });
        }

        // Pet criteria
        if (HasPets)
        {
            criteria.Add(new EvaluationCriterion { Name = "Yard Size", Category = CriterionCategory.Exterior, Weight = 7, IsSystemSuggested = true });
            criteria.Add(new EvaluationCriterion { Name = "Pet-Friendly Features", Category = CriterionCategory.Exterior, Weight = 6, IsSystemSuggested = true });
        }

        // Always add some basics
        criteria.Add(new EvaluationCriterion { Name = "Natural Light", Category = CriterionCategory.Interior, Weight = 6, IsSystemSuggested = true });
        criteria.Add(new EvaluationCriterion { Name = "Curb Appeal", Category = CriterionCategory.Exterior, Weight = 5, IsSystemSuggested = true });

        if (criteria.Count > 0)
        {
            await _criteriaRepository.CreateManyAsync(criteria);
        }
    }
}
