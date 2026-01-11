using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Core.Services;

namespace HomeBuyerHelper.ViewModels;

/// <summary>
/// View model for managing evaluation criteria.
/// </summary>
public partial class CriteriaViewModel : BaseViewModel
{
    private readonly ICriteriaRepository _criteriaRepository;
    private readonly IWeightBalancingService _weightBalancingService;

    [ObservableProperty]
    private IReadOnlyList<EvaluationCriterion> _criteria = [];

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private int _totalWeight;

    [ObservableProperty]
    private readonly Color _totalWeightColor = Colors.Black;

    [ObservableProperty]
    private bool _needsRebalancing;

    public IReadOnlyList<CriterionCategory> Categories { get; } = Enum.GetValues<CriterionCategory>();

    public CriteriaViewModel(
        ICriteriaRepository criteriaRepository,
        IWeightBalancingService weightBalancingService)
    {
        _criteriaRepository = criteriaRepository;
        _weightBalancingService = weightBalancingService;
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
            UpdateWeightSummary();
        });
    }

    private void UpdateWeightSummary()
    {
        TotalWeight = Criteria.Sum(c => c.Weight);
        NeedsRebalancing = TotalWeight != 100 && Criteria.Count > 0;

        if (TotalWeight == 100)
            TotalWeightColor = Color.FromArgb("#4CAF50"); // Success
        else if (TotalWeight > 100)
            TotalWeightColor = Color.FromArgb("#F44336"); // Error
        else
            TotalWeightColor = Color.FromArgb("#FF9800"); // Warning
    }

    [RelayCommand]
    private async Task AddNewAsync()
    {
        await Shell.Current.GoToAsync("CriterionEdit?id=0");
    }

    [RelayCommand]
    private async Task EditCriterionAsync(EvaluationCriterion criterion)
    {
        await Shell.Current.GoToAsync($"CriterionEdit?id={criterion.Id}");
    }

    [RelayCommand]
    private async Task RebalanceAsync()
    {
        if (Criteria.Count == 0) return;

        var criteriaList = Criteria.ToList();
        var result = _weightBalancingService.Rebalance(criteriaList);

        if (!result.Success)
        {
            await Shell.Current.DisplayAlert("Cannot Rebalance", result.Warnings.FirstOrDefault() ?? "Failed to rebalance weights.", "OK");
            return;
        }

        // Save updated weights
        await ExecuteBusyAsync(async () =>
        {
            foreach (var criterion in criteriaList)
            {
                await _criteriaRepository.UpdateAsync(criterion);
            }
            await LoadCriteriaAsync();
        });

        if (result.Warnings.Count > 0)
        {
            await Shell.Current.DisplayAlert("Rebalanced", string.Join("\n", result.Warnings), "OK");
        }
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
