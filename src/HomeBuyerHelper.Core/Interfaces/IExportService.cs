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
    /// Imports data from JSON content.
    /// </summary>
    /// <param name="jsonContent">JSON backup content.</param>
    /// <returns>True if import was successful.</returns>
    Task<bool> ImportFromJsonAsync(string jsonContent);

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
