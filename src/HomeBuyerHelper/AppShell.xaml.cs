namespace HomeBuyerHelper;

/// <summary>
/// Application shell providing navigation structure.
/// </summary>
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(Pages.PropertyListPage), typeof(Pages.PropertyListPage));
        Routing.RegisterRoute(nameof(Pages.PropertyDetailPage), typeof(Pages.PropertyDetailPage));
        Routing.RegisterRoute(nameof(Pages.OnboardingPage), typeof(Pages.OnboardingPage));
        Routing.RegisterRoute(nameof(Pages.CriteriaPage), typeof(Pages.CriteriaPage));
        Routing.RegisterRoute(nameof(Pages.ComparisonPage), typeof(Pages.ComparisonPage));
    }
}
