using HomeBuyerHelper.ViewModels.Analysis;

namespace HomeBuyerHelper.Pages.Analysis;

/// <summary>
/// Rent vs. buy calculator page.
/// </summary>
public partial class RentVsBuyPage : ContentPage
{
    public RentVsBuyPage(RentVsBuyViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is RentVsBuyViewModel vm)
        {
            await vm.OnAppearingAsync();
        }
    }
}
