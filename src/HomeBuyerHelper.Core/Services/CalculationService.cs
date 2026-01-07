using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Services;

/// <summary>
/// Financial calculation service.
/// </summary>
public class CalculationService : ICalculationService
{
    public decimal CalculateMonthlyMortgagePayment(decimal principal, decimal annualInterestRate, int termYears)
    {
        if (principal <= 0 || termYears <= 0)
            return 0;

        if (annualInterestRate <= 0)
            return principal / (termYears * 12m);

        var monthlyRate = annualInterestRate / 100 / 12;
        var numberOfPayments = termYears * 12;

        // M = P * [r(1+r)^n] / [(1+r)^n - 1]
        var compoundFactor = (decimal)Math.Pow((double)(1 + monthlyRate), numberOfPayments);
        var payment = principal * (monthlyRate * compoundFactor) / (compoundFactor - 1);

        return Math.Round(payment, 2);
    }

    public MonthlyCostBreakdown CalculateMonthlyHousingCost(
        decimal purchasePrice,
        decimal downPaymentPercent,
        decimal annualInterestRate,
        int termYears,
        decimal annualPropertyTax,
        decimal annualInsurance,
        decimal monthlyHOA)
    {
        var downPayment = CalculateDownPayment(purchasePrice, downPaymentPercent);
        var loanAmount = purchasePrice - downPayment;
        var monthlyPayment = CalculateMonthlyMortgagePayment(loanAmount, annualInterestRate, termYears);

        // For first month, calculate P&I split
        var monthlyRate = annualInterestRate / 100 / 12;
        var firstMonthInterest = loanAmount * monthlyRate;
        var firstMonthPrincipal = monthlyPayment - firstMonthInterest;

        // PMI typically required if down payment < 20%
        var pmi = downPaymentPercent < 20 ? loanAmount * 0.005m / 12 : 0;

        return new MonthlyCostBreakdown
        {
            Principal = Math.Round(firstMonthPrincipal, 2),
            Interest = Math.Round(firstMonthInterest, 2),
            PropertyTax = Math.Round(annualPropertyTax / 12, 2),
            Insurance = Math.Round(annualInsurance / 12, 2),
            HOA = monthlyHOA,
            PMI = Math.Round(pmi, 2)
        };
    }

    public decimal CalculateDownPayment(decimal purchasePrice, decimal downPaymentPercent)
    {
        return Math.Round(purchasePrice * downPaymentPercent / 100, 2);
    }

    public ClosingCostEstimate CalculateClosingCosts(decimal purchasePrice, string? state = null)
    {
        // Standard closing cost estimates (2-5% of purchase price)
        var loanOriginationFee = purchasePrice * 0.01m; // 1% of loan
        var appraisalFee = 500m;
        var titleInsurance = purchasePrice * 0.005m; // 0.5% of purchase price
        var titleSearch = 300m;
        var escrowFees = 500m;
        var recordingFees = 150m;
        var prepaidInterest = purchasePrice * 0.002m; // ~15 days of interest
        var homeInspection = 500m;
        var otherFees = 500m;

        return new ClosingCostEstimate
        {
            LoanOriginationFee = Math.Round(loanOriginationFee, 2),
            AppraisalFee = appraisalFee,
            TitleInsurance = Math.Round(titleInsurance, 2),
            TitleSearch = titleSearch,
            EscrowFees = escrowFees,
            RecordingFees = recordingFees,
            PrepaidInterest = Math.Round(prepaidInterest, 2),
            HomeInspection = homeInspection,
            OtherFees = otherFees,
            LowEstimate = Math.Round(purchasePrice * 0.02m, 2),
            HighEstimate = Math.Round(purchasePrice * 0.05m, 2)
        };
    }

    public decimal CalculateDTI(decimal monthlyDebt, decimal monthlyIncome)
    {
        if (monthlyIncome <= 0) return 0;
        return Math.Round(monthlyDebt / monthlyIncome * 100, 2);
    }

