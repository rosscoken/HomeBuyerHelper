using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels;

/// <summary>
/// View model for the property detail/edit page.
/// </summary>
[QueryProperty(nameof(PropertyId), "id")]
public partial class PropertyDetailViewModel : BaseViewModel
{
    private readonly IPropertyService _propertyService;
    private readonly ICalculationService _calculationService;
    private readonly IUserPreferencesRepository _preferencesRepository;
    private UserPreferences? _preferences;

    [ObservableProperty]
    private int? _propertyId;

    [ObservableProperty]
    private bool _isNewProperty;

    [ObservableProperty]
    private string _nickname = string.Empty;

    [ObservableProperty]
    private string? _address;

    [ObservableProperty]
    private string? _city;

    [ObservableProperty]
    private string? _state;

    [ObservableProperty]
    private string? _zipCode;

    [ObservableProperty]
    private decimal _askingPrice;

    [ObservableProperty]
    private decimal? _offerPrice;

    [ObservableProperty]
    private int _bedrooms;

    [ObservableProperty]
    private decimal _bathrooms;

    [ObservableProperty]
    private int _squareFeet;

    [ObservableProperty]
    private int? _yearBuilt;

    [ObservableProperty]
    private PropertyType _propertyType = PropertyType.SingleFamily;

    [ObservableProperty]
    private decimal _monthlyHOA;

    [ObservableProperty]
    private decimal? _annualPropertyTax;

    [ObservableProperty]
    private decimal? _annualInsurance;

    [ObservableProperty]
    private string? _listingUrl;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private MonthlyCostBreakdown? _costBreakdown;

    [ObservableProperty]
    private decimal _overallScore;

    [ObservableProperty]
    private int _rank;

    [ObservableProperty]
    private int _scoredCriteriaCount;

    [ObservableProperty]
    private int _totalCriteriaCount;

    [ObservableProperty]
    private bool _isFullyScored;

    [ObservableProperty]
    private readonly IReadOnlyList<PropertyScore> _scores = [];

    public IReadOnlyList<PropertyType> PropertyTypes { get; } = Enum.GetValues<PropertyType>();

    public PropertyDetailViewModel(
        IPropertyService propertyService,
        ICalculationService calculationService,
        IUserPreferencesRepository preferencesRepository)
    {
        _propertyService = propertyService;
        _calculationService = calculationService;
        _preferencesRepository = preferencesRepository;
    }

    public override async Task OnAppearingAsync()
    {
        _preferences = await _preferencesRepository.GetAsync();
        UpdateCostBreakdown();
    }

    partial void OnPropertyIdChanged(int? value)
    {
        if (value.HasValue && value.Value > 0)
        {
            IsNewProperty = false;
            Title = "Edit Property";
            LoadPropertyAndHandleErrors(value.Value);
        }
        else
        {
            IsNewProperty = true;
            Title = "Add Property";
        }
    }

    private async void LoadPropertyAndHandleErrors(int id)
    {
        try
        {
            await LoadPropertyAsync(id);
        }
        catch (Exception ex)
        {
            SetError($"Failed to load property: {ex.Message}");
        }
    }

    private async Task LoadPropertyAsync(int id)
    {
        await ExecuteBusyAsync(async () =>
        {
            var property = await _propertyService.GetPropertyDetailAsync(id);
            if (property != null)
            {
                Nickname = property.Nickname;
                Address = property.Address;
                City = property.City;
                State = property.State;
                ZipCode = property.ZipCode;
                AskingPrice = property.AskingPrice;
                OfferPrice = property.OfferPrice;
                Bedrooms = property.Bedrooms;
                Bathrooms = property.Bathrooms;
                SquareFeet = property.SquareFeet;
                YearBuilt = property.YearBuilt;
                PropertyType = property.PropertyType;
                MonthlyHOA = property.MonthlyHOA;
                AnnualPropertyTax = property.AnnualPropertyTax;
                AnnualInsurance = property.AnnualInsurance;
                ListingUrl = property.ListingUrl;
                Notes = property.Notes;
                OverallScore = property.OverallScore;
                Rank = property.Rank;
                ScoredCriteriaCount = property.ScoredCriteriaCount;
                TotalCriteriaCount = property.TotalCriteriaCount;
                IsFullyScored = property.IsFullyScored;
                Scores = property.Scores;

                UpdateCostBreakdown();
            }
        });
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Nickname))
        {
            SetError("Please enter a nickname for this property.");
            return;
        }

        await ExecuteBusyAsync(async () =>
        {
            var property = new Property
            {
                Id = PropertyId ?? 0,
                Nickname = Nickname,
                Address = Address,
                City = City,
                State = State,
                ZipCode = ZipCode,
                AskingPrice = AskingPrice,
                OfferPrice = OfferPrice,
                Bedrooms = Bedrooms,
                Bathrooms = Bathrooms,
                SquareFeet = SquareFeet,
                YearBuilt = YearBuilt,
                PropertyType = PropertyType,
                MonthlyHOA = MonthlyHOA,
                AnnualPropertyTax = AnnualPropertyTax,
                AnnualInsurance = AnnualInsurance,
                ListingUrl = ListingUrl,
                Notes = Notes
            };

            if (IsNewProperty)
            {
                await _propertyService.CreatePropertyAsync(property);
            }
            else
            {
                await _propertyService.UpdatePropertyAsync(property);
            }

            await Shell.Current.GoToAsync("..");
        });
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (IsNewProperty || !PropertyId.HasValue) return;

        var confirmed = await Shell.Current.DisplayAlert(
            "Delete Property",
            $"Are you sure you want to delete '{Nickname}'? This action cannot be undone.",
            "Delete",
            "Cancel");

        if (confirmed)
        {
            await _propertyService.DeletePropertyAsync(PropertyId.Value);
            await Shell.Current.GoToAsync("..");
        }
    }

    [RelayCommand]
    private async Task ScorePropertyAsync()
    {
        if (!PropertyId.HasValue) return;
        await Shell.Current.GoToAsync($"ScoringWalkthrough?propertyId={PropertyId.Value}");
    }

    [RelayCommand]
    private void CalculateCosts()
    {
        UpdateCostBreakdown();
    }

    [RelayCommand]
    private async Task OpenLoanSettingsAsync()
    {
        await Shell.Current.GoToAsync("LoanSettings");
    }

    private void UpdateCostBreakdown()
    {
        if (AskingPrice > 0)
        {
            var downPayment = _preferences?.DefaultDownPaymentPercent ?? 20m;
            var interestRate = _preferences?.DefaultInterestRate ?? 7.0m;
            var term = _preferences?.DefaultMortgageTerm ?? 30;
            var taxRate = _preferences?.DefaultPropertyTaxRate ?? 0.96m;
            var defaultInsurance = _preferences?.DefaultMonthlyInsurance ?? 125m;

            var effectivePrice = OfferPrice ?? AskingPrice;
            var annualTax = AnnualPropertyTax ?? (effectivePrice * taxRate / 100);
            var annualInsurance = AnnualInsurance ?? (defaultInsurance * 12);

            CostBreakdown = _calculationService.CalculateMonthlyHousingCost(
                effectivePrice,
                downPayment,
                interestRate,
                term,
                annualTax,
                annualInsurance,
                MonthlyHOA);
        }
    }
}
