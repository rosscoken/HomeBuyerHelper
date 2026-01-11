using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Service interface for exporting data to various formats.
/// </summary>
public interface IExportService
{
    /// <summary>
    /// Exports property comparison to PDF.
    /// </summary>
    /// <param name="propertyIds">IDs of properties to include.</param>
    /// <returns>Path to the generated PDF file.</returns>
    Task<string> ExportComparisonToPdfAsync(IEnumerable<int> propertyIds);

    /// <summary>
    /// Exports a single property detail to PDF.
    /// </summary>
    Task<string> ExportPropertyDetailToPdfAsync(int propertyId);

    /// <summary>
    /// Exports budget projections to PDF.
    /// </summary>
    Task<string> ExportBudgetToPdfAsync(int propertyId);

    /// <summary>
    /// Exports all data to a shareable format (JSON backup).
    /// </summary>
    Task<string> ExportAllDataAsync();

    /// <summary>
    /// Imports data from a backup file.
    /// </summary>
    Task ImportDataAsync(string filePath);

    /// <summary>
    /// Imports data from JSON content (merge mode).
    /// </summary>
    /// <param name="jsonContent">JSON backup content.</param>
    /// <returns>True if import was successful.</returns>
    Task<bool> ImportFromJsonAsync(string jsonContent);

    /// <summary>
    /// Imports data from JSON content with option to replace or merge.
    /// </summary>
    /// <param name="jsonContent">JSON backup content.</param>
    /// <param name="replaceExisting">True to replace all existing data, false to merge.</param>
    /// <returns>True if import was successful.</returns>
    Task<bool> ImportFromJsonAsync(string jsonContent, bool replaceExisting);

    /// <summary>
    /// Validates an import file and returns summary info.
    /// </summary>
    /// <param name="jsonContent">JSON backup content to validate.</param>
    /// <returns>Validation result with file contents summary.</returns>
    Task<ImportValidationResult> ValidateImportFileAsync(string jsonContent);

    /// <summary>
    /// Generates a share-ready summary of a property.
    /// </summary>
    Task<string> GenerateShareTextAsync(int propertyId);

    /// <summary>
    /// Gets available export formats.
    /// </summary>
    IReadOnlyList<ExportFormat> GetAvailableFormats();
}

/// <summary>
/// Available export format.
/// </summary>
public class ExportFormat
{
    public required string Name { get; init; }
    public required string Extension { get; init; }
    public required string MimeType { get; init; }
    public bool IsSupported { get; init; }
}

/// <summary>
/// Result of import file validation.
/// </summary>
public class ImportValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public string Version { get; set; } = string.Empty;
    public DateTime ExportDate { get; set; }
    public int PropertyCount { get; set; }
    public int CriteriaCount { get; set; }
    public int ScoreCount { get; set; }
    public bool HasSettings { get; set; }
}
