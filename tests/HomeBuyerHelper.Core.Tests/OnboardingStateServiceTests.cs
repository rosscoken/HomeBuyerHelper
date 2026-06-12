using FluentAssertions;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Core.Services;
using Xunit;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for OnboardingStateService persistence and caching.
/// </summary>
public class OnboardingStateServiceTests
{
    private sealed class InMemoryKeyValueStore : IKeyValueStore
    {
        private readonly Dictionary<string, string> _values = new();

        public string Get(string key, string defaultValue) =>
            _values.TryGetValue(key, out var value) ? value : defaultValue;

        public void Set(string key, string value) => _values[key] = value;

        public void Remove(string key) => _values.Remove(key);
    }

    [Fact]
    public void GetState_WhenNoSavedState_ReturnsDefaultState()
    {
        // Arrange
        var service = new OnboardingStateService(new InMemoryKeyValueStore());

        // Act
        var state = service.GetState();

        // Assert
        state.Should().NotBeNull();
        state.CurrentStep.Should().Be(1);
        state.SelectedCriteria.Should().BeEmpty();
    }

    [Fact]
    public void SaveState_ThenGetState_RoundTripsThroughStore()
    {
        // Arrange
        var store = new InMemoryKeyValueStore();
        var first = new OnboardingStateService(store);

        var state = first.GetState();
        state.CurrentStep = 3;
        state.BuyingSituation = BuyingSituation.FirstHome;
        state.LocationPriorities.Add("Short commute to work");
        first.SaveState(state);

        // Act - new service instance reads from the same backing store
        var second = new OnboardingStateService(store);
        var loaded = second.GetState();

        // Assert
        loaded.CurrentStep.Should().Be(3);
        loaded.BuyingSituation.Should().Be(BuyingSituation.FirstHome);
        loaded.LocationPriorities.Should().ContainSingle()
            .Which.Should().Be("Short commute to work");
    }

    [Fact]
    public void ClearState_RemovesSavedState()
    {
        // Arrange
        var store = new InMemoryKeyValueStore();
        var service = new OnboardingStateService(store);
        var state = service.GetState();
        state.CurrentStep = 5;
        service.SaveState(state);

        // Act
        service.ClearState();

        // Assert
        var reloaded = new OnboardingStateService(store).GetState();
        reloaded.CurrentStep.Should().Be(1);
    }

    [Fact]
    public void GetState_WithCorruptStoredJson_ReturnsDefaultState()
    {
        // Arrange
        var store = new InMemoryKeyValueStore();
        store.Set("onboarding_state", "{ corrupt json !");
        var service = new OnboardingStateService(store);

        // Act
        var state = service.GetState();

        // Assert
        state.CurrentStep.Should().Be(1);
    }

    [Fact]
    public void IsOnboardingInProgress_ReflectsCurrentStep()
    {
        // Arrange
        var service = new OnboardingStateService(new InMemoryKeyValueStore());

        // Assert - fresh state is step 1, not in progress
        service.IsOnboardingInProgress().Should().BeFalse();

        // Act
        var state = service.GetState();
        state.CurrentStep = 2;
        service.SaveState(state);

        // Assert
        service.IsOnboardingInProgress().Should().BeTrue();
    }
}
