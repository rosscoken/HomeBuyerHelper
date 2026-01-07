using HomeBuyerHelper.ViewModels.Onboarding;

namespace HomeBuyerHelper.Pages.Onboarding;

/// <summary>
/// Location priorities selection page - P1-ONB-005.
/// </summary>
public partial class LocationPrioritiesPage : ContentPage
{
    public LocationPrioritiesPage(LocationPrioritiesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
