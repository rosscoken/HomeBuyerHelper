using System.Text.Json;
using Microsoft.JSInterop;

namespace HomeBuyerHelper.Web.Storage;

/// <summary>
/// A typed collection persisted as JSON in browser localStorage.
/// Stands in for the SQLite tables used by the native app — fine for the
/// short-term web preview; data stays in the visitor's browser.
/// </summary>
public class LocalStore<T> where T : class
{
    private readonly IJSInProcessRuntime _js;
    private readonly string _key;
    private readonly Func<T, int> _getId;
    private readonly Action<T, int> _setId;
    private List<T>? _items;

    public LocalStore(IJSRuntime js, string key, Func<T, int> getId, Action<T, int> setId)
    {
        // Blazor WebAssembly always provides an in-process runtime.
        _js = (IJSInProcessRuntime)js;
        _key = $"hbh_{key}";
        _getId = getId;
        _setId = setId;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public List<T> Items
    {
        get
        {
            if (_items == null)
            {
                var json = _js.Invoke<string?>("localStorage.getItem", _key);
                _items = string.IsNullOrEmpty(json)
                    ? new List<T>()
                    : JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? new List<T>();
            }
            return _items;
        }
    }

    public void Save()
    {
        _js.InvokeVoid("localStorage.setItem", _key, JsonSerializer.Serialize(Items, JsonOptions));
    }

    public int Add(T item)
    {
        var nextId = Items.Count == 0 ? 1 : Items.Max(_getId) + 1;
        _setId(item, nextId);
        Items.Add(item);
        Save();
        return nextId;
    }

    public void Update(T item)
    {
        var id = _getId(item);
        var index = Items.FindIndex(existing => _getId(existing) == id);
        if (index >= 0)
        {
            Items[index] = item;
            Save();
        }
    }

    public void Delete(int id)
    {
        Items.RemoveAll(item => _getId(item) == id);
        Save();
    }

    public T? Find(int id) => Items.FirstOrDefault(item => _getId(item) == id);
}
