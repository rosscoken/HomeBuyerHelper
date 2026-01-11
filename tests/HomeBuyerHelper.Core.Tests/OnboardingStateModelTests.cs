using FluentAssertions;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for the OnboardingState model.
/// </summary>
public class OnboardingStateModelTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var state = new OnboardingState();

        // Assert
        state.Id.Should().Be(0);
        state.CurrentStep.Should().Be(1);
        state.BuyingSituation.Should().BeNull();
        state.PropertyCount.Should().BeNull();
        state.HouseholdType.Should().BeNull();
        state.WorkArrangement.Should().BeNull();
        state.Pets.Should().Be(PetType.None);
        state.LocationPriorities.Should().BeEmpty();
        state.HomePriorities.Should().BeEmpty();
        state.RankedPriorities.Should().BeEmpty();
        state.SelectedCriteria.Should().BeEmpty();
        state.StartedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        state.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(BuyingSituation.FirstHome)]
    [InlineData(BuyingSituation.Upgrading)]
    [InlineData(BuyingSituation.Downsizing)]
    [InlineData(BuyingSituation.Relocating)]
    [InlineData(BuyingSituation.InvestmentProperty)]
    public void BuyingSituation_AcceptsAllValidValues(BuyingSituation situation)
    {
        // Arrange & Act
        var state = new OnboardingState { BuyingSituation = situation };

        // Assert
        state.BuyingSituation.Should().Be(situation);
    }

    [Theory]
    [InlineData(HouseholdType.JustMe)]
    [InlineData(HouseholdType.WithPartner)]
    [InlineData(HouseholdType.FamilyWithKids)]
    [InlineData(HouseholdType.Roommates)]
    [InlineData(HouseholdType.MultiGenerational)]
    public void HouseholdType_AcceptsAllValidValues(HouseholdType householdType)
    {
        // Arrange & Act
        var state = new OnboardingState { HouseholdType = householdType };

        // Assert
        state.HouseholdType.Should().Be(householdType);
    }

    [Fact]
    public void PetType_IsFlags_SupportsMultipleValues()
    {
        // Arrange & Act
        var state = new OnboardingState
        {
            Pets = PetType.Dogs | PetType.Cats
        };

        // Assert
        state.Pets.HasFlag(PetType.Dogs).Should().BeTrue();
        state.Pets.HasFlag(PetType.Cats).Should().BeTrue();
        state.Pets.HasFlag(PetType.Other).Should().BeFalse();
    }

    [Fact]
    public void PetType_None_RepresentsNoPets()
    {
        // Arrange & Act
        var state = new OnboardingState { Pets = PetType.None };

        // Assert
        state.Pets.Should().Be(PetType.None);
        state.Pets.HasFlag(PetType.Dogs).Should().BeFalse();
    }

    [Fact]
    public void PetType_AllPets_CanBeCombined()
    {
        // Arrange & Act
        var state = new OnboardingState
        {
            Pets = PetType.Dogs | PetType.Cats | PetType.Other
        };

        // Assert
        state.Pets.HasFlag(PetType.Dogs).Should().BeTrue();
        state.Pets.HasFlag(PetType.Cats).Should().BeTrue();
        state.Pets.HasFlag(PetType.Other).Should().BeTrue();
    }

    [Fact]
    public void LocationPriorities_CanBeAdded()
    {
        // Arrange
        var state = new OnboardingState();

        // Act
        state.LocationPriorities.Add("Short commute");
        state.LocationPriorities.Add("Good schools");
        state.LocationPriorities.Add("Walkable");

        // Assert
        state.LocationPriorities.Should().HaveCount(3);
        state.LocationPriorities.Should().Contain("Short commute");
    }

    [Fact]
    public void HomePriorities_CanBeAdded()
    {
        // Arrange
        var state = new OnboardingState();

        // Act
        state.HomePriorities.Add("Move-in ready");
        state.HomePriorities.Add("Home office space");
        state.HomePriorities.Add("Large yard");

        // Assert
        state.HomePriorities.Should().HaveCount(3);
        state.HomePriorities.Should().Contain("Home office space");
    }

    [Fact]
    public void RankedPriorities_CombinesLocationAndHomePriorities()
    {
        // Arrange
        var state = new OnboardingState();

        // Act
        state.RankedPriorities.Add("Short commute");
        state.RankedPriorities.Add("Move-in ready");
        state.RankedPriorities.Add("Good schools");
        state.RankedPriorities.Add("Large yard");

        // Assert
        state.RankedPriorities.Should().HaveCount(4);
        state.RankedPriorities[0].Should().Be("Short commute"); // First priority
    }

    [Fact]
    public void SelectedCriteria_CanBeAdded()
    {
        // Arrange
        var state = new OnboardingState();
        var criterion = new CriterionSelection
        {
            Name = "Kitchen Quality",
            Category = CriterionCategory.Interior,
            Weight = 15,
            ScoreAnchorLow = "Outdated",
            ScoreAnchorHigh = "Dream kitchen",
            SuggestionReason = "Based on your preference for move-in ready"
        };

        // Act
        state.SelectedCriteria.Add(criterion);

        // Assert
        state.SelectedCriteria.Should().HaveCount(1);
        state.SelectedCriteria[0].Name.Should().Be("Kitchen Quality");
        state.SelectedCriteria[0].Weight.Should().Be(15);
    }

    [Fact]
    public void CurrentStep_TracksProgress()
    {
        // Arrange
        var state = new OnboardingState();

        // Act
        state.CurrentStep = 5;

        // Assert
        state.CurrentStep.Should().Be(5);
    }
}

