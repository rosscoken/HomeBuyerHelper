using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Funding;

/// <summary>
/// ViewModel for the funding plan summary (P3-FUN-001/010, P3-TAX-002/003).
/// </summary>
public partial class FundingSetupViewModel : BaseViewModel
{
    private readonly IFundingRepository _fundingRepository;
    private readonly ITaxImpactService _taxImpactService;
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly IPropertyService _propertyService;
    private readonly ICalculationService _calculationService;
    private readonly IOneTimeEventRepository _eventRepository;

    [ObservableProperty]
    private ObservableCollection<FundingSourceTaxResult> _sources = new();

    [ObservableProperty]
    private decimal _totalGross;

    [ObservableProperty]
    private decimal _totalTax;

    [ObservableProperty]
    private decimal _totalNet;

    [ObservableProperty]
    private decimal _targetAmount;

    [ObservableProperty]
    private decimal _surplus;

    [ObservableProperty]
    private bool _hasTarget;

    [ObservableProperty]
    private string? _targetPropertyName;

    [ObservableProperty]
    private bool _hasTaxLiability;

    public FundingSetupViewModel(
        IFundingRepository fundingRepository,
        ITaxImpactService taxImpactService,
        IUserPreferencesRepository preferencesRepository,
        IPropertyService propertyService,
        ICalculationService calculationService,
        IOneTimeEventRepository eventRepository)
    {
        _fundingRepository = fundingRepository;
        _taxImpactService = taxImpactService;
        _preferencesRepository = preferencesRepository;
        _propertyService = propertyService;
        _calculationService = calculationService;
        _eventRepository = eventRepository;
        Title = "Funding Plan";
    }

    public override async Task OnAppearingAsync()
    {
        await LoadDataAsync();
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var sources = await _fundingRepository.GetAllAsync();
            var preferences = await _preferencesRepository.GetAsync();

            var plan = _taxImpactService.BuildPlan(sources, preferences);
            Sources = new ObservableCollection<FundingSourceTaxResult>(plan.Sources);
            TotalGross = plan.TotalGross;
            TotalTax = plan.TotalTax;
            TotalNet = plan.TotalNet;
            HasTaxLiability = plan.TotalTax > 0;

            // Compare against the top-ranked property's cash-to-close.
            var rankings = await _propertyService.GetPropertyRankingsAsync();
            var topProperty = rankings.FirstOrDefault()?.Property;
            if (topProperty != null && topProperty.EffectivePrice > 0)
            {
                var downPayment = _calculationService.CalculateDownPayment(
                    topProperty.EffectivePrice, preferences.DefaultDownPaymentPercent);
                var closing = _calculationService.CalculateClosingCosts(
                    topProperty.EffectivePrice, topProperty.State);

                TargetAmount = downPayment + closing.Total;
                TargetPropertyName = topProperty.Nickname;
                Surplus = TotalNet - TargetAmount;
                HasTarget = true;
            }
            else
            {
                HasTarget = false;
            }
        });
    }

    [RelayCommand]
    private async Task AddNewAsync()
    {
        await Shell.Current.GoToAsync("FundingEdit?id=0");
    }

    [RelayCommand]
    private async Task EditSourceAsync(FundingSourceTaxResult result)
    {
        await Shell.Current.GoToAsync($"FundingEdit?id={result.Source.Id}");
    }

    [RelayCommand]
    private async Task DeleteSourceAsync(FundingSourceTaxResult result)
    {
        var confirmed = await Shell.Current.DisplayAlert(
            "Delete Funding Source",
            $"Delete '{result.Source.Name}'?",
            "Delete",
            "Cancel");

        if (!confirmed) return;

        await ExecuteBusyAsync(async () =>
        {
            await _fundingRepository.DeleteAsync(result.Source.Id);
        });
        await LoadDataAsync();
    }

    [RelayCommand]
    private async Task OpenTaxSettingsAsync()
    {
        await Shell.Current.GoToAsync("TaxSettings");
    }

    /// <summary>
    /// Adds the estimated tax bill to the budget as a one-time event the
    /// following April (P3-TAX-003).
    /// </summary>
    [RelayCommand]
    private async Task AddTaxSetAsideToBudgetAsync()
    {
        if (TotalTax <= 0) return;

        var taxDue = new DateTime(DateTime.Today.Year + 1, 4, 15);
        var confirmed = await Shell.Current.DisplayAlert(
            "Add to Budget",
            $"Add a one-time event of {TotalTax:C0} in {taxDue:MMMM yyyy} for taxes on your funding plan?\n\n" +
            "Tip: set this amount aside in a high-yield savings account until tax time.",
            "Add",
            "Cancel");

        if (!confirmed) return;

        await ExecuteBusyAsync(async () =>
        {
            await _eventRepository.CreateAsync(new OneTimeEvent
            {
                Name = "Down payment funding taxes",
                Amount = TotalTax,
                Date = taxDue,
                Category = OneTimeEventCategory.Taxes,
                Notes = "Estimated taxes from down payment funding sources (capital gains, IRA distributions)."
            });

            await Shell.Current.DisplayAlert(
                "Added",
                "The tax set-aside now appears in your cash flow projection.",
                "OK");
        });
    }
}
