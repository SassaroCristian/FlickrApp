using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlickrApp.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;
    public bool IsNotBusy => !IsBusy;

    protected void ExecuteSafely(Action operation, Action<Exception>? onError = null, Action? onComplete = null)
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            operation.Invoke();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during execution: {ex.Message}");
            onError?.Invoke(ex);
        }
        finally
        {
            IsBusy = false;
            onComplete?.Invoke();
        }
    }

    protected async Task ExecuteSafelyAsync(Func<Task> operation, Action<Exception>? onError = null,
        Action? onComplete = null)
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            ArgumentNullException.ThrowIfNull(operation);
            await operation();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during execution: {ex.Message}");
            onError?.Invoke(ex);
        }
        finally
        {
            IsBusy = false;
            onComplete?.Invoke();
        }
    }

    protected async Task<TResult?> ExecuteSafelyAsync<TResult>(Func<Task<TResult>> operation,
        Action<Exception>? onError = null, Action? onComplete = null)
    {
        if (IsBusy) return default;

        IsBusy = true;
        TResult? result = default;
        try
        {
            ArgumentNullException.ThrowIfNull(operation);
            result = await operation();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during execution: {ex.Message}");
            onError?.Invoke(ex);
        }
        finally
        {
            IsBusy = false;
            onComplete?.Invoke();
        }

        return result;
    }
}