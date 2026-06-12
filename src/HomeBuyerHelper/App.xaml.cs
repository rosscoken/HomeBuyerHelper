using HomeBuyerHelper.Core.Interfaces;

namespace HomeBuyerHelper;

/// <summary>
/// Main application class for HomeBuyerHelper.
/// </summary>
public partial class App : Application
{
    private readonly IUserPreferencesRepository _preferencesRepository;

    public App(IUserPreferencesRepository preferencesRepository)
    {
        InitializeComponent();
        _preferencesRepository = preferencesRepository;
        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        base.OnStart();
        await CheckOnboardingStatusAsync();
    }

    /// <summary>
    /// Applies the persisted theme selection (P4-DRK-002).
    /// </summary>
    public static void ApplyTheme(int themePreference)
    {
        if (Current != null)
        {
            Current.UserAppTheme = themePreference switch
            {
                1 => AppTheme.Light,
                2 => AppTheme.Dark,
                _ => AppTheme.Unspecified
            };
        }
    }

    private async Task CheckOnboardingStatusAsync()
    {
        try
        {
            var preferences = await _preferencesRepository.GetAsync();
            ApplyTheme(preferences.ThemePreference);
            if (!preferences.HasCompletedOnboarding)
            {
                await Shell.Current.GoToAsync("Welcome");
            }
        }
        catch
        {
            // If we can't check preferences, show onboarding to be safe
            await Shell.Current.GoToAsync("Welcome");
        }
    }
}
