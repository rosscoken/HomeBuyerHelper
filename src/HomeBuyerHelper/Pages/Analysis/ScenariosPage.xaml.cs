using HomeBuyerHelper.ViewModels.Analysis;

namespace HomeBuyerHelper.Pages.Analysis;

/// <summary>
/// "What if" scenario planning page.
/// </summary>
public partial class ScenariosPage : ContentPage
{
    public ScenariosPage(ScenariosViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ScenariosViewModel vm)
        {
            await vm.OnAppearingAsync();
        }
    }
}
