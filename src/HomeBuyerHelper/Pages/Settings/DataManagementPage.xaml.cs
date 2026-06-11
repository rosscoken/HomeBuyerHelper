using HomeBuyerHelper.ViewModels;

namespace HomeBuyerHelper.Pages.Settings;

/// <summary>
/// Page for data export and import functionality.
/// </summary>
public partial class DataManagementPage : ContentPage
{
    public DataManagementPage(DataManagementViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is DataManagementViewModel vm)
        {
            await vm.OnAppearingAsync();
        }
    }
}
