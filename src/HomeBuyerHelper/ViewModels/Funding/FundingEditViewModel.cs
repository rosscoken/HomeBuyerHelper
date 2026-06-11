using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Funding;

/// <summary>
/// ViewModel for adding/editing a funding source with type-specific fields
/// (P3-FUN-003 through P3-FUN-009).
/// </summary>
[QueryProperty(nameof(SourceId), "id")]
public partial class FundingEditViewModel : BaseViewModel
{
    private readonly IFundingRepository _fundingRepository;
    private readonly ITaxImpactService _taxImpactService;
    private readonly IUserPreferencesRepository _preferencesRepository;

    [ObservableProperty]
    private int _sourceId;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private decimal _amount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsBrokerage))]
    [NotifyPropertyChangedFor(nameof(IsTraditionalIra))]
    [NotifyPropertyChangedFor(nameof(IsRothIra))]
    [NotifyPropertyChangedFor(nameof(IsInheritedIra))]
    [NotifyPropertyChangedFor(nameof(Is401k))]
    [NotifyPropertyChangedFor(nameof(IsGift))]
    [NotifyPropertyChangedFor(nameof(NeedsAge))]
    private int _typeIndex;

    // Brokerage
    [ObservableProperty]
    private decimal? _costBasis;

    [ObservableProperty]
    private bool _isLongTermHolding = true;

    // IRA / 401k
    [ObservableProperty]
    private int _ownerAge = 35;

    [ObservableProperty]
    private bool _isFirstTimeBuyer = true;

    // Roth
    [ObservableProperty]
    private decimal _rothContributionPortion;

    [ObservableProperty]
    private bool _isRothAccount5Years;

    // 401k
    [ObservableProperty]
    private bool _is401kLoan = true;

    // Gift
    [ObservableProperty]
    private string? _donorName;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private bool _isNewSource = true;

    [ObservableProperty]
    private string _taxPreview = string.Empty;

    /// <summary>
    /// Source types shown to the user, mirroring the spec's selection list.
    /// </summary>
    public IReadOnlyList<string> TypeNames { get; } = new[]
    {
        "Savings Account",
        "Brokerage Account",
        "Traditional IRA",
        "Roth IRA",
        "Inherited IRA",
        "401(k)",
        "Family Gift",
        "Other"
    };

    private static readonly FundingType[] TypeMap =
    {
        FundingType.Savings,
        FundingType.Investment,
        FundingType.RetirementIRA,
        FundingType.RothIRA,
        FundingType.InheritedIRA,
        FundingType.Retirement401k,
        FundingType.Gift,
        FundingType.Other
    };

    public bool IsBrokerage => TypeMap[Math.Clamp(TypeIndex, 0, TypeMap.Length - 1)] == FundingType.Investment;
    public bool IsTraditionalIra => TypeMap[Math.Clamp(TypeIndex, 0, TypeMap.Length - 1)] == FundingType.RetirementIRA;
    public bool IsRothIra => TypeMap[Math.Clamp(TypeIndex, 0, TypeMap.Length - 1)] == FundingType.RothIRA;
    public bool IsInheritedIra => TypeMap[Math.Clamp(TypeIndex, 0, TypeMap.Length - 1)] == FundingType.InheritedIRA;
    public bool Is401k => TypeMap[Math.Clamp(TypeIndex, 0, TypeMap.Length - 1)] == FundingType.Retirement401k;
    public bool IsGift => TypeMap[Math.Clamp(TypeIndex, 0, TypeMap.Length - 1)] == FundingType.Gift;
    public bool NeedsAge => IsTraditionalIra || IsRothIra || Is401k;

    public FundingEditViewModel(
        IFundingRepository fundingRepository,
        ITaxImpactService taxImpactService,
        IUserPreferencesRepository preferencesRepository)
    {
        _fundingRepository = fundingRepository;
        _taxImpactService = taxImpactService;
        _preferencesRepository = preferencesRepository;
        Title = "Funding Source";
    }

    partial void OnSourceIdChanged(int value)
    {
        _ = LoadSourceAsync(value);
    }

    partial void OnAmountChanged(decimal value) => _ = UpdateTaxPreviewAsync();
    partial void OnTypeIndexChanged(int value) => _ = UpdateTaxPreviewAsync();
    partial void OnCostBasisChanged(decimal? value) => _ = UpdateTaxPreviewAsync();
    partial void OnIsLongTermHoldingChanged(bool value) => _ = UpdateTaxPreviewAsync();
    partial void OnOwnerAgeChanged(int value) => _ = UpdateTaxPreviewAsync();
    partial void OnIsFirstTimeBuyerChanged(bool value) => _ = UpdateTaxPreviewAsync();
    partial void OnRothContributionPortionChanged(decimal value) => _ = UpdateTaxPreviewAsync();
    partial void OnIsRothAccount5YearsChanged(bool value) => _ = UpdateTaxPreviewAsync();
    partial void OnIs401kLoanChanged(bool value) => _ = UpdateTaxPreviewAsync();

    private async Task UpdateTaxPreviewAsync()
    {
        if (Amount <= 0)
        {
            TaxPreview = string.Empty;
            return;
        }

        try
        {
            var preferences = await _preferencesRepository.GetAsync();
            var result = _taxImpactService.CalculateTax(BuildSource(), preferences);
            TaxPreview = $"Est. tax: {result.TotalTax:C0} → Net: {result.NetAmount:C0}\n{result.Explanation}";
        }
        catch (Exception)
        {
            TaxPreview = string.Empty;
        }
    }

    private FundingSource BuildSource() => new()
    {
        Id = SourceId,
        Name = string.IsNullOrWhiteSpace(Name) ? TypeNames[Math.Clamp(TypeIndex, 0, TypeNames.Count - 1)] : Name.Trim(),
        CurrentAmount = Amount,
        FundingType = TypeMap[Math.Clamp(TypeIndex, 0, TypeMap.Length - 1)],
        CostBasis = IsBrokerage ? CostBasis : null,
        IsLongTermHolding = IsLongTermHolding,
        OwnerAge = NeedsAge ? OwnerAge : null,
        IsFirstTimeBuyer = IsFirstTimeBuyer,
        RothContributionPortion = IsRothIra ? RothContributionPortion : null,
        IsRothAccount5Years = IsRothAccount5Years,
        Is401kLoan = Is401kLoan,
        DonorName = IsGift ? DonorName : null,
        Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim()
    };

    private async Task LoadSourceAsync(int id)
    {
        if (id <= 0)
        {
            IsNewSource = true;
            Title = "Add Funding Source";
            return;
        }

        await ExecuteBusyAsync(async () =>
        {
            var source = await _fundingRepository.GetByIdAsync(id);
            if (source == null)
            {
                SetError("Funding source not found.");
                return;
            }

            IsNewSource = false;
            Title = "Edit Funding Source";
            Name = source.Name;
            Amount = source.CurrentAmount;
            TypeIndex = Math.Max(0, Array.IndexOf(TypeMap, source.FundingType));
            CostBasis = source.CostBasis;
            IsLongTermHolding = source.IsLongTermHolding;
            OwnerAge = source.OwnerAge ?? 35;
            IsFirstTimeBuyer = source.IsFirstTimeBuyer;
            RothContributionPortion = source.RothContributionPortion ?? 0;
            IsRothAccount5Years = source.IsRothAccount5Years;
            Is401kLoan = source.Is401kLoan;
            DonorName = source.DonorName;
            Notes = source.Notes;
        });
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (Amount <= 0)
        {
            SetError("Amount must be greater than zero.");
            return;
        }

        await ExecuteBusyAsync(async () =>
        {
            var source = BuildSource();

            if (IsNewSource)
            {
                await _fundingRepository.CreateAsync(source);
            }
            else
            {
                await _fundingRepository.UpdateAsync(source);
            }

            await Shell.Current.GoToAsync("..");
        });
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
