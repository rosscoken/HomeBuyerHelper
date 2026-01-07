using HomeBuyerHelper.ViewModels.Onboarding;

namespace HomeBuyerHelper.Pages.Onboarding;

/// <summary>
/// Welcome screen - first page users see on app launch.
/// </summary>
public partial class WelcomePage : ContentPage
{
    public WelcomePage(WelcomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
