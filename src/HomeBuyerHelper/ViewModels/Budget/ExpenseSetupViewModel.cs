using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Budget;

/// <summary>
/// ViewModel for the expense list (P2-EXP-001).
/// </summary>
public partial class ExpenseSetupViewModel : BaseViewModel
{
    private readonly IExpenseRepository _expenseRepository;

    [ObservableProperty]
    private ObservableCollection<Expense> _fixedExpenses = new();

    [ObservableProperty]
    private ObservableCollection<Expense> _variableExpenses = new();

    [ObservableProperty]
    private decimal _fixedTotal;

    [ObservableProperty]
    private decimal _variableTotal;

    [ObservableProperty]
    private decimal _monthlyTotal;

    public ExpenseSetupViewModel(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
        Title = "Expenses";
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
            var expenses = await _expenseRepository.GetAllAsync();

            FixedExpenses = new ObservableCollection<Expense>(
                expenses.Where(e => !e.IsVariable).OrderByDescending(e => e.MonthlyAmount));
            VariableExpenses = new ObservableCollection<Expense>(
                expenses.Where(e => e.IsVariable).OrderByDescending(e => e.MonthlyAmount));

            FixedTotal = Math.Round(FixedExpenses.Sum(e => e.MonthlyAmount), 0);
            VariableTotal = Math.Round(VariableExpenses.Sum(e => e.MonthlyAmount), 0);
            MonthlyTotal = FixedTotal + VariableTotal;
        });
    }

    [RelayCommand]
    private async Task AddNewAsync()
    {
        await Shell.Current.GoToAsync("ExpenseEdit?id=0");
    }

    [RelayCommand]
    private async Task EditExpenseAsync(Expense expense)
    {
        await Shell.Current.GoToAsync($"ExpenseEdit?id={expense.Id}");
    }

    [RelayCommand]
    private async Task DeleteExpenseAsync(Expense expense)
    {
        var confirmed = await Shell.Current.DisplayAlert(
            "Delete Expense",
            $"Delete '{expense.Name}'?",
            "Delete",
            "Cancel");

        if (!confirmed) return;

        await ExecuteBusyAsync(async () =>
        {
            await _expenseRepository.DeleteAsync(expense.Id);
        });
        await LoadDataAsync();
    }
}
