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

    [ObservableProperty]
    private IReadOnlyList<Property> _properties = [];

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private Property? _selectedProperty;

    public PropertyListViewModel(IPropertyService propertyService)
    {
        _propertyService = propertyService;
        Title = "Properties";
    }

    public override async Task OnAppearingAsync()
    {
        await LoadPropertiesAsync();
    }

    [RelayCommand]
    private async Task LoadPropertiesAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            Properties = await _propertyService.GetPropertiesWithScoresAsync();
            IsEmpty = Properties.Count == 0;
        });
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
