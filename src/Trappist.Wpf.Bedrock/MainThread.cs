using System;
using System.Threading.Tasks;
using System.Windows.Threading;

using Trappist.Wpf.Bedrock.Abstractions;

namespace Trappist.Wpf.Bedrock;

public sealed class MainThread : IMainThread
{
    public void Execute(Action action)
    {
        if (!System.Windows.Application.Current.CheckAccess())
        {
            _ = System.Windows.Application.Current.Dispatcher.BeginInvoke(action, DispatcherPriority.Background);
        }
        else
        {
            action();
        }
    }

    public Task ExecuteAsync(Action action)
    {
        if (!System.Windows.Application.Current.Dispatcher.CheckAccess())
        {
            return RunAsync(System.Windows.Application.Current.Dispatcher, action);
        }

        return RunSynchronously(action);
    }

    private static Task RunAsync(Dispatcher dispatcher, Action action)
    {
        var completionSource = new TaskCompletionSource<object?>();

        var dispatcherOperation = dispatcher.BeginInvoke(() =>
        {
            try
            {
                action();

                completionSource.SetResult(default);
            }
            catch (Exception ex)
            {
                completionSource.SetException(ex);
            }
        }, DispatcherPriority.Background);

        dispatcherOperation.Aborted += (s, e) => completionSource.SetCanceled();
        dispatcherOperation.Completed += (s, e) => completionSource.SetResult(default);

        return completionSource.Task;
    }

    private static Task RunSynchronously(Action action)
    {
        var completionSource = new TaskCompletionSource<object?>();
        try
        {
            action();
            completionSource.SetResult(default);
        }
        catch (Exception ex)
        {
            completionSource.SetException(ex);
        }

        return completionSource.Task;
    }
}
