using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels;

/// <summary>
/// ViewModel for scoring a property through all criteria.
/// </summary>
[QueryProperty(nameof(PropertyId), "propertyId")]
public partial class ScoringWalkthroughViewModel : BaseViewModel
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ICriteriaRepository _criteriaRepository;
    private readonly IScoreRepository _scoreRepository;

    private Property? _property;
    private List<EvaluationCriterion> _allCriteria = new();
    private Dictionary<int, PropertyScore> _existingScores = new();

    [ObservableProperty]
    private int _propertyId;

    [ObservableProperty]
    private string _propertyName = string.Empty;

    [ObservableProperty]
    private int _currentIndex;

    [ObservableProperty]
    private int _totalCriteria;

    [ObservableProperty]
    private EvaluationCriterion? _currentCriterion;

    [ObservableProperty]
    private int _selectedScore;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private bool _canGoBack;

    [ObservableProperty]
    private bool _canGoForward;

    [ObservableProperty]
    private bool _isLastCriterion;

    [ObservableProperty]
    private bool _useSliderInput;

    public string ProgressText => $"{CurrentIndex + 1} of {TotalCriteria}";
    public double ProgressPercent => TotalCriteria > 0 ? (double)(CurrentIndex + 1) / TotalCriteria : 0;

    public ScoringWalkthroughViewModel(
        IPropertyRepository propertyRepository,
        ICriteriaRepository criteriaRepository,
        IScoreRepository scoreRepository)
    {
        _propertyRepository = propertyRepository;
        _criteriaRepository = criteriaRepository;
        _scoreRepository = scoreRepository;
        Title = "Score Property";
    }

    partial void OnPropertyIdChanged(int value)
    {
        if (value > 0)
        {
            _ = InitializeAsync();
        }
    }

    partial void OnCurrentIndexChanged(int value)
    {
        LoadCurrentCriterion();
        CanGoBack = value > 0;
        IsLastCriterion = value >= TotalCriteria - 1;
        OnPropertyChanged(nameof(ProgressText));
        OnPropertyChanged(nameof(ProgressPercent));
    }

    private async Task InitializeAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            _property = await _propertyRepository.GetByIdAsync(PropertyId);
            if (_property == null)
            {
                SetError("Property not found.");
                return;
            }

            PropertyName = _property.Nickname;
            Title = $"Score: {PropertyName}";

            _allCriteria = (await _criteriaRepository.GetAllAsync()).ToList();
            TotalCriteria = _allCriteria.Count;

            if (TotalCriteria == 0)
            {
                SetError("No criteria defined. Please add criteria first.");
                return;
            }

            // Load existing scores
            var scores = await _scoreRepository.GetScoresForPropertyAsync(PropertyId);
            _existingScores = scores.ToDictionary(s => s.CriterionId);

            CurrentIndex = 0;
            LoadCurrentCriterion();
        });
    }

    private void LoadCurrentCriterion()
    {
        if (_allCriteria.Count == 0 || CurrentIndex >= _allCriteria.Count)
            return;

        CurrentCriterion = _allCriteria[CurrentIndex];

        // Load existing score if available
        if (_existingScores.TryGetValue(CurrentCriterion.Id, out var existingScore))
        {
            SelectedScore = existingScore.Score;
            Notes = existingScore.Notes;
        }
        else
        {
            SelectedScore = 0;
            Notes = null;
        }
    }

    [RelayCommand]
    private void SelectScore(int score)
    {
        SelectedScore = score;
    }

    [RelayCommand]
    private async Task SaveAndNextAsync()
    {
        if (CurrentCriterion == null) return;

        // Save current score
        await SaveCurrentScoreAsync();

        // Move to next or finish
        if (IsLastCriterion)
        {
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            CurrentIndex++;
        }
    }

    [RelayCommand]
    private async Task SkipAsync()
    {
        if (IsLastCriterion)
        {
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            CurrentIndex++;
        }
    }

    [RelayCommand]
    private void GoBack()
    {
        if (CurrentIndex > 0)
        {
            CurrentIndex--;
        }
    }

    [RelayCommand]
    private async Task FinishAsync()
    {
        // Save current score if selected
        if (SelectedScore > 0)
        {
            await SaveCurrentScoreAsync();
        }
        await Shell.Current.GoToAsync("..");
    }

    private async Task SaveCurrentScoreAsync()
    {
        if (CurrentCriterion == null || SelectedScore <= 0) return;

        var score = new PropertyScore
        {
            PropertyId = PropertyId,
            CriterionId = CurrentCriterion.Id,
            Score = SelectedScore,
            Notes = Notes?.Trim()
        };

        if (_existingScores.TryGetValue(CurrentCriterion.Id, out var existingScore))
        {
            score.Id = existingScore.Id;
            await _scoreRepository.UpdateAsync(score);
            _existingScores[CurrentCriterion.Id] = score;
        }
        else
        {
            var savedScore = await _scoreRepository.CreateAsync(score);
            _existingScores[CurrentCriterion.Id] = savedScore;
        }
    }

    [RelayCommand]
    private void ToggleInputMode()
    {
        UseSliderInput = !UseSliderInput;
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
