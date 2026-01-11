using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels;

/// <summary>
/// ViewModel for editing a criterion.
/// </summary>
[QueryProperty(nameof(CriterionId), "id")]
public partial class CriterionEditViewModel : BaseViewModel
{
    private readonly ICriteriaRepository _criteriaRepository;

    [ObservableProperty]
    private int _criterionId;

    [ObservableProperty]
    private readonly string _name = string.Empty;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private readonly int _weight = 5;

    [ObservableProperty]
    private readonly CriterionCategory _category = CriterionCategory.Other;

    [ObservableProperty]
    private string? _scoreAnchorLow;

    [ObservableProperty]
    private string? _scoreAnchorMidLow;

    [ObservableProperty]
    private string? _scoreAnchorMid;

    [ObservableProperty]
    private string? _scoreAnchorMidHigh;

    [ObservableProperty]
    private string? _scoreAnchorHigh;

    [ObservableProperty]
    private bool _isWeightLocked;

    [ObservableProperty]
    private bool _isNewCriterion;

    public IReadOnlyList<CriterionCategory> Categories { get; } = Enum.GetValues<CriterionCategory>();

    public CriterionEditViewModel(ICriteriaRepository criteriaRepository)
    {
        _criteriaRepository = criteriaRepository;
        Title = "Edit Criterion";
    }

    partial void OnCriterionIdChanged(int value)
    {
        if (value > 0)
        {
            _ = LoadCriterionAsync();
        }
        else
        {
            IsNewCriterion = true;
            Title = "Add Criterion";
        }
    }

    private async Task LoadCriterionAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var criterion = await _criteriaRepository.GetByIdAsync(CriterionId);
            if (criterion != null)
            {
                Name = criterion.Name;
                Description = criterion.Description;
                Weight = criterion.Weight;
                Category = criterion.Category;
                ScoreAnchorLow = criterion.ScoreAnchorLow;
                ScoreAnchorMidLow = criterion.ScoreAnchorMidLow;
                ScoreAnchorMid = criterion.ScoreAnchorMid;
                ScoreAnchorMidHigh = criterion.ScoreAnchorMidHigh;
                ScoreAnchorHigh = criterion.ScoreAnchorHigh;
                IsWeightLocked = criterion.IsWeightLocked;
                Title = $"Edit: {criterion.Name}";
            }
        });
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            SetError("Criterion name is required.");
            return;
        }

        if (Weight < 1 || Weight > 50)
        {
            SetError("Weight must be between 1 and 50%.");
            return;
        }

        await ExecuteBusyAsync(async () =>
        {
            if (IsNewCriterion)
            {
                var criterion = new EvaluationCriterion
                {
                    Name = Name.Trim(),
                    Description = Description?.Trim(),
                    Weight = Weight,
                    Category = Category,
                    ScoreAnchorLow = ScoreAnchorLow?.Trim(),
                    ScoreAnchorMidLow = ScoreAnchorMidLow?.Trim(),
                    ScoreAnchorMid = ScoreAnchorMid?.Trim(),
                    ScoreAnchorMidHigh = ScoreAnchorMidHigh?.Trim(),
                    ScoreAnchorHigh = ScoreAnchorHigh?.Trim(),
                    IsWeightLocked = IsWeightLocked,
                    IsSystemSuggested = false
                };
                await _criteriaRepository.CreateAsync(criterion);
            }
            else
            {
                var criterion = await _criteriaRepository.GetByIdAsync(CriterionId);
                if (criterion != null)
                {
                    criterion.Name = Name.Trim();
                    criterion.Description = Description?.Trim();
                    criterion.Weight = Weight;
                    criterion.Category = Category;
                    criterion.ScoreAnchorLow = ScoreAnchorLow?.Trim();
                    criterion.ScoreAnchorMidLow = ScoreAnchorMidLow?.Trim();
                    criterion.ScoreAnchorMid = ScoreAnchorMid?.Trim();
                    criterion.ScoreAnchorMidHigh = ScoreAnchorMidHigh?.Trim();
                    criterion.ScoreAnchorHigh = ScoreAnchorHigh?.Trim();
                    criterion.IsWeightLocked = IsWeightLocked;
                    criterion.UpdatedAt = DateTime.UtcNow;
                    await _criteriaRepository.UpdateAsync(criterion);
                }
            }

            await Shell.Current.GoToAsync("..");
        });
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (IsNewCriterion) return;

        var confirmed = await Shell.Current.DisplayAlert(
            "Delete Criterion",
            $"Are you sure you want to delete '{Name}'? All scores for this criterion will also be deleted.",
            "Delete",
            "Cancel");

        if (confirmed)
        {
            await _criteriaRepository.DeleteAsync(CriterionId);
            await Shell.Current.GoToAsync("..");
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