    public AffordabilityResult CalculateAffordability(
        decimal monthlyIncome,
        decimal monthlyDebts,
        decimal downPaymentAmount,
        decimal annualInterestRate,
        int termYears,
        decimal maxDTI = 43m)
    {
        // Maximum housing payment = (maxDTI% of income) - existing debts
        var maxTotalDebt = monthlyIncome * maxDTI / 100;
        var maxHousingPayment = maxTotalDebt - monthlyDebts;

        if (maxHousingPayment <= 0)
        {
            return new AffordabilityResult
            {
                MaxPurchasePrice = 0,
                MaxMonthlyPayment = 0,
                RequiredDownPayment = downPaymentAmount,
                EstimatedMonthlyPayment = 0,
                DTIWithMaxPrice = maxDTI,
                MeetsConventionalGuidelines = false
            };
        }

        // Back-calculate max loan from max payment
        // P = M * [(1+r)^n - 1] / [r(1+r)^n]
        var monthlyRate = annualInterestRate / 100 / 12;
        var numberOfPayments = termYears * 12;

        decimal maxLoan;
        if (monthlyRate <= 0)
        {
            maxLoan = maxHousingPayment * numberOfPayments;
        }
        else
        {
            var compoundFactor = (decimal)Math.Pow((double)(1 + monthlyRate), numberOfPayments);
            maxLoan = maxHousingPayment * (compoundFactor - 1) / (monthlyRate * compoundFactor);
        }

        // Add down payment to get max purchase price
        var maxPurchasePrice = maxLoan + downPaymentAmount;

        // Calculate estimated payment at max price
        var estimatedPayment = CalculateMonthlyMortgagePayment(maxLoan, annualInterestRate, termYears);
        var dtiAtMax = CalculateDTI(monthlyDebts + estimatedPayment, monthlyIncome);

        return new AffordabilityResult
        {
            MaxPurchasePrice = Math.Round(maxPurchasePrice, 2),
            MaxMonthlyPayment = Math.Round(maxHousingPayment, 2),
            RequiredDownPayment = downPaymentAmount,
            EstimatedMonthlyPayment = Math.Round(estimatedPayment, 2),
            DTIWithMaxPrice = dtiAtMax,
            MeetsConventionalGuidelines = dtiAtMax <= 43
        };
    }

    public Task<IReadOnlyList<BudgetMonth>> GenerateBudgetProjectionsAsync(
        int propertyId,
        int monthsToProject = 12)
    {
        // This would typically fetch income/expense data and generate projections
        // For now, return empty list - will be implemented in Phase 2
        return Task.FromResult<IReadOnlyList<BudgetMonth>>(new List<BudgetMonth>());
    }

    public IReadOnlyList<AmortizationEntry> CalculateAmortizationSchedule(
        decimal principal,
        decimal annualInterestRate,
        int termYears)
    {
        var schedule = new List<AmortizationEntry>();
        var monthlyPayment = CalculateMonthlyMortgagePayment(principal, annualInterestRate, termYears);
        var monthlyRate = annualInterestRate / 100 / 12;
        var remainingBalance = principal;
        var totalInterestPaid = 0m;
        var totalPrincipalPaid = 0m;
        var startDate = DateTime.Today.AddMonths(1);

        for (var i = 1; i <= termYears * 12; i++)
        {
            var interestPayment = remainingBalance * monthlyRate;
            var principalPayment = monthlyPayment - interestPayment;

            // Handle final payment rounding
            if (i == termYears * 12)
            {
                principalPayment = remainingBalance;
            }

            remainingBalance -= principalPayment;
            totalInterestPaid += interestPayment;
            totalPrincipalPaid += principalPayment;

            schedule.Add(new AmortizationEntry
            {
                PaymentNumber = i,
                PaymentDate = startDate.AddMonths(i - 1),
                Payment = Math.Round(monthlyPayment, 2),
                Principal = Math.Round(principalPayment, 2),
                Interest = Math.Round(interestPayment, 2),
                RemainingBalance = Math.Max(0, Math.Round(remainingBalance, 2)),
                TotalInterestPaid = Math.Round(totalInterestPaid, 2),
                TotalPrincipalPaid = Math.Round(totalPrincipalPaid, 2)
            });
        }

        return schedule;
    }
}
