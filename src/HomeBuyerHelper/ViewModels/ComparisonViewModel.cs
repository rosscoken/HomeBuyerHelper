using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using System.Collections.ObjectModel;

namespace HomeBuyerHelper.ViewModels;

/// <summary>
/// View model for property comparison page.
/// </summary>
public partial class ComparisonViewModel : BaseViewModel
{
    private readonly IPropertyService _propertyService;
    private readonly IExportService _exportService;
    private readonly ICriteriaRepository _criteriaRepository;
    private readonly IScoreRepository _scoreRepository;

    [ObservableProperty]
    private IReadOnlyList<PropertyRanking> _comparisonResults = [];

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isSelectingProperties;

    [ObservableProperty]
    private ObservableCollection<SelectableProperty> _selectableProperties = new();

    [ObservableProperty]
    private IReadOnlyList<Property> _selectedProperties = [];

    [ObservableProperty]
    private IReadOnlyList<EvaluationCriterion> _criteria = [];

    [ObservableProperty]
    private IReadOnlyList<ComparisonRow> _comparisonRows = [];

    [ObservableProperty]
    private bool _showMatrix;

    public int SelectedCount => SelectableProperties.Count(p => p.IsSelected);

    public ComparisonViewModel(
        IPropertyService propertyService,
        IExportService exportService,
        ICriteriaRepository criteriaRepository,
        IScoreRepository scoreRepository)
    {
        _propertyService = propertyService;
        _exportService = exportService;
        _criteriaRepository = criteriaRepository;
        _scoreRepository = scoreRepository;
        Title = "Compare Properties";
    }

    public override async Task OnAppearingAsync()
    {
        await LoadComparisonAsync();
    }

    [RelayCommand]
    private async Task LoadComparisonAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            ComparisonResults = await _propertyService.GetPropertyRankingsAsync();
            IsEmpty = ComparisonResults.Count == 0;

            // Load properties for selection
            var properties = await _propertyService.GetPropertiesWithScoresAsync();
            SelectableProperties = new ObservableCollection<SelectableProperty>(
                properties.Select(p => new SelectableProperty
                {
                    Property = p,
                    IsSelected = false
                }));

            // Auto-select top 3 if available
            foreach (var prop in SelectableProperties.Take(Math.Min(3, SelectableProperties.Count)))
            {
                prop.IsSelected = true;
            }

            OnPropertyChanged(nameof(SelectedCount));
            await GenerateComparisonMatrixAsync();
        });
    }

    [RelayCommand]
    private void TogglePropertySelection(SelectableProperty item)
    {
        if (item.IsSelected)
        {
            item.IsSelected = false;
        }
        else if (SelectedCount < 4)
        {
            item.IsSelected = true;
        }
        OnPropertyChanged(nameof(SelectedCount));
    }

    [RelayCommand]
    private async Task GenerateComparisonMatrixAsync()
    {
        var selected = SelectableProperties.Where(p => p.IsSelected).Select(p => p.Property).ToList();
        if (selected.Count < 2)
        {
            ShowMatrix = false;
            return;
        }

        SelectedProperties = selected;
        Criteria = await _criteriaRepository.GetAllAsync();

        var rows = new List<ComparisonRow>();

        foreach (var criterion in Criteria)
        {
            var row = new ComparisonRow
            {
                CriterionName = criterion.Name,
                Weight = criterion.Weight,
                Scores = new List<ComparisonScore>()
            };

            foreach (var property in selected)
            {
                var score = property.Scores.FirstOrDefault(s => s.CriterionId == criterion.Id);
                row.Scores.Add(new ComparisonScore
                {
                    PropertyId = property.Id,
                    Score = score?.Score ?? 0,
                    HasScore = score != null
                });
            }

            // Determine winner for this criterion
            if (row.Scores.Any(s => s.HasScore))
            {
                var maxScore = row.Scores.Where(s => s.HasScore).Max(s => s.Score);
                foreach (var s in row.Scores.Where(s => s.Score == maxScore && s.HasScore))
                {
                    s.IsWinner = true;
                }
            }

            rows.Add(row);
        }

        ComparisonRows = rows;
        ShowMatrix = true;
    }

    [RelayCommand]
    private void ToggleSelectionMode()
    {
        IsSelectingProperties = !IsSelectingProperties;
    }

    [RelayCommand]
    private async Task ExportComparisonAsync()
    {
        if (ComparisonResults.Count == 0)
        {
            SetError("No properties to export.");
            return;
        }

        await ExecuteBusyAsync(async () =>
        {
            var propertyIds = ComparisonResults.Select(r => r.Property.Id);
            var filePath = await _exportService.ExportComparisonToPdfAsync(propertyIds);

            await Shell.Current.DisplayAlert(
                "Export Complete",
                $"Comparison exported to:\n{filePath}",
                "OK");
        });
    }

    [RelayCommand]
    private async Task ShareComparisonAsync()
    {
        if (ComparisonResults.Count == 0)
        {
            SetError("No properties to share.");
            return;
        }

        // Generate share text for top property
        var topProperty = ComparisonResults.FirstOrDefault();
        if (topProperty != null)
        {
            var shareText = await _exportService.GenerateShareTextAsync(topProperty.Property.Id);
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Text = shareText,
                Title = "Share Property Comparison"
            });
        }
    }
}

/// <summary>
/// Property with selection state for comparison.
/// </summary>
public partial class SelectableProperty : ObservableObject
{
    [ObservableProperty]
    private Property _property = null!;

    [ObservableProperty]
    private bool _isSelected;
}

/// <summary>
/// Row in the comparison matrix representing a criterion.
/// </summary>
public class ComparisonRow
{
    public string CriterionName { get; set; } = string.Empty;
    public int Weight { get; set; }
    public List<ComparisonScore> Scores { get; set; } = new();
}

/// <summary>
/// Score cell in the comparison matrix.
/// </summary>
public class ComparisonScore
{
    public int PropertyId { get; set; }
    public int Score { get; set; }
    public bool HasScore { get; set; }
    public bool IsWinner { get; set; }

    public string ScoreDisplay => HasScore ? Score.ToString() : "-";
    public string ScoreColor => IsWinner ? "#4CAF50" :
                                HasScore ? (Score >= 7 ? "#4CAF50" : Score >= 4 ? "#FF9800" : "#F44336") :
                                "#9E9E9E";
}
