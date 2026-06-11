using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.ViewModels.Budget;

/// <summary>
/// ViewModel for adding/editing a one-time event (P2-EXP-004).
/// </summary>
[QueryProperty(nameof(EventId), "id")]
public partial class OneTimeEventEditViewModel : BaseViewModel
{
    private readonly IOneTimeEventRepository _eventRepository;

    [ObservableProperty]
    private int _eventId;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private decimal _amount;

    [ObservableProperty]
    private DateTime _date = DateTime.Today;

    [ObservableProperty]
    private OneTimeEventCategory _category = OneTimeEventCategory.Other;

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private bool _isNewEvent = true;

    public IReadOnlyList<OneTimeEventCategory> Categories { get; } = Enum.GetValues<OneTimeEventCategory>();

    public OneTimeEventEditViewModel(IOneTimeEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
        Title = "One-Time Event";
    }

    partial void OnEventIdChanged(int value)
    {
        _ = LoadEventAsync(value);
    }

    private async Task LoadEventAsync(int id)
    {
        if (id <= 0)
        {
            IsNewEvent = true;
            Title = "Add Event";
            return;
        }

        await ExecuteBusyAsync(async () =>
        {
            var oneTimeEvent = await _eventRepository.GetByIdAsync(id);
            if (oneTimeEvent == null)
            {
                SetError("Event not found.");
                return;
            }

            IsNewEvent = false;
            Title = "Edit Event";
            Name = oneTimeEvent.Name;
            Amount = oneTimeEvent.Amount;
            Date = oneTimeEvent.Date;
            Category = oneTimeEvent.Category;
            Notes = oneTimeEvent.Notes;
        });
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            SetError("Please enter a name for this event.");
            return;
        }

        if (Amount <= 0)
        {
            SetError("Amount must be greater than zero.");
            return;
        }

        await ExecuteBusyAsync(async () =>
        {
            var oneTimeEvent = new OneTimeEvent
            {
                Id = EventId,
                Name = Name.Trim(),
                Amount = Amount,
                Date = Date,
                Category = Category,
                Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim()
            };

            if (IsNewEvent)
            {
                await _eventRepository.CreateAsync(oneTimeEvent);
            }
            else
            {
                await _eventRepository.UpdateAsync(oneTimeEvent);
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
