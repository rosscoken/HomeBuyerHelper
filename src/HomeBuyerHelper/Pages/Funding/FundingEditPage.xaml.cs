using HomeBuyerHelper.ViewModels.Funding;

namespace HomeBuyerHelper.Pages.Funding;

/// <summary>
/// Funding source add/edit page with type-specific fields.
/// </summary>
public partial class FundingEditPage : ContentPage
{
    public FundingEditPage(FundingEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
