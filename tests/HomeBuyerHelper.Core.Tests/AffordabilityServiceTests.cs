using FluentAssertions;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Core.Services;
using Xunit;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for AffordabilityService (P2-AFF-001).
/// </summary>
public class AffordabilityServiceTests
{
    private readonly AffordabilityService _service = new(new IncomeScenarioService());

    [Theory]
    [InlineData(2000, 10000, 20)]
    [InlineData(2800, 10000, 28)]
    [InlineData(0, 10000, 0)]
    public void HousingPercentage_CalculatesCorrectly(decimal housing, decimal income, decimal expected)
    {
        _service.CalculateHousingPercentage(housing, income).Should().Be(expected);
    }

    [Fact]
    public void HousingPercentage_ZeroIncome_ReturnsZero()
    {
        _service.CalculateHousingPercentage(2000, 0).Should().Be(0);
    }

    [Theory]
    [InlineData(27.99, AffordabilityZone.Comfortable)]
    [InlineData(28, AffordabilityZone.Stretching)]
    [InlineData(36, AffordabilityZone.Stretching)]
    [InlineData(36.01, AffordabilityZone.Aggressive)]
    [InlineData(43, AffordabilityZone.Aggressive)]
    [InlineData(43.01, AffordabilityZone.Risky)]
    [InlineData(80, AffordabilityZone.Risky)]
    public void GetZone_MapsThresholdsPerSpec(decimal percentage, AffordabilityZone expected)
    {
        _service.GetZone(percentage).Should().Be(expected);
    }

    [Fact]
    public void AssessAllScenarios_ReturnsAllThreeScenarios()
    {
        var sources = new[]
        {
            new IncomeSource
            {
                Name = "Salary",
                GrossAmount = 10_000m,
                Frequency = IncomeFrequency.Monthly,
                IsReliable = true
            },
            new IncomeSource
            {
                Name = "Bonus",
                GrossAmount = 24_000m,
                Frequency = IncomeFrequency.Annually,
                IsReliable = false,
                Probability = 50
            }
        };

        var results = _service.AssessAllScenarios(2_800m, sources);

        results.Should().HaveCount(3);

        var conservative = results.Single(r => r.Scenario == IncomeScenario.Conservative);
        var realistic = results.Single(r => r.Scenario == IncomeScenario.Realistic);
        var expected = results.Single(r => r.Scenario == IncomeScenario.Expected);

        conservative.GrossMonthlyIncome.Should().Be(10_000m);
        conservative.HousingPercentage.Should().Be(28m);
        conservative.Zone.Should().Be(AffordabilityZone.Stretching);

        // Realistic: 10,000 + (24,000/12 × 50%) = 11,000
        realistic.GrossMonthlyIncome.Should().Be(11_000m);
        realistic.Zone.Should().Be(AffordabilityZone.Comfortable);

        // Expected: 10,000 + 2,000 = 12,000
        expected.GrossMonthlyIncome.Should().Be(12_000m);
        expected.Zone.Should().Be(AffordabilityZone.Comfortable);
    }

    [Fact]
    public void AssessAllScenarios_NoIncome_MarksRisky()
    {
        var results = _service.AssessAllScenarios(2_000m, Array.Empty<IncomeSource>());

        results.Should().AllSatisfy(r => r.Zone.Should().Be(AffordabilityZone.Risky));
    }

    [Fact]
    public void ZoneColorAndLabel_AreMapped()
    {
        var assessment = new AffordabilityAssessment { Zone = AffordabilityZone.Comfortable };
        assessment.ZoneColor.Should().Be("#4CAF50");
        assessment.ZoneLabel.Should().Be("Comfortable");

        var risky = new AffordabilityAssessment { Zone = AffordabilityZone.Risky };
        risky.ZoneColor.Should().Be("#F44336");
        risky.ZoneLabel.Should().Be("Risky");
    }
}
