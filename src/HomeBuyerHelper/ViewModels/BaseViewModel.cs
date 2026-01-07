using CommunityToolkit.Mvvm.ComponentModel;

namespace HomeBuyerHelper.ViewModels;

/// <summary>
/// Base view model providing common functionality for all view models.
/// </summary>
public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>
    /// Inverse of IsBusy for binding convenience.
    /// </summary>
    public bool IsNotBusy => !IsBusy;

    /// <summary>
    /// Whether there is an error to display.
    /// </summary>
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    /// <summary>
    /// Called when the page appears. Override to load data.
    /// </summary>
    public virtual Task OnAppearingAsync() => Task.CompletedTask;

    /// <summary>
    /// Called when the page disappears. Override to cleanup.
    /// </summary>
    public virtual Task OnDisappearingAsync() => Task.CompletedTask;

    /// <summary>
    /// Clears any error message.
    /// </summary>
    protected void ClearError() => ErrorMessage = null;

    /// <summary>
    /// Sets an error message.
    /// </summary>
    protected void SetError(string message) => ErrorMessage = message;

    /// <summary>
    /// Executes an action with busy indicator.
    /// </summary>
    protected async Task ExecuteBusyAsync(Func<Task> action)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ClearError();
            await action();
        }
        catch (Exception ex)
        {
            SetError(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Executes an action with busy indicator and returns a result.
    /// </summary>
    protected async Task<T?> ExecuteBusyAsync<T>(Func<Task<T>> action)
    {
        if (IsBusy) return default;

        try
        {
            IsBusy = true;
            ClearError();
            return await action();
        }
        catch (Exception ex)
        {
            SetError(ex.Message);
            return default;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
