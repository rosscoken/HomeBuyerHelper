using HomeBuyerHelper.ViewModels.Onboarding;

namespace HomeBuyerHelper.Pages.Onboarding;

/// <summary>
/// Property count question page - P1-ONB-003.
/// </summary>
public partial class PropertyCountPage : ContentPage
{
    public PropertyCountPage(PropertyCountViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
