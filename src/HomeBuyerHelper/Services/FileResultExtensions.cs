namespace HomeBuyerHelper.Services;

/// <summary>
/// Helpers for reading picked files. FilePicker results don't reliably
/// expose a readable FullPath on all platforms (Android SAF, iOS
/// security-scoped URLs), so content must be read through the stream API.
/// </summary>
public static class FileResultExtensions
{
    /// <summary>
    /// Reads the picked file's full content as text via its stream.
    /// </summary>
    public static async Task<string> ReadAllTextAsync(this FileResult fileResult)
    {
        using var stream = await fileResult.OpenReadAsync();
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}
