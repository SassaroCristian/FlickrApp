using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlickrApp.ViewModels.Base;

public abstract partial class BaseViewModel : ObservableObject
{
    private static readonly AsyncLocal<bool> IsCurrentlyInSafeExecution = new();
    
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;
    public bool IsNotBusy => !IsBusy;

    protected void ExecuteSafely(Action operation, Action<Exception>? onError = null, Action? onComplete = null)
    {
        var isNestedCall = IsCurrentlyInSafeExecution.Value;

        if (!isNestedCall)
        {
            if (IsBusy)
            {
                Debug.WriteLine(
                    "ExecuteSafely: ViewModel is busy with a top-level operation. New top-level call skipped");
                return;
            }

            IsBusy = true;
            IsCurrentlyInSafeExecution.Value = true;
        }
        else
        {
            Debug.WriteLine("ExecuteSafely: Nested call detected");
        }
        
        try
        {
            operation.Invoke();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during ExecuteSafely (Nested: {isNestedCall}): {ex.Message}");
            onError?.Invoke(ex);
        }
        finally
        {
            if (!isNestedCall)
            {
                Debug.WriteLine("ExecuteSafely: Top-level operation finished.");
                IsBusy = false;
                IsCurrentlyInSafeExecution.Value = false;
            }
            else
            {
                Debug.WriteLine("ExecuteSafely: Nested operation finished its try-catch-finally");
            }
            onComplete?.Invoke();
        }
    }

    protected async Task ExecuteSafelyAsync(Func<Task> operation, Action<Exception>? onError = null,
        Action? onComplete = null)
    {
        var isNestedCall = IsCurrentlyInSafeExecution.Value;

        if (!isNestedCall)
        {
            if (IsBusy)
            {
                Debug.WriteLine(
                    "ExecuteSafelyAsync: ViewModel is busy with a top-level operation. New top-level call skipped");
                return;
            }

            IsBusy = true;
            IsCurrentlyInSafeExecution.Value = true;
        }
        else
        {
            Debug.WriteLine("ExecuteSafelyAsync: Nested call detected");
        }
        
        try
        {
            ArgumentNullException.ThrowIfNull(operation);
            await operation();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during ExecuteSafelyAsync (Nested: {isNestedCall}): {ex.Message}");
            onError?.Invoke(ex);
        }
        finally
        {
            if (!isNestedCall)
            {
                Debug.WriteLine("ExecuteSafelyAsync: Top-level operation finished.");
                IsBusy = false;
                IsCurrentlyInSafeExecution.Value = false;
            }
            else
            {
                Debug.WriteLine("ExecuteSafelyAsync: Nested operation finished its try-catch-finally");
            }
            onComplete?.Invoke();
        }
    }

    protected async Task<TResult?> ExecuteSafelyAsync<TResult>(Func<Task<TResult>> operation,
        Action<Exception>? onError = null, Action? onComplete = null)
    {
        var isNestedCall = IsCurrentlyInSafeExecution.Value;
        TResult? result = default;

        if (!isNestedCall)
        {
            if (IsBusy)
            {
                Debug.WriteLine(
                    "ExecuteSafelyAsync<TResult>: ViewModel is busy with a top-level operation. New top-level call skipped");
                return result;
            }

            IsBusy = true;
            IsCurrentlyInSafeExecution.Value = true;
        }
        else
        {
            Debug.WriteLine("ExecuteSafelyAsync<TResult>: Nested call detected");
        }
        
        try
        {
            ArgumentNullException.ThrowIfNull(operation);
            result = await operation();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during ExecuteSafelyAsync<TResult> (Nested: {isNestedCall}): {ex.Message}");
            onError?.Invoke(ex);
        }
        finally
        {
            if (!isNestedCall)
            {
                Debug.WriteLine("ExecuteSafelyAsync<TResult>: Top-level operation finished.");
                IsBusy = false;
                IsCurrentlyInSafeExecution.Value = false;
            }
            else
            {
                Debug.WriteLine("ExecuteSafelyAsync<TResult>: Nested operation finished its try-catch-finally");
            }
            onComplete?.Invoke();
        }

        return result;
    }
}