using HomeBuyerHelper.ViewModels;

namespace HomeBuyerHelper.Pages;

/// <summary>
/// Property detail/edit page.
/// </summary>
public partial class PropertyDetailPage : ContentPage
{
    public PropertyDetailPage(PropertyDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
