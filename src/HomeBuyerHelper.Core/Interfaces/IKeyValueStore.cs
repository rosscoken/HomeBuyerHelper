namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Abstraction over simple key/value persistence (e.g., MAUI Preferences).
/// Allows Core services to persist small state without a platform dependency.
/// </summary>
public interface IKeyValueStore
{
    /// <summary>
    /// Gets the value for a key, or the default if not present.
    /// </summary>
    string Get(string key, string defaultValue);

    /// <summary>
    /// Sets the value for a key.
    /// </summary>
    void Set(string key, string value);

    /// <summary>
    /// Removes a key and its value.
    /// </summary>
    void Remove(string key);
}
