using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Repository interface for UserPreferences operations.
/// </summary>
public interface IUserPreferencesRepository
{
    /// <summary>
    /// Gets the current user preferences (creates default if none exist).
    /// </summary>
    Task<UserPreferences> GetAsync();

    /// <summary>
    /// Saves/updates user preferences.
    /// </summary>
    Task SaveAsync(UserPreferences preferences);

    /// <summary>
    /// Resets preferences to defaults.
    /// </summary>
    Task ResetAsync();

    /// <summary>
    /// Checks if the user has completed onboarding.
    /// </summary>
    Task<bool> HasCompletedOnboardingAsync();

    /// <summary>
    /// Marks onboarding as complete.
    /// </summary>
    Task SetOnboardingCompleteAsync();
}
