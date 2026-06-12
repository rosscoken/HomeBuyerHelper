using HomeBuyerHelper.Core.Interfaces;

namespace HomeBuyerHelper.Core.Services;

/// <summary>
/// Year-by-year rent vs. buy wealth simulation (P4-RVB-001).
///
/// Buy scenario: wealth = home equity (appreciated value - remaining loan)
/// minus selling costs. Cash spent on ownership is gone.
/// Rent scenario: wealth = the down payment + closing costs invested at the
/// return rate, plus any monthly savings (ownership cost - rent) invested
/// as they accrue. When rent exceeds ownership cost the renter draws down.
/// </summary>
public class RentVsBuyService : IRentVsBuyService
{
    private readonly ICalculationService _calculationService;

    public RentVsBuyService(ICalculationService calculationService)
    {
        _calculationService = calculationService;
    }

    public RentVsBuyResult Compare(RentVsBuyInput input)
    {
        if (input.PurchasePrice <= 0 || input.HorizonYears <= 0)
        {
            return new RentVsBuyResult();
        }

        var downPayment = input.PurchasePrice * input.DownPaymentPercent / 100;
        var closingCosts = input.ClosingCosts > 0 ? input.ClosingCosts : input.PurchasePrice * 0.03m;
        var loanAmount = input.PurchasePrice - downPayment;
        var monthlyPandI = _calculationService.CalculateMonthlyMortgagePayment(
            loanAmount, input.InterestRate, input.MortgageTermYears);
        var monthlyOwnership = monthlyPandI + input.MonthlyOwnershipCosts;

        var monthlyRate = input.InterestRate / 100 / 12;
        var monthlyReturn = input.InvestmentReturnPercent / 100 / 12;

        var years = new List<RentVsBuyYear>(input.HorizonYears);

        var homeValue = input.PurchasePrice;
        var remainingLoan = loanAmount;
        var rent = input.CurrentMonthlyRent;
        // Renter starts with the cash a buyer would have put down.
        var renterPortfolio = downPayment + closingCosts;

        int? breakeven = null;

        for (var year = 1; year <= input.HorizonYears; year++)
        {
            for (var month = 0; month < 12; month++)
            {
                // Buyer pays down the loan.
                if (remainingLoan > 0)
                {
                    var interest = remainingLoan * monthlyRate;
                    var principal = Math.Min(monthlyPandI - interest, remainingLoan);
                    remainingLoan -= principal;
                }

                // Renter invests the cost difference (or draws it down).
                renterPortfolio *= 1 + monthlyReturn;
                renterPortfolio += monthlyOwnership - rent;
            }

            homeValue *= 1 + input.HomeAppreciationPercent / 100;
            rent *= 1 + input.AnnualRentIncreasePercent / 100;

            var equity = homeValue - remainingLoan;
            var buyWealth = equity - homeValue * input.SellingCostPercent / 100;
            var rentWealth = Math.Max(0, renterPortfolio);

            var entry = new RentVsBuyYear
            {
                Year = year,
                BuyNetWealth = Math.Round(buyWealth, 0),
                RentNetWealth = Math.Round(rentWealth, 0),
                MonthlyRent = Math.Round(rent, 0),
                MonthlyOwnershipCost = Math.Round(monthlyOwnership, 0)
            };
            years.Add(entry);

            if (breakeven == null && entry.BuyingIsAhead)
            {
                breakeven = year;
            }
        }

        return new RentVsBuyResult
        {
            Years = years,
            BreakevenYear = breakeven,
            InitialMonthlyOwnershipCost = Math.Round(monthlyOwnership, 2),
            FinalAdvantage = years[^1].BuyNetWealth - years[^1].RentNetWealth
        };
    }
}
