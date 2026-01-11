using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;

namespace HomeBuyerHelper.ViewModels.Onboarding;

/// <summary>
/// ViewModel for the welcome screen (P1-ONB-001).
/// </summary>
public partial class WelcomeViewModel : BaseViewModel
{
    private readonly IExportService _exportService;
    private readonly IUserPreferencesRepository _preferencesRepository;

    public WelcomeViewModel(
        IExportService exportService,
        IUserPreferencesRepository preferencesRepository)
    {
        _exportService = exportService;
        _preferencesRepository = preferencesRepository;
        Title = "Welcome";
    }

    [RelayCommand]
    private async Task GetStartedAsync()
    {
        await Shell.Current.GoToAsync("GoalSelection");
    }

    [RelayCommand]
    private async Task ImportBackupAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select backup file",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "public.json" } },
                        { DevicePlatform.Android, new[] { "application/json" } },
                        { DevicePlatform.WinUI, new[] { ".json" } },
                        { DevicePlatform.macOS, new[] { "json" } }
                    })
                });

                if (result != null)
                {
                    await using var stream = await result.OpenReadAsync();
                    using var reader = new StreamReader(stream);
                    var json = await reader.ReadToEndAsync();

                    var success = await _exportService.ImportFromJsonAsync(json);
                    if (success)
                    {
                        await _preferencesRepository.SetOnboardingCompleteAsync();
                        await Shell.Current.GoToAsync("//Dashboard");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Import Failed",
                            "Could not import the backup file. Please check the file format.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error",
                    $"Failed to import backup: {ex.Message}", "OK");
            }
        });
    }
}
