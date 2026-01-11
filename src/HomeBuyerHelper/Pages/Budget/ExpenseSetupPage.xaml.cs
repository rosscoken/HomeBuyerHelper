using HomeBuyerHelper.ViewModels.Budget;

namespace HomeBuyerHelper.Pages.Budget;

/// <summary>
/// Page for configuring expenses.
/// </summary>
public partial class ExpenseSetupPage : ContentPage
{
    private readonly ExpenseSetupViewModel _viewModel;

    public ExpenseSetupPage(ExpenseSetupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.OnAppearingAsync();
    }
}
