using HomeBuyerHelper.ViewModels;

namespace HomeBuyerHelper.Pages;

/// <summary>
/// Page for scoring a property through all criteria.
/// </summary>
public partial class ScoringWalkthroughPage : ContentPage
{
    public ScoringWalkthroughPage(ScoringWalkthroughViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
