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

    private async Task CheckOnboardingStatusAsync()
    {
        try
        {
            var preferences = await _preferencesRepository.GetAsync();
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
