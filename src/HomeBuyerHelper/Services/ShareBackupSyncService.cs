using HomeBuyerHelper.Core.Interfaces;

namespace HomeBuyerHelper.Services;

/// <summary>
/// Manual cloud backup via the platform share sheet (P3-SYN-006).
/// The user picks their own destination (iCloud Drive, Google Drive,
/// OneDrive, Dropbox, etc.) so no provider SDKs or accounts are required.
/// Native per-provider auto-sync (P3-SYN-002/003/004) requires
/// platform-specific SDK integration on device builds.
/// </summary>
public class ShareBackupSyncService : ICloudSyncService
{
    private readonly IExportService _exportService;

    public ShareBackupSyncService(IExportService exportService)
    {
        _exportService = exportService;
    }

    public CloudProvider Provider => CloudProvider.None;

    public bool IsAvailable => true;

    public async Task<SyncResult> BackupAsync()
    {
        try
        {
            var filePath = await _exportService.ExportAllDataAsync();

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Backup HomeBuyerHelper Data",
                File = new ShareFile(filePath, "application/json")
            });

            return new SyncResult { Success = true, Message = "Backup shared." };
        }
        catch (Exception ex)
        {
            return new SyncResult { Success = false, Message = ex.Message };
        }
    }

    public async Task<SyncResult> RestoreAsync()
    {
        try
        {
            var fileResult = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select HomeBuyerHelper Backup"
            });

            if (fileResult == null)
            {
                return new SyncResult { Success = false, Message = "No file selected." };
            }

            var json = await File.ReadAllTextAsync(fileResult.FullPath);
            var imported = await _exportService.ImportFromJsonAsync(json, replaceExisting: true);

            return new SyncResult
            {
                Success = imported,
                Message = imported ? "Backup restored." : "The selected file is not a valid backup."
            };
        }
        catch (Exception ex)
        {
            return new SyncResult { Success = false, Message = ex.Message };
        }
    }
}
