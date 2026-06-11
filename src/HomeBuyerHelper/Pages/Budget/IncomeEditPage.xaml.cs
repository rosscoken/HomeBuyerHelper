using HomeBuyerHelper.ViewModels.Budget;

namespace HomeBuyerHelper.Pages.Budget;

/// <summary>
/// Income source add/edit page.
/// </summary>
public partial class IncomeEditPage : ContentPage
{
    public IncomeEditPage(IncomeEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
