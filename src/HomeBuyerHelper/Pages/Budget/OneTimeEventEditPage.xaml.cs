using HomeBuyerHelper.ViewModels.Budget;

namespace HomeBuyerHelper.Pages.Budget;

/// <summary>
/// One-time event add/edit page.
/// </summary>
public partial class OneTimeEventEditPage : ContentPage
{
    public OneTimeEventEditPage(OneTimeEventEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
