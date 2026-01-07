using HomeBuyerHelper.ViewModels.Onboarding;

namespace HomeBuyerHelper.Pages.Onboarding;

/// <summary>
/// Onboarding complete page - P1-ONB-012.
/// </summary>
public partial class OnboardingCompletePage : ContentPage
{
    public OnboardingCompletePage(OnboardingCompleteViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
