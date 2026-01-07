using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels;

/// <summary>
/// View model for managing evaluation criteria.
/// </summary>
public partial class CriteriaViewModel : BaseViewModel
{
    private readonly ICriteriaRepository _criteriaRepository;

    [ObservableProperty]
    private IReadOnlyList<EvaluationCriterion> _criteria = [];

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private string _newCriterionName = string.Empty;

    [ObservableProperty]
    private CriterionCategory _newCriterionCategory = CriterionCategory.Other;

    [ObservableProperty]
    private int _newCriterionWeight = 5;

    public IReadOnlyList<CriterionCategory> Categories { get; } = Enum.GetValues<CriterionCategory>();

    public CriteriaViewModel(ICriteriaRepository criteriaRepository)
    {
        _criteriaRepository = criteriaRepository;
        Title = "Evaluation Criteria";
    }

    public override async Task OnAppearingAsync()
    {
        await LoadCriteriaAsync();
    }

    [RelayCommand]
    private async Task LoadCriteriaAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            Criteria = await _criteriaRepository.GetAllAsync();
            IsEmpty = Criteria.Count == 0;
        });
    }

    [RelayCommand]
    private async Task AddCriterionAsync()
    {
        if (string.IsNullOrWhiteSpace(NewCriterionName))
        {
            SetError("Please enter a name for the criterion.");
            return;
        }

        await ExecuteBusyAsync(async () =>
        {
            var criterion = new EvaluationCriterion
            {
                Name = NewCriterionName.Trim(),
                Category = NewCriterionCategory,
                Weight = NewCriterionWeight,
                IsSystemSuggested = false
            };

            await _criteriaRepository.CreateAsync(criterion);

            // Reset form
            NewCriterionName = string.Empty;
            NewCriterionCategory = CriterionCategory.Other;
            NewCriterionWeight = 5;

            await LoadCriteriaAsync();
        });
    }

    [RelayCommand]
    private async Task UpdateWeightAsync(EvaluationCriterion criterion)
    {
        await _criteriaRepository.UpdateAsync(criterion);
    }

    [RelayCommand]
    private async Task DeleteCriterionAsync(EvaluationCriterion criterion)
    {
        var confirmed = await Shell.Current.DisplayAlert(
            "Delete Criterion",
            $"Are you sure you want to delete '{criterion.Name}'? All scores for this criterion will also be deleted.",
            "Delete",
            "Cancel");

        if (confirmed)
        {
            await _criteriaRepository.DeleteAsync(criterion.Id);
            await LoadCriteriaAsync();
        }
    }
}
