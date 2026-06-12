using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;

namespace HomeBuyerHelper.ViewModels;

/// <summary>
/// View model for data management (export/import) page.
/// </summary>
public partial class DataManagementViewModel : BaseViewModel
{
    private readonly IExportService _exportService;
    private readonly IPropertyService _propertyService;

    [ObservableProperty]
    private string? _lastExportPath;

    [ObservableProperty]
    private ImportValidationResult? _importValidation;

    [ObservableProperty]
    private string? _selectedFilePath;

    private string? _selectedFileContent;

    [ObservableProperty]
    private bool _replaceExisting;

    [ObservableProperty]
    private bool _isFileSelected;

    [ObservableProperty]
    private bool _shareIncludePrices = true;

    [ObservableProperty]
    private bool _shareIncludeScores = true;

    [ObservableProperty]
    private bool _shareIncludeNotes = true;

    [ObservableProperty]
    private int _themeIndex;

    private bool _themeLoaded;

    public IReadOnlyList<string> ThemeNames { get; } = new[] { "System Default", "Light", "Dark" };

    public DataManagementViewModel(
        IExportService exportService,
        IPropertyService propertyService,
        IUserPreferencesRepository preferencesRepository)
    {
        _exportService = exportService;
        _propertyService = propertyService;
        _preferencesRepository = preferencesRepository;
        Title = "Data Management";
    }

    private readonly IUserPreferencesRepository _preferencesRepository;

    public override async Task OnAppearingAsync()
    {
        var preferences = await _preferencesRepository.GetAsync();
        ThemeIndex = Math.Clamp(preferences.ThemePreference, 0, 2);
        _themeLoaded = true;
    }

    partial void OnThemeIndexChanged(int value)
    {
        if (!_themeLoaded) return;
        _ = SaveThemeAsync(value);
    }

    private async Task SaveThemeAsync(int value)
    {
        App.ApplyTheme(value);
        var preferences = await _preferencesRepository.GetAsync();
        preferences.ThemePreference = value;
        await _preferencesRepository.SaveAsync(preferences);
    }

    /// <summary>
    /// Shareable read-only HTML report with privacy options (P4-SHR-001/002/003).
    /// Financial projections and income are never included.
    /// </summary>
    [RelayCommand]
    private async Task ShareReadOnlyReportAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var filePath = await _exportService.ExportShareableHtmlAsync(new ShareReportOptions
            {
                IncludePrices = ShareIncludePrices,
                IncludeScores = ShareIncludeScores,
                IncludeNotes = ShareIncludeNotes
            });

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Share Property Report",
                File = new ShareFile(filePath, "text/html")
            });
        });
    }

    /// <summary>
    /// Comparison matrix CSV export (P4-EXP-001).
    /// </summary>
    [RelayCommand]
    private async Task ExportComparisonCsvAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var rankings = await _propertyService.GetPropertyRankingsAsync();
            if (rankings.Count == 0)
            {
                SetError("Add properties before exporting the comparison.");
                return;
            }

            var filePath = await _exportService.ExportComparisonToCsvAsync(
                rankings.Select(r => r.Property.Id));
            await ShareCsvAsync(filePath, "Share Comparison CSV");
        });
    }

    /// <summary>
    /// Cash flow projection CSV export (P4-EXP-001).
    /// </summary>
    [RelayCommand]
    private async Task ExportCashFlowCsvAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var filePath = await _exportService.ExportCashFlowToCsvAsync();
            await ShareCsvAsync(filePath, "Share Cash Flow CSV");
        });
    }

    private static async Task ShareCsvAsync(string filePath, string title)
    {
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = title,
            File = new ShareFile(filePath, "text/csv")
        });
    }

    [RelayCommand]
    private async Task ExportDataAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var filePath = await _exportService.ExportAllDataAsync();
            LastExportPath = filePath;

            var result = await Shell.Current.DisplayAlert(
                "Export Complete",
                $"Data exported to:\n{filePath}\n\nWould you like to share this file?",
                "Share",
                "OK");

            if (result)
            {
                await ShareExportFileAsync(filePath);
            }
        });
    }

    private static async Task ShareExportFileAsync(string filePath)
    {
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Share HomeBuyerHelper Backup",
            File = new ShareFile(filePath, "application/json")
        });
    }

    [RelayCommand]
    private async Task SelectImportFileAsync()
    {
        try
        {
            var fileResult = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select HomeBuyerHelper Backup",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.json" } },
                    { DevicePlatform.Android, new[] { "application/json" } },
                    { DevicePlatform.WinUI, new[] { ".json" } },
                    { DevicePlatform.macOS, new[] { "json" } }
                })
            });

            if (fileResult != null)
            {
                // Read through the stream once and cache: FullPath is not
                // reliably readable on Android/iOS, and the picked file may
                // not be re-readable later.
                SelectedFilePath = fileResult.FileName;
                _selectedFileContent = await Services.FileResultExtensions.ReadAllTextAsync(fileResult);
                ImportValidation = await _exportService.ValidateImportFileAsync(_selectedFileContent);
                IsFileSelected = true;
            }
        }
        catch (Exception ex)
        {
            SetError($"Error selecting file: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ImportDataAsync()
    {
        if (string.IsNullOrEmpty(_selectedFileContent) || ImportValidation == null || !ImportValidation.IsValid)
        {
            SetError("Please select a valid backup file first.");
            return;
        }

        var actionText = ReplaceExisting ? "replace all existing data with" : "merge with";
        var confirmed = await Shell.Current.DisplayAlert(
            "Confirm Import",
            $"This will {actionText} the data from the backup file.\n\n" +
            $"Properties: {ImportValidation.PropertyCount}\n" +
            $"Criteria: {ImportValidation.CriteriaCount}\n" +
            $"Scores: {ImportValidation.ScoreCount}\n\n" +
            (ReplaceExisting ? "WARNING: This will delete all existing data!" : "New data will be added to existing data."),
            "Import",
            "Cancel");

        if (!confirmed)
            return;

        await ExecuteBusyAsync(async () =>
        {
            var success = await _exportService.ImportFromJsonAsync(_selectedFileContent!, ReplaceExisting);

            if (success)
            {
                await Shell.Current.DisplayAlert(
                    "Import Complete",
                    "Data imported successfully. Please restart the app to see changes.",
                    "OK");

                // Clear import state
                SelectedFilePath = null;
                _selectedFileContent = null;
                ImportValidation = null;
                IsFileSelected = false;
                ReplaceExisting = false;
            }
            else
            {
                SetError("Import failed. Please check the file format and try again.");
            }
        });
    }

    [RelayCommand]
    private void ClearSelection()
    {
        SelectedFilePath = null;
        _selectedFileContent = null;
        ImportValidation = null;
        IsFileSelected = false;
        ReplaceExisting = false;
        ClearError();
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
