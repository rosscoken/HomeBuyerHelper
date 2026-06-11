using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;

namespace HomeBuyerHelper.ViewModels.Analysis;

/// <summary>
/// ViewModel for the rent vs. buy calculator (P4-RVB-001/002).
/// </summary>
public partial class RentVsBuyViewModel : BaseViewModel
{
    private readonly IRentVsBuyService _rentVsBuyService;
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly IPropertyService _propertyService;

    [ObservableProperty]
    private decimal _purchasePrice;

    [ObservableProperty]
    private decimal _monthlyRent;

    [ObservableProperty]
    private decimal _monthlyOwnershipCosts = 800m;

    [ObservableProperty]
    private decimal _rentIncreasePercent = 3m;

    [ObservableProperty]
    private decimal _investmentReturnPercent = 7m;

    [ObservableProperty]
    private decimal _appreciationPercent = 3m;

    [ObservableProperty]
    private int _horizonYears = 10;

    [ObservableProperty]
    private ObservableCollection<RentVsBuyYear> _years = new();

    [ObservableProperty]
    private string _verdict = string.Empty;

    [ObservableProperty]
    private bool _hasResult;

    [ObservableProperty]
    private decimal _initialOwnershipCost;

    private decimal _downPaymentPercent = 20m;
    private decimal _interestRate = 7m;
    private int _termYears = 30;

    public RentVsBuyViewModel(
        IRentVsBuyService rentVsBuyService,
        IUserPreferencesRepository preferencesRepository,
        IPropertyService propertyService)
    {
        _rentVsBuyService = rentVsBuyService;
        _preferencesRepository = preferencesRepository;
        _propertyService = propertyService;
        Title = "Rent vs. Buy";
    }

    public override async Task OnAppearingAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var preferences = await _preferencesRepository.GetAsync();
            _downPaymentPercent = preferences.DefaultDownPaymentPercent;
            _interestRate = preferences.DefaultInterestRate;
            _termYears = preferences.DefaultMortgageTerm;

            // Seed price from the top-ranked property when available.
            if (PurchasePrice <= 0)
            {
                var rankings = await _propertyService.GetPropertyRankingsAsync();
                var top = rankings.FirstOrDefault()?.Property;
                if (top != null)
                {
                    PurchasePrice = top.EffectivePrice;
                }
            }
        });
    }

    [RelayCommand]
    private void Calculate()
    {
        if (PurchasePrice <= 0 || MonthlyRent <= 0)
        {
            SetError("Enter a purchase price and your current rent.");
            return;
        }

        ClearError();

        var result = _rentVsBuyService.Compare(new RentVsBuyInput
        {
            PurchasePrice = PurchasePrice,
            DownPaymentPercent = _downPaymentPercent,
            InterestRate = _interestRate,
            MortgageTermYears = _termYears,
            MonthlyOwnershipCosts = MonthlyOwnershipCosts,
            CurrentMonthlyRent = MonthlyRent,
            AnnualRentIncreasePercent = RentIncreasePercent,
            InvestmentReturnPercent = InvestmentReturnPercent,
            HomeAppreciationPercent = AppreciationPercent,
            HorizonYears = Math.Clamp(HorizonYears, 1, 30)
        });

        Years = new ObservableCollection<RentVsBuyYear>(result.Years);
        InitialOwnershipCost = result.InitialMonthlyOwnershipCost;
        HasResult = true;

        Verdict = result.BreakevenYear is int year
            ? $"Buying pulls ahead in year {year}. After {HorizonYears} years, buying leaves you {result.FinalAdvantage:C0} ahead."
            : $"Renting stays ahead for the full {HorizonYears}-year horizon " +
              $"({Math.Abs(result.FinalAdvantage):C0} better). Buying may still win over a longer stay.";
    }
}
