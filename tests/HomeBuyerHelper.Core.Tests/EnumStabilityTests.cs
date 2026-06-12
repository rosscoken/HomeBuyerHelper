using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using Xunit;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Pins the integer values of enums that are persisted to SQLite, JSON
/// backups, and browser localStorage. Reordering or inserting enum members
/// silently corrupts every existing database and backup — these tests make
/// that a red build instead. Add new members at the END of each enum.
/// </summary>
public class EnumStabilityTests
{
    [Theory]
    [InlineData(IncomeType.Employment, 0)]
    [InlineData(IncomeType.Other, 9)]
    [InlineData(IncomeType.Bonus, 10)]
    [InlineData(IncomeType.RSU, 11)]
    [InlineData(IncomeType.PartnerContribution, 12)]
    public void IncomeType_ValuesArePinned(IncomeType value, int expected) =>
        Assert.Equal(expected, (int)value);

    [Theory]
    [InlineData(FundingType.Savings, 0)]
    [InlineData(FundingType.Investment, 2)]
    [InlineData(FundingType.Retirement401k, 3)]
    [InlineData(FundingType.RetirementIRA, 4)]
    [InlineData(FundingType.Gift, 5)]
    [InlineData(FundingType.Other, 10)]
    [InlineData(FundingType.RothIRA, 11)]
    [InlineData(FundingType.InheritedIRA, 12)]
    public void FundingType_ValuesArePinned(FundingType value, int expected) =>
        Assert.Equal(expected, (int)value);

    [Theory]
    [InlineData(IncomeFrequency.Weekly, 0)]
    [InlineData(IncomeFrequency.Annually, 5)]
    public void IncomeFrequency_ValuesArePinned(IncomeFrequency value, int expected) =>
        Assert.Equal(expected, (int)value);

    [Theory]
    [InlineData(ExpenseCategory.Housing, 0)]
    [InlineData(ExpenseCategory.Other, 15)]
    public void ExpenseCategory_ValuesArePinned(ExpenseCategory value, int expected) =>
        Assert.Equal(expected, (int)value);

    [Theory]
    [InlineData(PropertyType.SingleFamily, 0)]
    [InlineData(PropertyType.Other, 6)]
    public void PropertyType_ValuesArePinned(PropertyType value, int expected) =>
        Assert.Equal(expected, (int)value);

    [Theory]
    [InlineData(CriterionCategory.Location, 0)]
    [InlineData(CriterionCategory.Other, 6)]
    public void CriterionCategory_ValuesArePinned(CriterionCategory value, int expected) =>
        Assert.Equal(expected, (int)value);

    [Theory]
    [InlineData(TaxFilingStatus.Single, 0)]
    [InlineData(TaxFilingStatus.HeadOfHousehold, 3)]
    public void TaxFilingStatus_ValuesArePinned(TaxFilingStatus value, int expected) =>
        Assert.Equal(expected, (int)value);

    [Theory]
    [InlineData(OneTimeEventCategory.Moving, 0)]
    [InlineData(OneTimeEventCategory.Taxes, 5)]
    [InlineData(OneTimeEventCategory.Other, 6)]
    public void OneTimeEventCategory_ValuesArePinned(OneTimeEventCategory value, int expected) =>
        Assert.Equal(expected, (int)value);
}
