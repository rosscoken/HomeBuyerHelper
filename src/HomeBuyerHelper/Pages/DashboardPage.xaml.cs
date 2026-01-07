using HomeBuyerHelper.ViewModels;

namespace HomeBuyerHelper.Pages;

/// <summary>
/// Main dashboard page.
/// </summary>
public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is DashboardViewModel vm)
        {
            await vm.OnAppearingAsync();
        }
    }
}
