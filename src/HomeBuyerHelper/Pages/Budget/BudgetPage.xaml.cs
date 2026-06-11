using HomeBuyerHelper.ViewModels.Budget;

namespace HomeBuyerHelper.Pages.Budget;

/// <summary>
/// Budget overview hub page.
/// </summary>
public partial class BudgetPage : ContentPage
{
    public BudgetPage(BudgetOverviewViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is BudgetOverviewViewModel vm)
        {
            await vm.OnAppearingAsync();
        }
    }
}
