using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Data.Entities;
using SQLite;

namespace HomeBuyerHelper.Data;

/// <summary>
/// SQLite database service implementation.
/// </summary>
public class DatabaseService : IDatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _databasePath;

    public DatabaseService()
        : this(null)
    {
    }

    /// <summary>
    /// Creates a database service with an explicit database file path.
    /// Used by tests to isolate database files; the app uses the default path.
    /// </summary>
    public DatabaseService(string? databasePath)
    {
        _databasePath = databasePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "HomeBuyerHelper",
            "homebuyerhelper.db");
    }

    public string DatabasePath => _databasePath;

    private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
    {
        if (_database != null)
        {
            return _database;
        }

        // Ensure directory exists
        var directory = Path.GetDirectoryName(_databasePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _database = new SQLiteAsyncConnection(_databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        await InitializeAsync();
        return _database;
    }

    /// <summary>
    /// Gets the database connection, initializing if needed.
    /// </summary>
    public Task<SQLiteAsyncConnection> GetConnectionAsync() => GetDatabaseAsync();

    public async Task InitializeAsync()
    {
        if (_database == null)
        {
            await GetDatabaseAsync();
            return;
        }

        // Create all tables
        await _database.CreateTableAsync<PropertyEntity>();
        await _database.CreateTableAsync<EvaluationCriterionEntity>();
        await _database.CreateTableAsync<PropertyScoreEntity>();
        await _database.CreateTableAsync<UserPreferencesEntity>();
        await _database.CreateTableAsync<IncomeSourceEntity>();
        await _database.CreateTableAsync<ExpenseEntity>();
        await _database.CreateTableAsync<FundingSourceEntity>();
        await _database.CreateTableAsync<OneTimeEventEntity>();
    }

    public async Task<string> ExportBackupAsync()
    {
        var backupDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "HomeBuyerHelper",
            "Backups");

        if (!Directory.Exists(backupDir))
        {
            Directory.CreateDirectory(backupDir);
        }

        var backupPath = Path.Combine(backupDir, $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");

        // Close connection before copying
        if (_database != null)
        {
            await _database.CloseAsync();
            _database = null;
        }

        File.Copy(_databasePath, backupPath, overwrite: true);

        return backupPath;
    }

    public async Task ImportBackupAsync(string backupPath)
    {
        if (!File.Exists(backupPath))
        {
            throw new FileNotFoundException("Backup file not found", backupPath);
        }

        // Close connection before replacing
        if (_database != null)
        {
            await _database.CloseAsync();
            _database = null;
        }

        File.Copy(backupPath, _databasePath, overwrite: true);

        // Reinitialize connection
        await GetDatabaseAsync();
    }

    public async Task ClearAllDataAsync()
    {
        var db = await GetDatabaseAsync();

        await db.DeleteAllAsync<PropertyScoreEntity>();
        await db.DeleteAllAsync<PropertyEntity>();
        await db.DeleteAllAsync<EvaluationCriterionEntity>();
        await db.DeleteAllAsync<UserPreferencesEntity>();
        await db.DeleteAllAsync<IncomeSourceEntity>();
        await db.DeleteAllAsync<ExpenseEntity>();
        await db.DeleteAllAsync<FundingSourceEntity>();
        await db.DeleteAllAsync<OneTimeEventEntity>();
    }
}
