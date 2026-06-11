using System.Text.Json;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Services;

/// <summary>
/// Service for managing onboarding flow state.
/// Persists state to a key/value store for session continuity.
/// </summary>
public class OnboardingStateService : IOnboardingStateService
{
    private const string StateKey = "onboarding_state";
    private readonly IKeyValueStore _store;
    private OnboardingState? _cachedState;

    public OnboardingStateService(IKeyValueStore store)
    {
        _store = store;
    }

    public OnboardingState GetState()
    {
        if (_cachedState != null)
            return _cachedState;

        var json = _store.Get(StateKey, string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            try
            {
                _cachedState = JsonSerializer.Deserialize<OnboardingState>(json);
            }
            catch
            {
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
        _store.Set(StateKey, json);
    }

    public void ClearState()
    {
        _cachedState = null;
        _store.Remove(StateKey);
    }

    public bool IsOnboardingInProgress()
    {
        var state = GetState();
        return state.CurrentStep > 1;
    }
}
