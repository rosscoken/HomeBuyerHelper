using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Budget;

/// <summary>
/// ViewModel for the one-time events calendar (P2-EXP-004).
/// </summary>
public partial class OneTimeEventsViewModel : BaseViewModel
{
    private readonly IOneTimeEventRepository _eventRepository;

    [ObservableProperty]
    private ObservableCollection<OneTimeEvent> _events = new();

    [ObservableProperty]
    private decimal _totalUpcoming;

    public OneTimeEventsViewModel(IOneTimeEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
        Title = "One-Time Events";
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
            var events = await _eventRepository.GetAllAsync();
            Events = new ObservableCollection<OneTimeEvent>(events);
            TotalUpcoming = events
                .Where(e => e.Date >= new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1))
                .Sum(e => e.Amount);
        });
    }

    [RelayCommand]
    private async Task AddNewAsync()
    {
        await Shell.Current.GoToAsync("OneTimeEventEdit?id=0");
    }

    [RelayCommand]
    private async Task EditEventAsync(OneTimeEvent oneTimeEvent)
    {
        await Shell.Current.GoToAsync($"OneTimeEventEdit?id={oneTimeEvent.Id}");
    }

    [RelayCommand]
    private async Task DeleteEventAsync(OneTimeEvent oneTimeEvent)
    {
        var confirmed = await Shell.Current.DisplayAlert(
            "Delete Event",
            $"Delete '{oneTimeEvent.Name}'?",
            "Delete",
            "Cancel");

        if (!confirmed) return;

        await ExecuteBusyAsync(async () =>
        {
            await _eventRepository.DeleteAsync(oneTimeEvent.Id);
        });
        await LoadDataAsync();
    }
}
