using HomeBuyerHelper.ViewModels.Settings;

namespace HomeBuyerHelper.Pages.Settings;

/// <summary>
/// Tax bracket configuration page.
/// </summary>
public partial class TaxSettingsPage : ContentPage
{
    public TaxSettingsPage(TaxSettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is TaxSettingsViewModel vm)
        {
            await vm.OnAppearingAsync();
        }
    }
}
