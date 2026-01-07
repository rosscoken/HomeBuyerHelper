namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Service interface for database operations.
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Initializes the database and creates tables if needed.
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Gets the path to the database file.
    /// </summary>
    string DatabasePath { get; }

    /// <summary>
    /// Exports the database to a backup file.
    /// </summary>
    /// <returns>The path to the backup file.</returns>
    Task<string> ExportBackupAsync();

    /// <summary>
    /// Imports a database from a backup file.
    /// </summary>
    Task ImportBackupAsync(string backupPath);

    /// <summary>
    /// Clears all data from the database.
    /// </summary>
    Task ClearAllDataAsync();
}
