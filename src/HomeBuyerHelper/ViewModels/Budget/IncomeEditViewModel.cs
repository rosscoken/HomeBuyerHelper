using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Budget;

/// <summary>
/// ViewModel for adding/editing an income source (P2-INC-002 through 005).
/// </summary>
[QueryProperty(nameof(IncomeId), "id")]
public partial class IncomeEditViewModel : BaseViewModel
{
    private readonly IIncomeRepository _incomeRepository;
    private readonly IIncomeScenarioService _incomeScenarioService;

    [ObservableProperty]
    private int _incomeId;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private decimal _grossAmount;

    [ObservableProperty]
    private IncomeFrequency _frequency = IncomeFrequency.Monthly;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRsu))]
    [NotifyPropertyChangedFor(nameof(ShowPaymentMonth))]
    private IncomeType _incomeType = IncomeType.Employment;

    [ObservableProperty]
    private bool _isReliable = true;

    [ObservableProperty]
    private decimal _probability = 70m;

    [ObservableProperty]
    private int _paymentMonthIndex; // 0 = default, 1-12 = month

    [ObservableProperty]
    private DateTime? _startDate;

    [ObservableProperty]
    private bool _hasStartDate;

    [ObservableProperty]
    private DateTime _startDateValue = DateTime.Today;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private bool _isNewSource = true;

    // RSU helper fields (P2-INC-004)
    [ObservableProperty]
    private decimal _rsuShares;

    [ObservableProperty]
    private decimal _rsuSharePrice;

    [ObservableProperty]
    private decimal _rsuWithholding = 22m;

    [ObservableProperty]
    private decimal _rsuNetPerVest;

    public bool IsRsu => IncomeType == IncomeType.RSU;

    public bool ShowPaymentMonth => Frequency is IncomeFrequency.Quarterly or IncomeFrequency.Annually;

    public IReadOnlyList<IncomeFrequency> Frequencies { get; } = Enum.GetValues<IncomeFrequency>();
    public IReadOnlyList<IncomeType> IncomeTypes { get; } = Enum.GetValues<IncomeType>();
    public IReadOnlyList<string> PaymentMonths { get; } = new[]
    {
        "Default", "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    };

    public IncomeEditViewModel(
        IIncomeRepository incomeRepository,
        IIncomeScenarioService incomeScenarioService)
    {
        _incomeRepository = incomeRepository;
        _incomeScenarioService = incomeScenarioService;
        Title = "Income Source";
    }

    partial void OnIncomeIdChanged(int value)
    {
        _ = LoadSourceAsync(value);
    }

    partial void OnFrequencyChanged(IncomeFrequency value)
    {
        OnPropertyChanged(nameof(ShowPaymentMonth));
    }

    partial void OnRsuSharesChanged(decimal value) => UpdateRsuNet();
    partial void OnRsuSharePriceChanged(decimal value) => UpdateRsuNet();
    partial void OnRsuWithholdingChanged(decimal value) => UpdateRsuNet();

    private void UpdateRsuNet()
    {
        RsuNetPerVest = _incomeScenarioService.CalculateRsuNetPerVest(RsuShares, RsuSharePrice, RsuWithholding);
    }

    [RelayCommand]
    private void ApplyRsuValue()
    {
        if (RsuNetPerVest > 0)
        {
            GrossAmount = RsuNetPerVest;
        }
    }

    private async Task LoadSourceAsync(int id)
    {
        if (id <= 0)
        {
            IsNewSource = true;
            Title = "Add Income";
            return;
        }

        await ExecuteBusyAsync(async () =>
        {
            var source = await _incomeRepository.GetByIdAsync(id);
            if (source == null)
            {
                SetError("Income source not found.");
                return;
            }

            IsNewSource = false;
            Title = "Edit Income";
            Name = source.Name;
            GrossAmount = source.GrossAmount;
            Frequency = source.Frequency;
            IncomeType = source.IncomeType;
            IsReliable = source.IsReliable;
            Probability = source.Probability;
            PaymentMonthIndex = source.PaymentMonth ?? 0;
            HasStartDate = source.StartDate.HasValue;
            StartDateValue = source.StartDate ?? DateTime.Today;
            Notes = source.Notes;
        });
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            SetError("Please enter a name for this income source.");
            return;
        }

        if (GrossAmount <= 0)
        {
            SetError("Amount must be greater than zero.");
            return;
        }

        await ExecuteBusyAsync(async () =>
        {
            var source = new IncomeSource
            {
                Id = IncomeId,
                Name = Name.Trim(),
                GrossAmount = GrossAmount,
                Frequency = Frequency,
                IncomeType = IncomeType,
                IsReliable = IsReliable,
                Probability = IsReliable ? 100m : Math.Clamp(Probability, 0, 100),
                PaymentMonth = PaymentMonthIndex > 0 ? PaymentMonthIndex : null,
                StartDate = HasStartDate ? StartDateValue : null,
                Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim()
            };

            if (IsNewSource)
            {
                await _incomeRepository.CreateAsync(source);
            }
            else
            {
                await _incomeRepository.UpdateAsync(source);
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
