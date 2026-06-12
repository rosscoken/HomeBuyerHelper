using HomeBuyerHelper.ViewModels.Budget;

namespace HomeBuyerHelper.Pages.Budget;

/// <summary>
/// Expense list page.
/// </summary>
public partial class ExpenseSetupPage : ContentPage
{
    public ExpenseSetupPage(ExpenseSetupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ExpenseSetupViewModel vm)
        {
            await vm.OnAppearingAsync();
        }
    }
}
