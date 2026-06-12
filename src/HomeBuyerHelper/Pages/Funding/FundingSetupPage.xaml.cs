using HomeBuyerHelper.ViewModels.Funding;

namespace HomeBuyerHelper.Pages.Funding;

/// <summary>
/// Funding plan summary page.
/// </summary>
public partial class FundingSetupPage : ContentPage
{
    public FundingSetupPage(FundingSetupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is FundingSetupViewModel vm)
        {
            await vm.OnAppearingAsync();
        }
    }
}
