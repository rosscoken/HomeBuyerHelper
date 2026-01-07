using HomeBuyerHelper.Pages.Onboarding;
using HomeBuyerHelper.Pages.Settings;

namespace HomeBuyerHelper;

/// <summary>
/// Application shell providing navigation structure.
/// </summary>
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register main app routes
        Routing.RegisterRoute(nameof(Pages.PropertyListPage), typeof(Pages.PropertyListPage));
        Routing.RegisterRoute(nameof(Pages.PropertyDetailPage), typeof(Pages.PropertyDetailPage));
        Routing.RegisterRoute(nameof(Pages.CriteriaPage), typeof(Pages.CriteriaPage));
        Routing.RegisterRoute("CriterionEdit", typeof(Pages.CriterionEditPage));
        Routing.RegisterRoute(nameof(Pages.ComparisonPage), typeof(Pages.ComparisonPage));
        Routing.RegisterRoute("PropertyEntry", typeof(Pages.PropertyDetailPage));
        Routing.RegisterRoute("ScoringWalkthrough", typeof(Pages.ScoringWalkthroughPage));
        Routing.RegisterRoute("LoanSettings", typeof(LoanSettingsPage));
        Routing.RegisterRoute("DataManagement", typeof(DataManagementPage));

        // Register onboarding flow routes
        Routing.RegisterRoute("Welcome", typeof(WelcomePage));
        Routing.RegisterRoute("GoalSelection", typeof(GoalSelectionPage));
        Routing.RegisterRoute("PropertyCount", typeof(PropertyCountPage));
        Routing.RegisterRoute("Household", typeof(HouseholdPage));
        Routing.RegisterRoute("LocationPriorities", typeof(LocationPrioritiesPage));
        Routing.RegisterRoute("HomePriorities", typeof(HomePrioritiesPage));
        Routing.RegisterRoute("CriteriaReview", typeof(CriteriaReviewPage));
        Routing.RegisterRoute("OnboardingComplete", typeof(OnboardingCompletePage));
    }
}
