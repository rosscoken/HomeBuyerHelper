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
    private readonly IIncomeRepository _incomeRepository;
    private readonly IAffordabilityService _affordabilityService;
    private readonly ITrueTotalCostService _trueTotalCostService;
    private readonly IPhotoService _photoService;
    private readonly IProConRepository _proConRepository;
    private UserPreferences? _preferences;
    private IReadOnlyList<IncomeSource> _incomeSources = [];

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
    private IReadOnlyList<AffordabilityAssessment> _affordability = [];

    [ObservableProperty]
    private bool _hasAffordabilityData;

    [ObservableProperty]
    private bool _showAffordabilityWarning;

    [ObservableProperty]
    private int? _commuteMinutesPrimary;

    [ObservableProperty]
    private int? _commuteMinutesSecondary;

    [ObservableProperty]
    private decimal _monthlyUtilities;

    [ObservableProperty]
    private TrueTotalCost? _trueCost;

    [ObservableProperty]
    private CommuteAnalysis? _commuteAnalysis;

    [ObservableProperty]
    private bool _hasCommuteAnalysis;

    [ObservableProperty]
    private System.Collections.ObjectModel.ObservableCollection<PropertyPhoto> _photos = new();

    [ObservableProperty]
    private System.Collections.ObjectModel.ObservableCollection<PropertyProCon> _pros = new();

    [ObservableProperty]
    private System.Collections.ObjectModel.ObservableCollection<PropertyProCon> _cons = new();

    [ObservableProperty]
    private string _newProText = string.Empty;

    [ObservableProperty]
    private string _newConText = string.Empty;

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
    private IReadOnlyList<PropertyScore> _scores = [];

    public IReadOnlyList<PropertyType> PropertyTypes { get; } = Enum.GetValues<PropertyType>();

    public PropertyDetailViewModel(
        IPropertyService propertyService,
        ICalculationService calculationService,
        IUserPreferencesRepository preferencesRepository,
        IIncomeRepository incomeRepository,
        IAffordabilityService affordabilityService,
        ITrueTotalCostService trueTotalCostService,
        IPhotoService photoService,
        IProConRepository proConRepository)
    {
        _propertyService = propertyService;
        _calculationService = calculationService;
        _preferencesRepository = preferencesRepository;
        _incomeRepository = incomeRepository;
        _affordabilityService = affordabilityService;
        _trueTotalCostService = trueTotalCostService;
        _photoService = photoService;
        _proConRepository = proConRepository;
    }

    public override async Task OnAppearingAsync()
    {
        _preferences = await _preferencesRepository.GetAsync();
        _incomeSources = await _incomeRepository.GetAllAsync();
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
                CommuteMinutesPrimary = property.CommuteMinutesPrimary;
                CommuteMinutesSecondary = property.CommuteMinutesSecondary;
                MonthlyUtilities = property.MonthlyUtilities;
                ListingUrl = property.ListingUrl;
                Notes = property.Notes;
                OverallScore = property.OverallScore;
                Rank = property.Rank;
                ScoredCriteriaCount = property.ScoredCriteriaCount;
                TotalCriteriaCount = property.TotalCriteriaCount;
                IsFullyScored = property.IsFullyScored;
                Scores = property.Scores;

                UpdateCostBreakdown();
                await LoadPhotosAsync(id);
                await LoadProsConsAsync(id);
            }
        });
    }

    private async Task LoadPhotosAsync(int id)
    {
        var photos = await _photoService.GetPhotosAsync(id);
        Photos = new System.Collections.ObjectModel.ObservableCollection<PropertyPhoto>(photos);
    }

    private async Task LoadProsConsAsync(int id)
    {
        var items = await _proConRepository.GetByPropertyIdAsync(id);
        Pros = new System.Collections.ObjectModel.ObservableCollection<PropertyProCon>(items.Where(i => i.IsPro));
        Cons = new System.Collections.ObjectModel.ObservableCollection<PropertyProCon>(items.Where(i => !i.IsPro));
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
                CommuteMinutesPrimary = CommuteMinutesPrimary,
                CommuteMinutesSecondary = CommuteMinutesSecondary,
                MonthlyUtilities = MonthlyUtilities,
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

            UpdateAffordability();
        }

        UpdateTrueCost();
    }

    private void UpdateAffordability()
    {
        if (CostBreakdown == null || _incomeSources.Count == 0)
        {
            Affordability = [];
            HasAffordabilityData = false;
            ShowAffordabilityWarning = false;
            return;
        }

        Affordability = _affordabilityService.AssessAllScenarios(CostBreakdown.Total, _incomeSources);
        HasAffordabilityData = true;
        ShowAffordabilityWarning = Affordability.Any(a =>
            a.Zone is AffordabilityZone.Aggressive or AffordabilityZone.Risky);
    }

    /// <summary>
    /// True total cost + commute analysis (P3-TTC-002, P3-COM-004).
    /// </summary>
    private void UpdateTrueCost()
    {
        if (_preferences == null || AskingPrice <= 0)
        {
            TrueCost = null;
            HasCommuteAnalysis = false;
            return;
        }

        var property = new Property
        {
            Nickname = Nickname.Length > 0 ? Nickname : "Property",
            AskingPrice = AskingPrice,
            OfferPrice = OfferPrice,
            MonthlyHOA = MonthlyHOA,
            AnnualPropertyTax = AnnualPropertyTax,
            AnnualInsurance = AnnualInsurance,
            MonthlyUtilities = MonthlyUtilities,
            CommuteMinutesPrimary = CommuteMinutesPrimary,
            CommuteMinutesSecondary = CommuteMinutesSecondary
        };

        TrueCost = _trueTotalCostService.Calculate(property, _preferences);
        CommuteAnalysis = TrueCost.Commute;
        HasCommuteAnalysis = CommuteAnalysis?.HasCommuteData == true;
    }

    [RelayCommand]
    private async Task AddPhotoFromCameraAsync()
    {
        if (!PropertyId.HasValue || PropertyId.Value <= 0)
        {
            SetError("Save the property before adding photos.");
            return;
        }

        var photo = await _photoService.CapturePhotoAsync(PropertyId.Value);
        if (photo != null)
        {
            Photos.Add(photo);
        }
    }

    [RelayCommand]
    private async Task AddPhotoFromGalleryAsync()
    {
        if (!PropertyId.HasValue || PropertyId.Value <= 0)
        {
            SetError("Save the property before adding photos.");
            return;
        }

        var photo = await _photoService.PickPhotoAsync(PropertyId.Value);
        if (photo != null)
        {
            Photos.Add(photo);
        }
    }

    [RelayCommand]
    private async Task DeletePhotoAsync(PropertyPhoto photo)
    {
        var confirmed = await Shell.Current.DisplayAlert(
            "Delete Photo", "Remove this photo?", "Delete", "Cancel");
        if (!confirmed) return;

        await _photoService.DeletePhotoAsync(photo);
        Photos.Remove(photo);
    }

    [RelayCommand]
    private async Task AddProAsync()
    {
        await AddProConAsync(isPro: true, NewProText);
        NewProText = string.Empty;
    }

    [RelayCommand]
    private async Task AddConAsync()
    {
        await AddProConAsync(isPro: false, NewConText);
        NewConText = string.Empty;
    }

    private async Task AddProConAsync(bool isPro, string text)
    {
        if (!PropertyId.HasValue || PropertyId.Value <= 0)
        {
            SetError("Save the property before adding pros and cons.");
            return;
        }

        if (string.IsNullOrWhiteSpace(text)) return;

        var list = isPro ? Pros : Cons;
        var item = new PropertyProCon
        {
            PropertyId = PropertyId.Value,
            IsPro = isPro,
            Description = text.Trim(),
            SortOrder = list.Count
        };
        item.Id = await _proConRepository.CreateAsync(item);
        list.Add(item);
    }

    [RelayCommand]
    private async Task DeleteProConAsync(PropertyProCon item)
    {
        await _proConRepository.DeleteAsync(item.Id);
        (item.IsPro ? Pros : Cons).Remove(item);
    }
}
