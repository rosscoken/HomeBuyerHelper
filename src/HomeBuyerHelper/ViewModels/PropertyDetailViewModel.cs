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

    public IReadOnlyList<PropertyType> PropertyTypes { get; } = Enum.GetValues<PropertyType>();

    public PropertyDetailViewModel(
        IPropertyService propertyService,
        ICalculationService calculationService)
    {
        _propertyService = propertyService;
        _calculationService = calculationService;
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
    private void CalculateCosts()
    {
        UpdateCostBreakdown();
    }

    private void UpdateCostBreakdown()
    {
        if (AskingPrice > 0)
        {
            CostBreakdown = _calculationService.CalculateMonthlyHousingCost(
                OfferPrice ?? AskingPrice,
                20m, // Default down payment
                7.0m, // Default interest rate
                30, // Default term
                AnnualPropertyTax ?? 0,
                AnnualInsurance ?? 0,
                MonthlyHOA);
        }
    }
}
