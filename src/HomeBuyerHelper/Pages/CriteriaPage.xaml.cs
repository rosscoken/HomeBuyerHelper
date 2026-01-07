using HomeBuyerHelper.ViewModels;

namespace HomeBuyerHelper.Pages;

/// <summary>
/// Evaluation criteria management page.
/// </summary>
public partial class CriteriaPage : ContentPage
{
    public CriteriaPage(CriteriaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is CriteriaViewModel vm)
        {
            await vm.OnAppearingAsync();
        }
    }
}
