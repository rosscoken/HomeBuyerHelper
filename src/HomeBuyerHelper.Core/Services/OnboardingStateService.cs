using System.Text.Json;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Services;

/// <summary>
/// Service for managing onboarding flow state.
/// Persists state to Preferences for session continuity.
/// </summary>
public class OnboardingStateService : IOnboardingStateService
{
    private const string StateKey = "onboarding_state";
    private OnboardingState? _cachedState;

    public OnboardingState GetState()
    {
        if (_cachedState != null)
            return _cachedState;

        var json = Preferences.Get(StateKey, string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                _cachedState = JsonSerializer.Deserialize<OnboardingState>(json);
            }
            catch (JsonException)
            {
                // Invalid stored state - reset to default
                _cachedState = null;
            }
        }

        _cachedState ??= new OnboardingState();
        return _cachedState;
    }

    public void SaveState(OnboardingState state)
    {
        state.UpdatedAt = DateTime.UtcNow;
        _cachedState = state;

        var json = JsonSerializer.Serialize(state);
        Preferences.Set(StateKey, json);
    }

    public void ClearState()
    {
        _cachedState = null;
        Preferences.Remove(StateKey);
    }

    public bool IsOnboardingInProgress()
    {
        var state = GetState();
        return state.CurrentStep > 1;
    }
}
