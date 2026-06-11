using HomeBuyerHelper.ViewModels.Budget;

namespace HomeBuyerHelper.Pages.Budget;

/// <summary>
/// One-time events calendar page.
/// </summary>
public partial class OneTimeEventsPage : ContentPage
{
    public OneTimeEventsPage(OneTimeEventsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is OneTimeEventsViewModel vm)
        {
            await vm.OnAppearingAsync();
        }
    }
}
