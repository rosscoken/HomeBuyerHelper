using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Service for managing onboarding flow state.
/// </summary>
public interface IOnboardingStateService
{
    /// <summary>
    /// Gets the current onboarding state.
    /// </summary>
    OnboardingState GetState();

    /// <summary>
    /// Saves the current onboarding state.
    /// </summary>
    void SaveState(OnboardingState state);

    /// <summary>
    /// Clears the onboarding state.
    /// </summary>
    void ClearState();

    /// <summary>
    /// Checks if onboarding is in progress.
    /// </summary>
    bool IsOnboardingInProgress();
}
