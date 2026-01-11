using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Budget;

/// <summary>
/// View model for income source setup and management.
/// </summary>
public partial class IncomeSetupViewModel : BaseViewModel
{
    private readonly IIncomeRepository _incomeRepository;

    [ObservableProperty]
    private ObservableCollection<IncomeSource> _incomeSources = new();

    [ObservableProperty]
    private IncomeSource? _selectedIncome;

    [ObservableProperty]
    private decimal _totalMonthlyIncome;

    [ObservableProperty]
    private decimal _totalAnnualIncome;

    [ObservableProperty]
    private decimal _reliableMonthlyIncome;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private string _editName = string.Empty;

    [ObservableProperty]
    private decimal _editGrossAmount;

    [ObservableProperty]
    private IncomeFrequency _editFrequency = IncomeFrequency.Monthly;

    [ObservableProperty]
    private IncomeType _editIncomeType = IncomeType.Employment;

    [ObservableProperty]
    private bool _editIsReliable = true;

    [ObservableProperty]
    private string? _editNotes;

    public IReadOnlyList<IncomeFrequency> FrequencyOptions { get; } = Enum.GetValues<IncomeFrequency>();
    public IReadOnlyList<IncomeType> IncomeTypeOptions { get; } = Enum.GetValues<IncomeType>();

    public IncomeSetupViewModel(IIncomeRepository incomeRepository)
    {
        _incomeRepository = incomeRepository;
        Title = "Income Setup";
    }

    public override async Task OnAppearingAsync()
    {
        await LoadIncomesAsync();
    }

    [RelayCommand]
    private async Task LoadIncomesAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            var incomes = await _incomeRepository.GetAllAsync();
            IncomeSources.Clear();
            foreach (var income in incomes)
            {
                IncomeSources.Add(income);
            }
            UpdateTotals();
        });
    }

    [RelayCommand]
    private void StartAddIncome()
    {
        SelectedIncome = null;
        EditName = string.Empty;
        EditGrossAmount = 0;
        EditFrequency = IncomeFrequency.Monthly;
        EditIncomeType = IncomeType.Employment;
        EditIsReliable = true;
        EditNotes = null;
        IsEditing = true;
        ClearError();
    }

    [RelayCommand]
    private void StartEditIncome(IncomeSource income)
    {
        SelectedIncome = income;
        EditName = income.Name;
        EditGrossAmount = income.GrossAmount;
        EditFrequency = income.Frequency;
        EditIncomeType = income.IncomeType;
        EditIsReliable = income.IsReliable;
        EditNotes = income.Notes;
        IsEditing = true;
        ClearError();
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditing = false;
        SelectedIncome = null;
        ClearError();
    }

    [RelayCommand]
    private async Task SaveIncomeAsync()
    {
        if (!ValidateInput())
            return;

        await ExecuteBusyAsync(async () =>
        {
            if (SelectedIncome == null)
            {
                // Creating new income
                var newIncome = new IncomeSource
                {
                    Name = EditName.Trim(),
                    GrossAmount = EditGrossAmount,
                    Frequency = EditFrequency,
                    IncomeType = EditIncomeType,
                    IsReliable = EditIsReliable,
                    Notes = string.IsNullOrWhiteSpace(EditNotes) ? null : EditNotes.Trim()
                };
                await _incomeRepository.CreateAsync(newIncome);
            }
            else
            {
                // Updating existing income
                SelectedIncome.Name = EditName.Trim();
                SelectedIncome.GrossAmount = EditGrossAmount;
                SelectedIncome.Frequency = EditFrequency;
                SelectedIncome.IncomeType = EditIncomeType;
                SelectedIncome.IsReliable = EditIsReliable;
                SelectedIncome.Notes = string.IsNullOrWhiteSpace(EditNotes) ? null : EditNotes.Trim();
                await _incomeRepository.UpdateAsync(SelectedIncome);
            }

            IsEditing = false;
            SelectedIncome = null;
            await LoadIncomesAsync();
        });
    }

    [RelayCommand]
    private async Task DeleteIncomeAsync(IncomeSource income)
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Delete Income",
            $"Are you sure you want to delete '{income.Name}'?",
            "Delete",
            "Cancel");

        if (!confirm)
            return;

        await ExecuteBusyAsync(async () =>
        {
            await _incomeRepository.DeleteAsync(income.Id);
            await LoadIncomesAsync();
        });
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(EditName))
        {
            SetError("Please enter a name for this income source.");
            return false;
        }

        if (EditGrossAmount <= 0)
        {
            SetError("Please enter a valid gross amount greater than zero.");
            return false;
        }

        ClearError();
        return true;
    }

    private void UpdateTotals()
    {
        TotalMonthlyIncome = IncomeSources.Sum(i => i.MonthlyGrossIncome);
        TotalAnnualIncome = TotalMonthlyIncome * 12;
        ReliableMonthlyIncome = IncomeSources.Where(i => i.IsReliable).Sum(i => i.MonthlyGrossIncome);
    }

    public string GetFrequencyDisplayText(IncomeFrequency frequency) => frequency switch
    {
        IncomeFrequency.Weekly => "Weekly",
        IncomeFrequency.BiWeekly => "Bi-weekly",
        IncomeFrequency.SemiMonthly => "Semi-monthly",
        IncomeFrequency.Monthly => "Monthly",
        IncomeFrequency.Quarterly => "Quarterly",
        IncomeFrequency.Annually => "Annually",
        _ => frequency.ToString()
    };

    public string GetIncomeTypeDisplayText(IncomeType type) => type switch
    {
        IncomeType.Employment => "Employment",
        IncomeType.SelfEmployment => "Self-Employment",
        IncomeType.Rental => "Rental Income",
        IncomeType.Investment => "Investment",
        IncomeType.Retirement => "Retirement",
        IncomeType.SocialSecurity => "Social Security",
        IncomeType.Disability => "Disability",
        IncomeType.Alimony => "Alimony",
        IncomeType.ChildSupport => "Child Support",
        IncomeType.Other => "Other",
        _ => type.ToString()
    };
}
