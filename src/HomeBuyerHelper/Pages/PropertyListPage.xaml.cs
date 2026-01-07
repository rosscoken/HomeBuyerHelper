using HomeBuyerHelper.ViewModels;

namespace HomeBuyerHelper.Pages;

/// <summary>
/// Property list page.
/// </summary>
public partial class PropertyListPage : ContentPage
{
    public PropertyListPage(PropertyListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is PropertyListViewModel vm)
        {
            await vm.OnAppearingAsync();
        }
    }
}
