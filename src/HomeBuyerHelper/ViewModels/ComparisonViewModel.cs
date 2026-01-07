using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;

namespace HomeBuyerHelper.ViewModels;

/// <summary>
/// View model for property comparison page.
/// </summary>
public partial class ComparisonViewModel : BaseViewModel
{
    private readonly IPropertyService _propertyService;
    private readonly IExportService _exportService;

    [ObservableProperty]
    private IReadOnlyList<PropertyRanking> _comparisonResults = [];

    [ObservableProperty]
    private bool _isEmpty;

    public ComparisonViewModel(
        IPropertyService propertyService,
        IExportService exportService)
    {
        _propertyService = propertyService;
        _exportService = exportService;
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
        });
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
