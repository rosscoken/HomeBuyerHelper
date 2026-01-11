using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels;

/// <summary>
/// View model for the property list page.
/// </summary>
public partial class PropertyListViewModel : BaseViewModel
{
    private readonly IPropertyService _propertyService;
    private IReadOnlyList<Property> _allProperties = [];

    [ObservableProperty]
    private IReadOnlyList<Property> _properties = [];

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private Property? _selectedProperty;

    [ObservableProperty]
    private string _selectedSort = "Score";

    [ObservableProperty]
    private bool _isRefreshing;

    public IReadOnlyList<string> SortOptions { get; } = new[] { "Score", "Price", "Date Added", "Name" };

    public PropertyListViewModel(IPropertyService propertyService)
    {
        _propertyService = propertyService;
        Title = "Properties";
    }

    partial void OnSelectedSortChanged(string value)
    {
        ApplySort();
    }

    public override async Task OnAppearingAsync()
    {
        await LoadPropertiesAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        try
        {
            await LoadPropertiesInternalAsync();
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task LoadPropertiesAsync()
    {
        await ExecuteBusyAsync(LoadPropertiesInternalAsync);
    }

    private async Task LoadPropertiesInternalAsync()
    {
        _allProperties = await _propertyService.GetPropertiesWithScoresAsync();
        ApplySort();
        IsEmpty = Properties.Count == 0;
    }

    private void ApplySort()
    {
        Properties = SelectedSort switch
        {
            "Score" => _allProperties.OrderByDescending(p => p.OverallScore).ThenBy(p => p.Nickname).ToList(),
            "Price" => _allProperties.OrderBy(p => p.EffectivePrice).ThenBy(p => p.Nickname).ToList(),
            "Date Added" => _allProperties.OrderByDescending(p => p.CreatedAt).ThenBy(p => p.Nickname).ToList(),
            "Name" => _allProperties.OrderBy(p => p.Nickname).ToList(),
            _ => _allProperties.ToList()
        };
    }

    [RelayCommand]
    private async Task AddPropertyAsync()
    {
        await Shell.Current.GoToAsync(nameof(Pages.PropertyDetailPage));
    }

    [RelayCommand]
    private async Task SelectPropertyAsync(Property property)
    {
        await Shell.Current.GoToAsync($"{nameof(Pages.PropertyDetailPage)}?id={property.Id}");
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync(Property property)
    {
        await _propertyService.ToggleFavoriteAsync(property.Id);
        await LoadPropertiesAsync();
    }

    [RelayCommand]
    private async Task DeletePropertyAsync(Property property)
    {
        var confirmed = await Shell.Current.DisplayAlert(
            "Delete Property",
            $"Are you sure you want to delete '{property.Nickname}'? This cannot be undone.",
            "Delete",
            "Cancel");

        if (confirmed)
        {
            await _propertyService.DeletePropertyAsync(property.Id);
            await LoadPropertiesAsync();
        }
    }
}
