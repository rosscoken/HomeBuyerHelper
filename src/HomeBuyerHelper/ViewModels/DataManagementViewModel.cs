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

    [ObservableProperty]
    private string? _lastExportPath;

    [ObservableProperty]
    private ImportValidationResult? _importValidation;

    [ObservableProperty]
    private string? _selectedFilePath;

    [ObservableProperty]
    private bool _replaceExisting;

    [ObservableProperty]
    private bool _isFileSelected;

    public DataManagementViewModel(IExportService exportService)
    {
        _exportService = exportService;
        Title = "Data Management";
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
                SelectedFilePath = fileResult.FullPath;
                var content = await File.ReadAllTextAsync(fileResult.FullPath);
                ImportValidation = await _exportService.ValidateImportFileAsync(content);
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
        if (string.IsNullOrEmpty(SelectedFilePath) || ImportValidation == null || !ImportValidation.IsValid)
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
            var content = await File.ReadAllTextAsync(SelectedFilePath);
            var success = await _exportService.ImportFromJsonAsync(content, ReplaceExisting);

            if (success)
            {
                await Shell.Current.DisplayAlert(
                    "Import Complete",
                    "Data imported successfully. Please restart the app to see changes.",
                    "OK");

                // Clear import state
                SelectedFilePath = null;
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
