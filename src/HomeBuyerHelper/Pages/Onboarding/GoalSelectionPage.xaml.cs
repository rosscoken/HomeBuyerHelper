using HomeBuyerHelper.ViewModels.Onboarding;

namespace HomeBuyerHelper.Pages.Onboarding;

/// <summary>
/// Goal selection page - P1-ONB-002.
/// </summary>
public partial class GoalSelectionPage : ContentPage
{
    public GoalSelectionPage(GoalSelectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
