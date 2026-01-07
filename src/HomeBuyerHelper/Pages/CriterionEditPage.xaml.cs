using HomeBuyerHelper.ViewModels;

namespace HomeBuyerHelper.Pages;

/// <summary>
/// Page for editing or creating a criterion.
/// </summary>
public partial class CriterionEditPage : ContentPage
{
    public CriterionEditPage(CriterionEditViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
