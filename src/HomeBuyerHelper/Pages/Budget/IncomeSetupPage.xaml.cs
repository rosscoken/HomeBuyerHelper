using HomeBuyerHelper.ViewModels.Budget;

namespace HomeBuyerHelper.Pages.Budget;

/// <summary>
/// Income source list page.
/// </summary>
public partial class IncomeSetupPage : ContentPage
{
    public IncomeSetupPage(IncomeSetupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is IncomeSetupViewModel vm)
        {
            await vm.OnAppearingAsync();
        }
    }
}
