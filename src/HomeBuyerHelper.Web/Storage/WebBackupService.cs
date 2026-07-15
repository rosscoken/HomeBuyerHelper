using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.JSInterop;

namespace HomeBuyerHelper.Web.Storage;

/// <summary>
/// Exports and imports the entire app's localStorage data as one versioned
/// JSON document, at the raw-key level rather than through typed models.
/// That keeps backups robust to model evolution: an export made before a
/// field was added/renamed still imports cleanly, because we never
/// deserialize into C# types here — we just move JSON blobs in and out of
/// localStorage keys.
/// </summary>
public class WebBackupService
{
    private const string KeyPrefix = "hbh_";
    private const string LastBackupKey = "hbh_meta_lastBackup";
    private const string AppName = "HomeBuyerHelper";
    private const int SchemaVersion = 1;

    private readonly IJSInProcessRuntime _js;

    public WebBackupService(IJSRuntime js)
    {
        // Blazor WebAssembly always provides an in-process runtime.
        _js = (IJSInProcessRuntime)js;
    }

    /// <summary>
    /// Serializes every hbh_* localStorage key (except the backup-stamp
    /// itself) into one JSON document, stamps "last backup" as a side
    /// effect, and returns the document text.
    /// </summary>
    public string Export()
    {
        var stores = new JsonObject();
        foreach (var key in GetAllAppKeys())
        {
            if (key == LastBackupKey)
            {
                continue;
            }

            var raw = _js.Invoke<string?>("localStorage.getItem", key);
            if (raw == null)
            {
                continue;
            }

            JsonNode? value;
            try
            {
                value = JsonNode.Parse(raw);
            }
            catch (JsonException)
            {
                // Not JSON (shouldn't happen for our own keys) -- preserve
                // the raw text rather than dropping the store.
                value = JsonValue.Create(raw);
            }

            stores[key] = value;
        }

        var document = new JsonObject
        {
            ["app"] = AppName,
            ["schemaVersion"] = SchemaVersion,
            ["exportedAt"] = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
            ["stores"] = stores
        };

        var json = document.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        StampLastBackup();
        return json;
    }

    /// <summary>
    /// Validates a backup document fully, then replaces all current hbh_*
    /// data with its contents. Throws <see cref="InvalidDataException"/>
    /// with a user-facing message on any validation failure; nothing in
    /// localStorage is touched unless validation passes completely.
    /// </summary>
    public void Import(string json)
    {
        JsonNode? root;
        try
        {
            root = JsonNode.Parse(json);
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException("That file isn't valid JSON.", ex);
        }

        if (root is not JsonObject obj)
        {
            throw new InvalidDataException("Backup file must contain a JSON object at its root.");
        }

        if (!obj.TryGetPropertyValue("app", out var appNode) ||
            appNode is not JsonValue appValue ||
            !appValue.TryGetValue<string>(out var appName) ||
            appName != AppName)
        {
            throw new InvalidDataException("This doesn't look like a HomeBuyerHelper backup file.");
        }

        if (!obj.TryGetPropertyValue("schemaVersion", out var versionNode) ||
            versionNode is not JsonValue versionValue ||
            !versionValue.TryGetValue<int>(out var schemaVersion))
        {
            throw new InvalidDataException("Backup file is missing a valid integer schemaVersion.");
        }

        if (schemaVersion != SchemaVersion)
        {
            throw new InvalidDataException(
                $"This backup is schema version {schemaVersion}, but this app only supports version {SchemaVersion}.");
        }

        if (!obj.TryGetPropertyValue("stores", out var storesNode) || storesNode is not JsonObject storesObj)
        {
            throw new InvalidDataException("Backup file is missing its stores data.");
        }

        // Validation is complete. Build the write set before touching
        // localStorage so the import is all-or-nothing.
        var toWrite = new List<(string Key, string Value)>();
        foreach (var (key, value) in storesObj)
        {
            if (string.IsNullOrEmpty(key) ||
                !key.StartsWith(KeyPrefix, StringComparison.Ordinal) ||
                key == LastBackupKey)
            {
                continue;
            }

            toWrite.Add((key, value?.ToJsonString() ?? "null"));
        }

        foreach (var key in GetAllAppKeys().Where(key => key != LastBackupKey).ToList())
        {
            _js.InvokeVoid("localStorage.removeItem", key);
        }

        foreach (var (key, value) in toWrite)
        {
            _js.InvokeVoid("localStorage.setItem", key, value);
        }
    }

    /// <summary>Removes every hbh_* key, including the backup stamp.</summary>
    public void DeleteAllData()
    {
        foreach (var key in GetAllAppKeys())
        {
            _js.InvokeVoid("localStorage.removeItem", key);
        }
    }

    /// <summary>When the last export happened, or null if never backed up.</summary>
    public DateTime? GetLastBackupAt()
    {
        var raw = _js.Invoke<string?>("localStorage.getItem", LastBackupKey);
        if (string.IsNullOrEmpty(raw))
        {
            return null;
        }

        return DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed)
            ? parsed
            : null;
    }

    private void StampLastBackup()
    {
        _js.InvokeVoid("localStorage.setItem", LastBackupKey, DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Enumerates localStorage keys via IJSInProcessRuntime (no bespoke JS
    /// helper needed: localStorage.key(i) returns null past the end).
    /// </summary>
    private List<string> GetAllAppKeys()
    {
        var keys = new List<string>();
        for (var i = 0; ; i++)
        {
            var key = _js.Invoke<string?>("localStorage.key", i);
            if (key == null)
            {
                break;
            }

            if (key.StartsWith(KeyPrefix, StringComparison.Ordinal))
            {
                keys.Add(key);
            }
        }

        return keys;
    }
}
