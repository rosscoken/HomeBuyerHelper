using FluentAssertions;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Core.Services;
using Xunit;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for TaxImpactService (P3-TAX-001, P3-FUN-003 through 009).
/// </summary>
public class TaxImpactServiceTests
{
    private readonly TaxImpactService _service = new();

    /// <summary>22% federal bracket, no state tax.</summary>
    private static UserPreferences Preferences22Percent => new()
    {
        FilingStatus = TaxFilingStatus.Single,
        EstimatedTaxableIncome = 100_000m,
        StateMarginalTaxRate = 0m
    };

    [Theory]
    [InlineData(TaxFilingStatus.Single, 50_000, 22)]
    [InlineData(TaxFilingStatus.Single, 11_000, 10)]
    [InlineData(TaxFilingStatus.Single, 700_000, 37)]
    [InlineData(TaxFilingStatus.MarriedFilingJointly, 100_000, 22)]
    [InlineData(TaxFilingStatus.MarriedFilingJointly, 90_000, 12)]
    [InlineData(TaxFilingStatus.HeadOfHousehold, 60_000, 12)]
    public void MarginalFederalRate_MatchesBrackets(TaxFilingStatus status, decimal income, decimal expected)
    {
        _service.GetMarginalFederalRate(status, income).Should().Be(expected);
    }

    [Theory]
    [InlineData(TaxFilingStatus.Single, 40_000, 0)]
    [InlineData(TaxFilingStatus.Single, 100_000, 15)]
    [InlineData(TaxFilingStatus.Single, 600_000, 20)]
    [InlineData(TaxFilingStatus.MarriedFilingJointly, 90_000, 0)]
    public void LongTermCapitalGainsRate_MatchesBrackets(TaxFilingStatus status, decimal income, decimal expected)
    {
        _service.GetLongTermCapitalGainsRate(status, income).Should().Be(expected);
    }

    [Fact]
    public void Savings_HasNoTax()
    {
        var source = new FundingSource
        {
            Name = "Chase Savings",
            CurrentAmount = 25_000m,
            FundingType = FundingType.Savings
        };

        var result = _service.CalculateTax(source, Preferences22Percent);

        result.TotalTax.Should().Be(0);
        result.NetAmount.Should().Be(25_000m);
    }

    [Fact]
    public void Gift_HasNoTaxToRecipient()
    {
        var source = new FundingSource
        {
            Name = "Gift from Parents",
            CurrentAmount = 50_000m,
            FundingType = FundingType.Gift,
            DonorName = "Mom and Dad"
        };

        var result = _service.CalculateTax(source, Preferences22Percent);

        result.TotalTax.Should().Be(0);
        result.Explanation.Should().Contain("gift letter");
    }

    [Fact]
    public void Brokerage_LongTerm_UsesCapitalGainsRate()
    {
        var source = new FundingSource
        {
            Name = "Fidelity MSFT",
            CurrentAmount = 110_000m,
            FundingType = FundingType.Investment,
            CostBasis = 55_000m,
            IsLongTermHolding = true
        };

        // 100k income → 15% LTCG. Gain = 55,000 → tax = 8,250.
        var result = _service.CalculateTax(source, Preferences22Percent);

        result.CapitalGainsTax.Should().Be(8_250m);
        result.NetAmount.Should().Be(101_750m);
    }

    [Fact]
    public void Brokerage_ShortTerm_UsesOrdinaryRate()
    {
        var source = new FundingSource
        {
            Name = "Recent Stock",
            CurrentAmount = 20_000m,
            FundingType = FundingType.Investment,
            CostBasis = 10_000m,
            IsLongTermHolding = false
        };

        // Gain = 10,000 at 22% ordinary = 2,200.
        var result = _service.CalculateTax(source, Preferences22Percent);

        result.CapitalGainsTax.Should().Be(2_200m);
    }

    [Fact]
    public void Brokerage_Loss_HasNoTax()
    {
        var source = new FundingSource
        {
            Name = "Underwater Stock",
            CurrentAmount = 10_000m,
            FundingType = FundingType.Investment,
            CostBasis = 15_000m,
            IsLongTermHolding = true
        };

        _service.CalculateTax(source, Preferences22Percent).TotalTax.Should().Be(0);
    }

    [Fact]
    public void TraditionalIra_Under59Half_FirstTimeBuyer_PenaltyOnlyAboveExemption()
    {
        var source = new FundingSource
        {
            Name = "Traditional IRA",
            CurrentAmount = 30_000m,
            FundingType = FundingType.RetirementIRA,
            OwnerAge = 40,
            IsFirstTimeBuyer = true
        };

        var result = _service.CalculateTax(source, Preferences22Percent);

        // Income tax on full 30k at 22% = 6,600.
        result.IncomeTax.Should().Be(6_600m);
        // Penalty only on (30,000 - 10,000) × 10% = 2,000.
        result.Penalty.Should().Be(2_000m);
    }

    [Fact]
    public void TraditionalIra_Over59Half_NoPenalty()
    {
        var source = new FundingSource
        {
            Name = "Traditional IRA",
            CurrentAmount = 30_000m,
            FundingType = FundingType.RetirementIRA,
            OwnerAge = 62,
            IsFirstTimeBuyer = false
        };

        _service.CalculateTax(source, Preferences22Percent).Penalty.Should().Be(0);
    }

