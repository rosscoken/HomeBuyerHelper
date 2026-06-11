using HomeBuyerHelper.ViewModels.Budget;

namespace HomeBuyerHelper.Pages.Budget;

/// <summary>
/// Expense add/edit page.
/// </summary>
public partial class ExpenseEditPage : ContentPage
{
    public ExpenseEditPage(ExpenseEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
