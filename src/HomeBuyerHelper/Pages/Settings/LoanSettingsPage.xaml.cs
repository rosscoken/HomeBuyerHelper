using HomeBuyerHelper.ViewModels;

namespace HomeBuyerHelper.Pages.Settings;

/// <summary>
/// Page for configuring default loan parameters.
/// </summary>
public partial class LoanSettingsPage : ContentPage
{
    private readonly LoanSettingsViewModel _viewModel;

    public LoanSettingsPage(LoanSettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.OnAppearingAsync();
    }

    private void OnTermSelected(object? sender, EventArgs e)
    {
        if (sender is Button button && button.Text.Contains("15"))
        {
            _viewModel.MortgageTermYears = 15;
        }
        else if (sender is Button button2 && button2.Text.Contains("20"))
        {
            _viewModel.MortgageTermYears = 20;
        }
        else
        {
            _viewModel.MortgageTermYears = 30;
        }
    }
}
