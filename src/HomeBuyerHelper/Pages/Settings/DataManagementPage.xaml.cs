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
}
