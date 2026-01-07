using HomeBuyerHelper.ViewModels;

namespace HomeBuyerHelper.Pages;

/// <summary>
/// Property comparison page.
/// </summary>
public partial class ComparisonPage : ContentPage
{
    public ComparisonPage(ComparisonViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ComparisonViewModel vm)
        {
            await vm.OnAppearingAsync();
        }
    }
}
