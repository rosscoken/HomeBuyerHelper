using HomeBuyerHelper.ViewModels.Settings;

namespace HomeBuyerHelper.Pages.Settings;

/// <summary>
/// Commute analysis configuration page.
/// </summary>
public partial class CommuteSettingsPage : ContentPage
{
    public CommuteSettingsPage(CommuteSettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is CommuteSettingsViewModel vm)
        {
            await vm.OnAppearingAsync();
        }
    }
}
