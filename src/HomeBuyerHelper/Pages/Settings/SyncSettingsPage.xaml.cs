using HomeBuyerHelper.ViewModels.Settings;

namespace HomeBuyerHelper.Pages.Settings;

/// <summary>
/// Optional cloud backup page.
/// </summary>
public partial class SyncSettingsPage : ContentPage
{
    public SyncSettingsPage(SyncSettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
