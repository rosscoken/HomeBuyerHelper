using HomeBuyerHelper.ViewModels.Onboarding;

namespace HomeBuyerHelper.Pages.Onboarding;

/// <summary>
/// Household composition questions page - P1-ONB-004.
/// </summary>
public partial class HouseholdPage : ContentPage
{
    public HouseholdPage(HouseholdViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
