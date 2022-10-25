using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace Trappist.Wpf.Bedrock.Command;

public class RelayCommand : ICommand
{
    private readonly Action methodToExecute;
    private readonly Func<bool>? canExecuteEvaluator;

    /// <summary>
    /// Occurs when changes occur that affect whether or not the command should execute.
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand"/> class.
    /// </summary>
    /// <param name="methodToExecute">The method to execute.</param>
    /// <param name="canExecuteEvaluator">The can execute evaluator.</param>
    public RelayCommand([DisallowNull] Action methodToExecute, Func<bool>? canExecuteEvaluator)
    {
        this.methodToExecute = methodToExecute;
        this.canExecuteEvaluator = canExecuteEvaluator;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand"/> class.
    /// </summary>
    /// <param name="methodToExecute">The method to execute.</param>
    public RelayCommand([DisallowNull] Action methodToExecute)
        : this(methodToExecute, null)
    {
    }

    /// <summary>
    /// Defines the method that determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
    /// <returns>
    ///   <see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.
    /// </returns>
    public bool CanExecute(object? parameter) => this.canExecuteEvaluator?.Invoke() ?? true;

    /// <summary>
    /// Defines the method to be called when the command is invoked.
    /// </summary>
    /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
    public void Execute(object? parameter) => this.methodToExecute.Invoke();
}
