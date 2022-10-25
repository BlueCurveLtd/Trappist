#pragma warning disable SA1512

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using InGroupe.Innovation.Wpf.Bedrock.Command.Abstractions;
using InGroupe.Innovation.Wpf.Bedrock.ObjectModel;

namespace InGroupe.Innovation.Wpf.Bedrock.Command;

/// <summary>
/// A command that mirrors the functionality of <see cref="RelayCommand"/>, with the addition of
/// accepting a <see cref="Func{TResult}"/> returning a <see cref="Task"/> as the execute
/// action, and providing an <see cref="ExecutionTask"/> property that notifies changes when
/// <see cref="ExecuteAsync"/> is invoked and when the returned <see cref="Task"/> completes.
/// </summary>
public sealed class AsyncRelayCommand : ValidableObjectBase, IAsyncRelayCommand
{
    /// <summary>
    /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="CanBeCanceled"/>.
    /// </summary>
    internal static readonly PropertyChangedEventArgs CanBeCanceledChangedEventArgs = new(nameof(CanBeCanceled));

    /// <summary>
    /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="IsCancellationRequested"/>.
    /// </summary>
    internal static readonly PropertyChangedEventArgs IsCancellationRequestedChangedEventArgs = new(nameof(IsCancellationRequested));

    /// <summary>
    /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="IsRunning"/>.
    /// </summary>
    internal static readonly PropertyChangedEventArgs IsRunningChangedEventArgs = new(nameof(IsRunning));

    /// <summary>
    /// The <see cref="Func{TResult}"/> to invoke when <see cref="Execute"/> is used.
    /// </summary>
    private readonly Func<Task>? execute;

    /// <summary>
    /// The cancelable <see cref="Func{T,TResult}"/> to invoke when <see cref="Execute"/> is used.
    /// </summary>
    /// <remarks>Only one between this and <see cref="execute"/> is not <see langword="null"/>.</remarks>
    private readonly Func<CancellationToken, Task>? cancelableExecute;

    /// <summary>
    /// The optional action to invoke when <see cref="CanExecute"/> is used.
    /// </summary>
    private readonly Func<bool>? canExecute;

    /// <summary>
    /// The <see cref="CancellationTokenSource"/> instance to use to cancel <see cref="cancelableExecute"/>.
    /// </summary>
    /// <remarks>This is only used when <see cref="cancelableExecute"/> is not <see langword="null"/>.</remarks>
    private CancellationTokenSource? cancellationTokenSource;

    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class that can always execute.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    public AsyncRelayCommand([DisallowNull] Func<Task> execute) => this.execute = execute;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class that can always execute.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution logic.</param>
    public AsyncRelayCommand([DisallowNull] Func<CancellationToken, Task> cancelableExecute) => this.cancelableExecute = cancelableExecute;


    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public AsyncRelayCommand([DisallowNull] Func<Task> execute, [DisallowNull] Func<bool> canExecute)
    {
        this.execute = execute;
        this.canExecute = canExecute;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="cancelableExecute">The cancelable execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public AsyncRelayCommand([DisallowNull] Func<CancellationToken, Task> cancelableExecute, [DisallowNull] Func<bool> canExecute)
    {
        this.cancelableExecute = cancelableExecute;
        this.canExecute = canExecute;
    }

    private TaskNotifier? executionTask;

    /// <inheritdoc/>
    public Task? ExecutionTask
    {
        get => this.executionTask;
        private set
        {
            if (this.SetPropertyAndNotifyOnCompletion(ref this.executionTask, value, _ =>
            {
                // When the task completes
                this.OnPropertyChanged(IsRunningChangedEventArgs);
                this.OnPropertyChanged(CanBeCanceledChangedEventArgs);
            }))
            {
                // When setting the task
                this.OnPropertyChanged(IsRunningChangedEventArgs);
                this.OnPropertyChanged(CanBeCanceledChangedEventArgs);
            }
        }
    }

    /// <inheritdoc/>
    public bool CanBeCanceled => this.cancelableExecute is not null && this.IsRunning;

    /// <inheritdoc/>
    public bool IsCancellationRequested => this.cancellationTokenSource?.IsCancellationRequested == true;

    /// <inheritdoc/>
    public bool IsRunning => this.ExecutionTask?.IsCompleted == false;

    /// <inheritdoc/>
    public void NotifyCanExecuteChanged() => this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanExecute(object? parameter) => this.canExecute?.Invoke() != false;

    /// <inheritdoc/>
    public void Execute(object? parameter) => _ = this.ExecuteAsync(parameter);

    /// <inheritdoc/>
    public Task ExecuteAsync(object? parameter)
    {
        if (this.CanExecute(parameter))
        {
            // Non cancelable command delegate
            if (this.execute is not null)
            {
                return this.ExecutionTask = this.execute();
            }

            // Cancel the previous operation, if one is pending
            this.cancellationTokenSource?.Cancel();

            CancellationTokenSource cancellationTokenSource = this.cancellationTokenSource = new();

            this.OnPropertyChanged(IsCancellationRequestedChangedEventArgs);

            // Invoke the cancelable command delegate with a new linked token
            return this.ExecutionTask = this.cancelableExecute!(cancellationTokenSource.Token);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Cancel()
    {
        this.cancellationTokenSource?.Cancel();

        this.OnPropertyChanged(IsCancellationRequestedChangedEventArgs);
        this.OnPropertyChanged(CanBeCanceledChangedEventArgs);
    }
}
