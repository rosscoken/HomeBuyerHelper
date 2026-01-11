using HomeBuyerHelper.ViewModels.Budget;

namespace HomeBuyerHelper.Pages.Budget;

/// <summary>
/// Page for configuring income sources.
/// </summary>
public partial class IncomeSetupPage : ContentPage
{
    private readonly IncomeSetupViewModel _viewModel;

    public IncomeSetupPage(IncomeSetupViewModel viewModel)
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
