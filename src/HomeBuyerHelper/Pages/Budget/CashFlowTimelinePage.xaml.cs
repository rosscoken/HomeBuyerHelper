using HomeBuyerHelper.ViewModels.Budget;

namespace HomeBuyerHelper.Pages.Budget;

/// <summary>
/// Month-by-month cash flow timeline page.
/// </summary>
public partial class CashFlowTimelinePage : ContentPage
{
    public CashFlowTimelinePage(CashFlowTimelineViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is CashFlowTimelineViewModel vm)
        {
            await vm.OnAppearingAsync();
        }
    }
}
