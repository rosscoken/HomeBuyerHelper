using HomeBuyerHelper.Core.Interfaces;

namespace HomeBuyerHelper.Services;

/// <summary>
/// Key/value store backed by MAUI Preferences.
/// </summary>
public class MauiPreferencesStore : IKeyValueStore
{
    public string Get(string key, string defaultValue) => Preferences.Get(key, defaultValue);

    public void Set(string key, string value) => Preferences.Set(key, value);

    public void Remove(string key) => Preferences.Remove(key);
}
