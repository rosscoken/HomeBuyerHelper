namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Optional cloud sync abstraction (design spec section 6.4, P3-SYN-001).
/// All sync uses the user's own cloud account; nothing is ever sent to
/// application servers.
/// </summary>
public interface ICloudSyncService
{
    /// <summary>
    /// The provider this service syncs to.
    /// </summary>
    CloudProvider Provider { get; }

    /// <summary>
    /// Whether this provider is available on the current platform/build.
    /// </summary>
    bool IsAvailable { get; }

    /// <summary>
    /// Pushes the current database backup to the user's cloud storage.
    /// </summary>
    Task<SyncResult> BackupAsync();

    /// <summary>
    /// Restores the most recent backup from the user's cloud storage.
    /// </summary>
    Task<SyncResult> RestoreAsync();
}

/// <summary>
/// Supported cloud providers.
/// </summary>
public enum CloudProvider
{
    None,
    ICloud,
    GoogleDrive,
    OneDrive,
    Dropbox
}

/// <summary>
/// Result of a sync operation.
/// </summary>
public class SyncResult
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
