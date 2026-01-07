using HomeBuyerHelper.ViewModels.Onboarding;

namespace HomeBuyerHelper.Pages.Onboarding;

/// <summary>
/// Home priorities selection page - P1-ONB-006.
/// </summary>
public partial class HomePrioritiesPage : ContentPage
{
    public HomePrioritiesPage(HomePrioritiesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