    [Fact]
    public void InheritedIra_TaxedAsIncomeWithoutPenalty()
    {
        var source = new FundingSource
        {
            Name = "Inherited IRA",
            CurrentAmount = 50_000m,
            FundingType = FundingType.InheritedIRA,
            OwnerAge = 35
        };

        var result = _service.CalculateTax(source, Preferences22Percent);

        result.IncomeTax.Should().Be(11_000m); // 22% of 50k
        result.Penalty.Should().Be(0);
        result.Explanation.Should().Contain("10 years");
    }

    [Fact]
    public void RothIra_ContributionsAreAlwaysFree()
    {
        var source = new FundingSource
        {
            Name = "Roth IRA",
            CurrentAmount = 20_000m,
            FundingType = FundingType.RothIRA,
            RothContributionPortion = 20_000m, // all contributions
            IsRothAccount5Years = false,
            OwnerAge = 35
        };

        _service.CalculateTax(source, Preferences22Percent).TotalTax.Should().Be(0);
    }

    [Fact]
    public void RothIra_Earnings_QualifiedFirstTimeBuyer_FreeUpTo10k()
    {
        var source = new FundingSource
        {
            Name = "Roth IRA",
            CurrentAmount = 28_000m,
            FundingType = FundingType.RothIRA,
            RothContributionPortion = 20_000m, // 8k of earnings
            IsRothAccount5Years = true,
            IsFirstTimeBuyer = true,
            OwnerAge = 35
        };

        // 8k earnings fully covered by the 10k first-time buyer exemption.
        _service.CalculateTax(source, Preferences22Percent).TotalTax.Should().Be(0);
    }

    [Fact]
    public void RothIra_Earnings_NotQualified_TaxedAndPenalized()
    {
        var source = new FundingSource
        {
            Name = "Young Roth",
            CurrentAmount = 15_000m,
            FundingType = FundingType.RothIRA,
            RothContributionPortion = 10_000m, // 5k earnings
            IsRothAccount5Years = false,        // fails 5-year rule
            IsFirstTimeBuyer = true,
            OwnerAge = 35
        };

        var result = _service.CalculateTax(source, Preferences22Percent);

        result.IncomeTax.Should().Be(1_100m); // 22% of 5k
        result.Penalty.Should().Be(500m);     // 10% of 5k
    }

    [Fact]
    public void FourOhOneK_Loan_HasNoTax()
    {
        var source = new FundingSource
        {
            Name = "401k Loan",
            CurrentAmount = 50_000m,
            FundingType = FundingType.Retirement401k,
            Is401kLoan = true
        };

        _service.CalculateTax(source, Preferences22Percent).TotalTax.Should().Be(0);
    }

    [Fact]
    public void FourOhOneK_HardshipWithdrawal_TaxedAndPenalized()
    {
        var source = new FundingSource
        {
            Name = "401k Withdrawal",
            CurrentAmount = 50_000m,
            FundingType = FundingType.Retirement401k,
            Is401kLoan = false,
            OwnerAge = 40
        };

        var result = _service.CalculateTax(source, Preferences22Percent);

        result.IncomeTax.Should().Be(11_000m); // 22%
        result.Penalty.Should().Be(5_000m);    // 10%
    }

    [Fact]
    public void StateTax_AddsToFederalRates()
    {
        var preferences = new UserPreferences
        {
            FilingStatus = TaxFilingStatus.Single,
            EstimatedTaxableIncome = 100_000m,
            StateMarginalTaxRate = 5m
        };
        var source = new FundingSource
        {
            Name = "Inherited IRA",
            CurrentAmount = 10_000m,
            FundingType = FundingType.InheritedIRA
        };

        // (22 + 5)% of 10,000 = 2,700.
        _service.CalculateTax(source, preferences).IncomeTax.Should().Be(2_700m);
    }

    [Fact]
    public void BuildPlan_MatchesSpecExampleTotals()
    {
        // Design spec section 4.4.4 example.
        var sources = new[]
        {
            new FundingSource
            {
                Name = "Chase Savings",
                CurrentAmount = 25_000m,
                FundingType = FundingType.Savings
            },
            new FundingSource
            {
                Name = "Fidelity (MSFT shares)",
                CurrentAmount = 110_000m,
                FundingType = FundingType.Investment,
                CostBasis = 55_000m,
                IsLongTermHolding = true
            },
            new FundingSource
            {
                Name = "Gift from Parents",
                CurrentAmount = 50_000m,
                FundingType = FundingType.Gift
            }
        };

        var plan = _service.BuildPlan(sources, Preferences22Percent);

        plan.TotalGross.Should().Be(185_000m);
        plan.TotalTax.Should().Be(8_250m);
        plan.TotalNet.Should().Be(176_750m);

        // Optimal order: tax-free sources first, taxed brokerage last.
        plan.OptimalOrder[^1].Source.Name.Should().Be("Fidelity (MSFT shares)");
    }
}
