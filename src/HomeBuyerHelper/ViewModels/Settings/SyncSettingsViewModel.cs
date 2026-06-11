using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;

namespace HomeBuyerHelper.ViewModels.Settings;

/// <summary>
/// ViewModel for the optional cloud backup page (P3-SYN-001/006).
/// Manual backup via the share sheet sends the backup file to whatever
/// cloud storage the user chooses — their account, their data.
/// </summary>
public partial class SyncSettingsViewModel : BaseViewModel
{
    private readonly ICloudSyncService _syncService;

    [ObservableProperty]
    private string? _lastResultMessage;

    [ObservableProperty]
    private bool _lastResultSuccess;

    public SyncSettingsViewModel(ICloudSyncService syncService)
    {
        _syncService = syncService;
        Title = "Cloud Backup";
    }

    [RelayCommand]
    private async Task BackupNowAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var result = await _syncService.BackupAsync();
            LastResultMessage = result.Message;
            LastResultSuccess = result.Success;
        });
    }

    [RelayCommand]
    private async Task RestoreAsync()
    {
        var confirmed = await Shell.Current.DisplayAlert(
            "Restore Backup",
            "Restoring replaces ALL current data with the backup's contents. Continue?",
            "Restore",
            "Cancel");

        if (!confirmed) return;

        await ExecuteBusyAsync(async () =>
        {
            var result = await _syncService.RestoreAsync();
            LastResultMessage = result.Message;
            LastResultSuccess = result.Success;
        });
    }
}
