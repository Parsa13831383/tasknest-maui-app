using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.ApplicationModel;

namespace TaskNest.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    private readonly SemaphoreSlim executionGate = new(1, 1);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;

    public bool IsNotBusy => !IsBusy;

    protected async Task RunExclusiveAsync(Func<Task> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);

        await executionGate.WaitAsync().ConfigureAwait(false);

        try
        {
            await MainThread.InvokeOnMainThreadAsync(() => IsBusy = true);
            await operation().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            PublishError(exception.Message, exception);
        }
        finally
        {
            await MainThread.InvokeOnMainThreadAsync(() => IsBusy = false);
            executionGate.Release();
        }
    }

    protected Task NavigateAsync(string route, IDictionary<string, object>? parameters = null)
    {
        if (Shell.Current is null)
        {
            PublishError("Navigation is not available.");
            return Task.CompletedTask;
        }

        return MainThread.InvokeOnMainThreadAsync(() =>
            parameters is null
                ? Shell.Current.GoToAsync(route)
                : Shell.Current.GoToAsync(route, parameters));
    }

    protected void PublishError(string message, Exception? exception = null)
    {
        WeakReferenceMessenger.Default.Send(new ErrorMessage(message, exception, GetType().Name));
    }
}

public sealed record ErrorMessage(string Message, Exception? Exception = null, string? Source = null);

/// <summary>Broadcast when any task's completion status changes so subscribers can refresh their data.</summary>
public sealed record TaskStatusChangedMessage();
