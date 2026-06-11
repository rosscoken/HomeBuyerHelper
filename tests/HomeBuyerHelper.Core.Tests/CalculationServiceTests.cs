using Xunit;
using FluentAssertions;
using HomeBuyerHelper.Core.Services;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for the CalculationService.
/// </summary>
public class CalculationServiceTests
{
    private readonly CalculationService _sut;

    public CalculationServiceTests()
    {
        _sut = new CalculationService();
    }

    [Fact]
    public void CalculateMonthlyMortgagePayment_WithValidInputs_ReturnsCorrectPayment()
    {
        // Arrange
        var principal = 400_000m;
        var annualRate = 7.0m;
        var termYears = 30;

        // Act
        var result = _sut.CalculateMonthlyMortgagePayment(principal, annualRate, termYears);

        // Assert
        result.Should().BeApproximately(2661.21m, 1m); // Standard mortgage calculator result
    }

    [Fact]
    public void CalculateMonthlyMortgagePayment_WithZeroPrincipal_ReturnsZero()
    {
        // Arrange & Act
        var result = _sut.CalculateMonthlyMortgagePayment(0, 7.0m, 30);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void CalculateMonthlyMortgagePayment_WithZeroInterest_ReturnsPrincipalDividedByMonths()
    {
        // Arrange
        var principal = 360_000m;
        var termYears = 30;
        var expectedMonthlyPayment = principal / (termYears * 12m);

        // Act
        var result = _sut.CalculateMonthlyMortgagePayment(principal, 0, termYears);

        // Assert
        result.Should().Be(expectedMonthlyPayment);
    }

    [Fact]
    public void CalculateDownPayment_ReturnsCorrectAmount()
    {
        // Arrange
        var purchasePrice = 500_000m;
        var downPaymentPercent = 20m;

        // Act
        var result = _sut.CalculateDownPayment(purchasePrice, downPaymentPercent);

        // Assert
        result.Should().Be(100_000m);
    }

    [Fact]
    public void CalculateDTI_WithValidInputs_ReturnsCorrectPercentage()
    {
        // Arrange
        var monthlyDebt = 2000m;
        var monthlyIncome = 8000m;

        // Act
        var result = _sut.CalculateDTI(monthlyDebt, monthlyIncome);

        // Assert
        result.Should().Be(25m);
    }

    [Fact]
    public void CalculateDTI_WithZeroIncome_ReturnsZero()
    {
        // Arrange & Act
        var result = _sut.CalculateDTI(2000m, 0);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void CalculateMonthlyHousingCost_ReturnsCompleteBreakdown()
    {
        // Arrange
        var purchasePrice = 400_000m;
        var downPaymentPercent = 20m;
        var annualInterestRate = 7.0m;
        var termYears = 30;
        var annualPropertyTax = 6_000m;
        var annualInsurance = 2_400m;
        var monthlyHOA = 200m;

        // Act
        var result = _sut.CalculateMonthlyHousingCost(
            purchasePrice,
            downPaymentPercent,
            annualInterestRate,
            termYears,
            annualPropertyTax,
            annualInsurance,
            monthlyHOA);

        // Assert
        result.PropertyTax.Should().Be(500m);
        result.Insurance.Should().Be(200m);
        result.HOA.Should().Be(200m);
        result.PMI.Should().Be(0); // 20% down, no PMI
        result.Total.Should().BeGreaterThan(0);
    }

    [Fact]
    public void CalculateMonthlyHousingCost_WithLowDownPayment_IncludesPMI()
    {
        // Arrange
        var purchasePrice = 400_000m;
        var downPaymentPercent = 10m; // Less than 20%

        // Act
        var result = _sut.CalculateMonthlyHousingCost(
            purchasePrice,
            downPaymentPercent,
            7.0m,
            30,
            6_000m,
            2_400m,
            0);

        // Assert
        result.PMI.Should().BeGreaterThan(0);
    }

    [Fact]
    public void CalculateClosingCosts_ReturnsEstimatesWithinRange()
    {
        // Arrange
        var purchasePrice = 400_000m;

        // Act
        var result = _sut.CalculateClosingCosts(purchasePrice);

        // Assert
        result.Total.Should().BeGreaterThan(0);
        result.LowEstimate.Should().Be(purchasePrice * 0.02m);
        result.HighEstimate.Should().Be(purchasePrice * 0.05m);
        result.Total.Should().BeInRange(result.LowEstimate, result.HighEstimate);
    }

    [Fact]
    public void CalculateAffordability_WithHighDebts_ReturnsLowMaxPrice()
    {
        // Arrange
        var monthlyIncome = 10_000m;
        var monthlyDebts = 4_000m; // High existing debt
        var downPayment = 80_000m;
        var rate = 7.0m;
        var term = 30;

        // Act
        var result = _sut.CalculateAffordability(
            monthlyIncome,
            monthlyDebts,
            downPayment,
            rate,
            term);

        // Assert
        result.MaxPurchasePrice.Should().BeGreaterThan(0);
        result.DTIWithMaxPrice.Should().BeLessThanOrEqualTo(43);
    }

    [Fact]
    public void CalculateAmortizationSchedule_ReturnsCorrectNumberOfPayments()
    {
        // Arrange
        var principal = 300_000m;
        var rate = 7.0m;
        var termYears = 30;

        // Act
        var result = _sut.CalculateAmortizationSchedule(principal, rate, termYears);

        // Assert
        result.Should().HaveCount(termYears * 12);
        result.First().PaymentNumber.Should().Be(1);
        result.Last().PaymentNumber.Should().Be(360);
        result.Last().RemainingBalance.Should().Be(0);
    }
}
