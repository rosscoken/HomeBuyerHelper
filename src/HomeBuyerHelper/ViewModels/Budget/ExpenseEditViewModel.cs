using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Budget;

/// <summary>
/// ViewModel for adding/editing an expense (P2-EXP-002/003).
/// </summary>
[QueryProperty(nameof(ExpenseId), "id")]
public partial class ExpenseEditViewModel : BaseViewModel
{
    private readonly IExpenseRepository _expenseRepository;

    [ObservableProperty]
    private int _expenseId;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private decimal _amount;

    [ObservableProperty]
    private ExpenseFrequency _frequency = ExpenseFrequency.Monthly;

    [ObservableProperty]
    private ExpenseCategory _category = ExpenseCategory.Other;

    [ObservableProperty]
    private bool _isVariable;

    [ObservableProperty]
    private bool _isEssential = true;

    [ObservableProperty]
    private bool _continuesAfterPurchase = true;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private bool _isNewExpense = true;

    public IReadOnlyList<ExpenseFrequency> Frequencies { get; } = Enum.GetValues<ExpenseFrequency>();
    public IReadOnlyList<ExpenseCategory> Categories { get; } = Enum.GetValues<ExpenseCategory>();

    public ExpenseEditViewModel(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
        Title = "Expense";
    }

    partial void OnExpenseIdChanged(int value)
    {
        _ = LoadExpenseAsync(value);
    }

    private async Task LoadExpenseAsync(int id)
    {
        if (id <= 0)
        {
            IsNewExpense = true;
            Title = "Add Expense";
            return;
        }

        await ExecuteBusyAsync(async () =>
        {
            var expense = await _expenseRepository.GetByIdAsync(id);
            if (expense == null)
            {
                SetError("Expense not found.");
                return;
            }

            IsNewExpense = false;
            Title = "Edit Expense";
            Name = expense.Name;
            Amount = expense.Amount;
            Frequency = expense.Frequency;
            Category = expense.Category;
            IsVariable = expense.IsVariable;
            IsEssential = expense.IsEssential;
            ContinuesAfterPurchase = expense.ContinuesAfterPurchase;
            Notes = expense.Notes;
        });
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            SetError("Please enter a name for this expense.");
            return;
        }

        if (Amount <= 0)
        {
            SetError("Amount must be greater than zero.");
            return;
        }

        await ExecuteBusyAsync(async () =>
        {
            var expense = new Expense
            {
                Id = ExpenseId,
                Name = Name.Trim(),
                Amount = Amount,
                Frequency = Frequency,
                Category = Category,
                IsVariable = IsVariable,
                IsEssential = IsEssential,
                ContinuesAfterPurchase = ContinuesAfterPurchase,
                Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim()
            };

            if (IsNewExpense)
            {
                await _expenseRepository.CreateAsync(expense);
            }
            else
            {
                await _expenseRepository.UpdateAsync(expense);
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