/// <summary>
/// Tests for the CriterionSelection class.
/// </summary>
public class CriterionSelectionTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var selection = new CriterionSelection { Name = "Test" };

        // Assert
        selection.Category.Should().Be(CriterionCategory.Location); // Default enum value
        selection.Weight.Should().Be(0);
        selection.IsLocked.Should().BeFalse();
        selection.ScoreAnchorLow.Should().BeNull();
        selection.ScoreAnchorHigh.Should().BeNull();
        selection.SuggestionReason.Should().BeNull();
    }

    [Fact]
    public void AllProperties_CanBeSet()
    {
        // Arrange & Act
        var selection = new CriterionSelection
        {
            Name = "Commute Time",
            Category = CriterionCategory.Location,
            Weight = 20,
            IsLocked = true,
            ScoreAnchorLow = "> 60 minutes each way",
            ScoreAnchorHigh = "< 15 minutes each way",
            SuggestionReason = "You indicated commute is important"
        };

        // Assert
        selection.Name.Should().Be("Commute Time");
        selection.Category.Should().Be(CriterionCategory.Location);
        selection.Weight.Should().Be(20);
        selection.IsLocked.Should().BeTrue();
        selection.ScoreAnchorLow.Should().Be("> 60 minutes each way");
        selection.ScoreAnchorHigh.Should().Be("< 15 minutes each way");
        selection.SuggestionReason.Should().Be("You indicated commute is important");
    }
}

/// <summary>
/// Tests for onboarding-related enums.
/// </summary>
public class OnboardingEnumTests
{
    [Fact]
    public void BuyingSituation_HasExpectedValues()
    {
        Enum.GetValues<BuyingSituation>().Should().HaveCount(5);
    }

    [Fact]
    public void HouseholdType_HasExpectedValues()
    {
        Enum.GetValues<HouseholdType>().Should().HaveCount(5);
    }

    [Fact]
    public void PetType_HasExpectedFlagsValues()
    {
        PetType.None.Should().Be((PetType)0);
        PetType.Dogs.Should().Be((PetType)1);
        PetType.Cats.Should().Be((PetType)2);
        PetType.Other.Should().Be((PetType)4);
    }
}
