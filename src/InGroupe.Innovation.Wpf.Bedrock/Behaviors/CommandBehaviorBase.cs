using System;
using System.Windows.Input;
using System.Diagnostics.CodeAnalysis;

#if HAS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#elif HAS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using System.Windows;
using System.Windows.Controls;
#endif

namespace InGroupe.Innovation.Wpf.Bedrock.Behaviors;

/// <summary>
/// Base behavior to handle connecting a <see cref="Control"/> to a Command.
/// </summary>
/// <typeparam name="T">The target object must derive from Control.</typeparam>
/// <remarks>
/// CommandBehaviorBase can be used to provide new behaviors for commands.
/// </remarks>
public class CommandBehaviorBase<T> where T : UIElement
{
    private ICommand? command;
    private object? commandParameter;
    private bool autoEnabled = true;
    private readonly WeakReference targetObject; 

    /// <summary>
    /// Constructor specifying the target object.
    /// </summary>
    /// <param name="targetObject">The target object the behavior is attached to.</param>
    public CommandBehaviorBase([DisallowNull] T targetObject)
    {
        this.targetObject = new WeakReference(targetObject); 
    }

    /// <summary>
    /// If <c>true</c> the target object's IsEnabled property will update based on the commands ability to execute.
    /// If <c>false</c> the target object's IsEnabled property will not update.
    /// </summary>
    public bool AutoEnable
    {
        get => this.autoEnabled;
        set
        {
            this.autoEnabled = value;
            this.UpdateEnabledState();
        }
    }

    /// <summary>
    /// Corresponding command to be execute and monitored for <see cref="ICommand.CanExecuteChanged"/>.
    /// </summary>        
    [MaybeNull]
    [AllowNull]
    public ICommand? Command
    {
        get => this.command;
        set
        {
            if (this.command is not null)
            {
                this.command.CanExecuteChanged -= this.CommandCanExecuteChanged;
            }

            this.command = value;

            if (this.command is not null)
            {
                this.command.CanExecuteChanged += this.CommandCanExecuteChanged;
                this.UpdateEnabledState();
            }
        }
    }

    /// <summary>
    /// The parameter to supply the command during execution.
    /// </summary>
    [MaybeNull]
    [AllowNull]
    public object? CommandParameter
    {
        get => this.commandParameter;
        set
        {
            if (this.commandParameter != value)
            {
                this.commandParameter = value;
                this.UpdateEnabledState();
            }
        }
    }

    /// <summary>
    /// Object to which this behavior is attached.
    /// </summary>
    [MaybeNull]
    protected T? TargetObject => this.targetObject.Target as T;


    /// <summary>
    /// Updates the target object's IsEnabled property based on the commands ability to execute.
    /// </summary>
    protected virtual void UpdateEnabledState()
    {
        if (this.TargetObject is null)
        {
            this.Command = null;
            this.CommandParameter = null;
        }
        else if (this.Command is not null)
        {
#if HAS_UWP || HAS_WINUI
            if (this.AutoEnable && this.TargetObject is Control control) {
                this.control.IsEnabled = this.Command.CanExecute(this.CommandParameter);
            }
#else
            if (this.AutoEnable)
            {
                this.TargetObject.IsEnabled = this.Command.CanExecute(this.CommandParameter);
            }
#endif
        }
    }

    private void CommandCanExecuteChanged(object? sender, EventArgs e) => this.UpdateEnabledState();

    /// <summary>
    /// Executes the command, if it's set, providing the <see cref="CommandParameter"/>.
    /// </summary>
    protected virtual void ExecuteCommand(object? parameter)
    {
        if (this.Command is not null)
        {
            this.Command.Execute(this.CommandParameter ?? parameter);
        }
    }
}
