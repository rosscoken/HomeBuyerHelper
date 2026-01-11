using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Budget;

/// <summary>
/// View model for expense setup and management.
/// </summary>
public partial class ExpenseSetupViewModel : BaseViewModel
{
    private readonly IExpenseRepository _expenseRepository;

    [ObservableProperty]
    private ObservableCollection<Expense> _expenses = new();

    [ObservableProperty]
    private Expense? _selectedExpense;

    [ObservableProperty]
    private decimal _totalMonthlyExpenses;

    [ObservableProperty]
    private decimal _totalAnnualExpenses;

    [ObservableProperty]
    private decimal _essentialMonthlyExpenses;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private string _editName = string.Empty;

    [ObservableProperty]
    private decimal _editAmount;

    [ObservableProperty]
    private ExpenseFrequency _editFrequency = ExpenseFrequency.Monthly;

    [ObservableProperty]
    private ExpenseCategory _editCategory = ExpenseCategory.Other;

    [ObservableProperty]
    private bool _editIsEssential = true;

    [ObservableProperty]
    private bool _editContinuesAfterPurchase = true;

    [ObservableProperty]
    private string? _editNotes;

    [ObservableProperty]
    private ExpenseCategory? _filterCategory;

    public IReadOnlyList<ExpenseFrequency> FrequencyOptions { get; } = Enum.GetValues<ExpenseFrequency>();
    public IReadOnlyList<ExpenseCategory> CategoryOptions { get; } = Enum.GetValues<ExpenseCategory>();

    public ExpenseSetupViewModel(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
        Title = "Expense Setup";
    }

    public override async Task OnAppearingAsync()
    {
        await LoadExpensesAsync();
    }

    [RelayCommand]
    private async Task LoadExpensesAsync()
    {
        await ExecuteBusyAsync(async () =>
        {
            IReadOnlyList<Expense> expenses;
            if (FilterCategory.HasValue)
            {
                expenses = await _expenseRepository.GetByCategoryAsync(FilterCategory.Value);
            }
            else
            {
                expenses = await _expenseRepository.GetAllAsync();
            }

            Expenses.Clear();
            foreach (var expense in expenses)
            {
                Expenses.Add(expense);
            }
            UpdateTotals();
        });
    }

    [RelayCommand]
    private async Task FilterByCategoryAsync(ExpenseCategory? category)
    {
        FilterCategory = category;
        await LoadExpensesAsync();
    }

    [RelayCommand]
    private void StartAddExpense()
    {
        SelectedExpense = null;
        EditName = string.Empty;
        EditAmount = 0;
        EditFrequency = ExpenseFrequency.Monthly;
        EditCategory = ExpenseCategory.Other;
        EditIsEssential = true;
        EditContinuesAfterPurchase = true;
        EditNotes = null;
        IsEditing = true;
        ClearError();
    }

    [RelayCommand]
    private void StartEditExpense(Expense expense)
    {
        SelectedExpense = expense;
        EditName = expense.Name;
        EditAmount = expense.Amount;
        EditFrequency = expense.Frequency;
        EditCategory = expense.Category;
        EditIsEssential = expense.IsEssential;
        EditContinuesAfterPurchase = expense.ContinuesAfterPurchase;
        EditNotes = expense.Notes;
        IsEditing = true;
        ClearError();
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditing = false;
        SelectedExpense = null;
        ClearError();
    }

    [RelayCommand]
    private async Task SaveExpenseAsync()
    {
        if (!ValidateInput())
            return;

        await ExecuteBusyAsync(async () =>
        {
            if (SelectedExpense == null)
            {
                // Creating new expense
                var newExpense = new Expense
                {
                    Name = EditName.Trim(),
                    Amount = EditAmount,
                    Frequency = EditFrequency,
                    Category = EditCategory,
                    IsEssential = EditIsEssential,
                    ContinuesAfterPurchase = EditContinuesAfterPurchase,
                    Notes = string.IsNullOrWhiteSpace(EditNotes) ? null : EditNotes.Trim()
                };
                await _expenseRepository.CreateAsync(newExpense);
            }
            else
            {
                // Updating existing expense
                SelectedExpense.Name = EditName.Trim();
                SelectedExpense.Amount = EditAmount;
                SelectedExpense.Frequency = EditFrequency;
                SelectedExpense.Category = EditCategory;
                SelectedExpense.IsEssential = EditIsEssential;
                SelectedExpense.ContinuesAfterPurchase = EditContinuesAfterPurchase;
                SelectedExpense.Notes = string.IsNullOrWhiteSpace(EditNotes) ? null : EditNotes.Trim();
                await _expenseRepository.UpdateAsync(SelectedExpense);
            }

            IsEditing = false;
            SelectedExpense = null;
            await LoadExpensesAsync();
        });
    }

    [RelayCommand]
    private async Task DeleteExpenseAsync(Expense expense)
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Delete Expense",
            $"Are you sure you want to delete '{expense.Name}'?",
            "Delete",
            "Cancel");

        if (!confirm)
            return;

        await ExecuteBusyAsync(async () =>
        {
            await _expenseRepository.DeleteAsync(expense.Id);
            await LoadExpensesAsync();
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
            SetError("Please enter a name for this expense.");
            return false;
        }

        if (EditAmount <= 0)
        {
            SetError("Please enter a valid amount greater than zero.");
            return false;
        }

        ClearError();
        return true;
    }

    private void UpdateTotals()
    {
        TotalMonthlyExpenses = Expenses.Sum(e => e.MonthlyAmount);
        TotalAnnualExpenses = TotalMonthlyExpenses * 12;
        EssentialMonthlyExpenses = Expenses.Where(e => e.IsEssential).Sum(e => e.MonthlyAmount);
    }

    public string GetFrequencyDisplayText(ExpenseFrequency frequency) => frequency switch
    {
        ExpenseFrequency.Weekly => "Weekly",
        ExpenseFrequency.BiWeekly => "Bi-weekly",
        ExpenseFrequency.Monthly => "Monthly",
        ExpenseFrequency.Quarterly => "Quarterly",
        ExpenseFrequency.SemiAnnually => "Semi-annually",
        ExpenseFrequency.Annually => "Annually",
        _ => frequency.ToString()
    };

    public string GetCategoryDisplayText(ExpenseCategory category) => category switch
    {
        ExpenseCategory.Housing => "Housing",
        ExpenseCategory.Transportation => "Transportation",
        ExpenseCategory.Food => "Food",
        ExpenseCategory.Utilities => "Utilities",
        ExpenseCategory.Insurance => "Insurance",
        ExpenseCategory.Healthcare => "Healthcare",
        ExpenseCategory.DebtPayments => "Debt Payments",
        ExpenseCategory.Entertainment => "Entertainment",
        ExpenseCategory.PersonalCare => "Personal Care",
        ExpenseCategory.Clothing => "Clothing",
        ExpenseCategory.Education => "Education",
        ExpenseCategory.Savings => "Savings",
        ExpenseCategory.Subscriptions => "Subscriptions",
        ExpenseCategory.Childcare => "Childcare",
        ExpenseCategory.PetCare => "Pet Care",
        ExpenseCategory.Other => "Other",
        _ => category.ToString()
    };
}
