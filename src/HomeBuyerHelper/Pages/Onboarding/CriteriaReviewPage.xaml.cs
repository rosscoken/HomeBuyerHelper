using HomeBuyerHelper.ViewModels.Onboarding;

namespace HomeBuyerHelper.Pages.Onboarding;

/// <summary>
/// Criteria review page - P1-ONB-008.
/// </summary>
public partial class CriteriaReviewPage : ContentPage
{
    public CriteriaReviewPage(CriteriaReviewViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
