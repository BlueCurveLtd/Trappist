using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

using Microsoft.Xaml.Behaviors;

namespace Trappist.Wpf.Bedrock.Behaviors;

public class EventToCommandBehavior : Behavior<UIElement>
{
    /// <summary>
    /// Bindable property for Name of the event that will be forwarded to 
    /// <see cref="Command"/>
    /// </summary>
    public static readonly DependencyProperty EventNameProperty =
         DependencyProperty.RegisterAttached(nameof(EventName), typeof(string), typeof(EventToCommandBehavior));

    /// <summary>
    /// Bindable property for Command to execute
    /// </summary>
    public static readonly DependencyProperty CommandProperty =
         DependencyProperty.RegisterAttached(nameof(Command), typeof(ICommand), typeof(EventToCommandBehavior));

    /// <summary>
    /// Bindable property for Argument sent to <see cref="ICommand.Execute(object)"/>
    /// </summary>
    public static readonly DependencyProperty CommandParameterProperty =
         DependencyProperty.RegisterAttached(nameof(CommandParameter), typeof(object), typeof(EventToCommandBehavior));

    /// <summary>
    /// Bindable property to set instance of <see cref="IValueConverter" /> to convert the <see cref="EventArgs" /> for <see cref="EventName" />
    /// </summary>
    public static readonly DependencyProperty EventArgsConverterProperty =
         DependencyProperty.RegisterAttached(nameof(EventArgsConverter), typeof(IValueConverter), typeof(EventToCommandBehavior));

    /// <summary>
    /// Bindable property to set Argument passed as parameter to <see cref="IValueConverter.Convert" />
    /// </summary>
    public static readonly DependencyProperty EventArgsConverterParameterProperty =
         DependencyProperty.RegisterAttached(nameof(EventArgsConverterParameter), typeof(object), typeof(EventToCommandBehavior));

    /// <summary>
    /// Bindable property to set Parameter path to extract property from <see cref="EventArgs"/> instance to pass to <see cref="ICommand.Execute"/>
    /// </summary>
    public static readonly DependencyProperty EventArgsParameterPathProperty =
         DependencyProperty.RegisterAttached(
            nameof(EventArgsParameterPath),
            typeof(string),
            typeof(EventToCommandBehavior));


    /// <summary>
    /// Bindable property to set Parameter path to extract property from <see cref="EventArgs"/> instance to pass to <see cref="ICommand.Execute"/>
    /// </summary>
    public static readonly DependencyProperty FromDataContextProperty =
         DependencyProperty.RegisterAttached(
            nameof(FromDataContext),
            typeof(bool),
            typeof(EventToCommandBehavior));

    /// <summary>
    /// <see cref="System.Reflection.EventInfo"/>
    /// </summary>
    protected EventInfo? EventInfo;

    /// <summary>
    /// Delegate to Invoke when event is raised
    /// </summary>
    protected Delegate? Handler;

    /// <summary>
    /// Parameter path to extract property from <see cref="EventArgs"/> instance to pass to <see cref="ICommand.Execute"/>
    /// </summary>
    [MaybeNull]
    [AllowNull]
    public string? EventArgsParameterPath
    {
        get => this.GetValue(EventArgsParameterPathProperty) as string;
        set => this.SetValue(EventArgsParameterPathProperty, value);
    }

    /// <summary>
    /// Name of the event that will be forwarded to <see cref="Command" />
    /// </summary>
    /// <remarks>
    /// An event that is invalid for the attached <see cref="View" /> will result in <see cref="ArgumentException" /> thrown.
    /// </remarks>
    [MaybeNull]
    [AllowNull]
    public string? EventName
    {
        get => this.GetValue(EventNameProperty) as string;
        set => this.SetValue(EventNameProperty, value);
    }

    /// <summary>
    /// The command to execute
    /// </summary>
    [MaybeNull]
    [AllowNull]
    public ICommand? Command
    {
        get => this.GetValue(CommandProperty) as ICommand;
        set => this.SetValue(CommandProperty, value);
    }

    /// <summary>
    /// Argument sent to <see cref="ICommand.Execute" />
    /// </summary>
    /// <para>
    /// If <see cref="EventArgsConverter" /> and <see cref="EventArgsConverterParameter" /> is set then the result of the
    /// conversion
    /// will be sent.
    /// </para>
    [MaybeNull]
    [AllowNull]
    public object? CommandParameter
    {
        get => this.GetValue(CommandParameterProperty);
        set => this.SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// Instance of <see cref="IValueConverter" /> to convert the <see cref="EventArgs" /> for <see cref="EventName" />
    /// </summary>
    [MaybeNull]
    [AllowNull]
    public IValueConverter? EventArgsConverter
    {
        get => this.GetValue(EventArgsConverterProperty) as IValueConverter;
        set => this.SetValue(EventArgsConverterProperty, value);
    }

    /// <summary>
    /// Argument passed as parameter to <see cref="IValueConverter.Convert" />
    /// </summary>
    [MaybeNull]
    [AllowNull]
    public object? EventArgsConverterParameter
    {
        get => this.GetValue(EventArgsConverterParameterProperty);
        set => this.SetValue(EventArgsConverterParameterProperty, value);
    }

    public bool FromDataContext
    {
        get => (bool)this.GetValue(FromDataContextProperty);
        set => this.SetValue(FromDataContextProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        if (this.FromDataContext)
        {
            this.EventInfo =
                this.AssociatedObject.GetType()
                .GetProperty(FrameworkElement.DataContextProperty.Name)!
                .GetValue(this.AssociatedObject)?
                .GetType()?.GetEvent(this.EventName!);
        }
        else
        {
            this.EventInfo = this.AssociatedObject.GetType().GetRuntimeEvent(this.EventName!);
        }



        if (this.EventInfo is null)
        {
            throw new ArgumentException(
                $"No matching event '{this.EventName}' on attached type '{this.AssociatedObject.GetType().Name}'");
        }



        if (this.FromDataContext)
        {
            this.AddEventHandler(
                this.EventInfo, this.AssociatedObject.GetType().GetProperty(FrameworkElement.DataContextProperty.Name)!.GetValue(this.AssociatedObject)!,
                this.OnEventRaised);
        }
        else
        {
            this.AddEventHandler(this.EventInfo, this.AssociatedObject, this.OnEventRaised);
        }
    }

    protected override void OnDetaching()
    {
        if (this.Handler is not null)
        {
            this.EventInfo!.RemoveEventHandler(this.AssociatedObject, this.Handler);
        }

        this.Handler = null;
        this.EventInfo = null;

        base.OnDetaching();
    }

    private void AddEventHandler(EventInfo eventInfo, object item, Action<object, EventArgs> action)
    {
        var eventParameters = eventInfo.EventHandlerType!
            .GetRuntimeMethods().First(m => string.Equals(m.Name, "Invoke", StringComparison.Ordinal))
            .GetParameters()
            .Select(p => System.Linq.Expressions.Expression.Parameter(p.ParameterType))
            .ToArray();

        var actionInvoke = action.GetType()
            .GetRuntimeMethods().First(m => string.Equals(m.Name, "Invoke", StringComparison.Ordinal));

        this.Handler = System.Linq.Expressions.Expression.Lambda(
            eventInfo.EventHandlerType!,
            System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.Constant(action), actionInvoke, eventParameters[0], eventParameters[1]),
            eventParameters)
            .Compile();

        eventInfo.AddEventHandler(item, this.Handler);
    }

    /// <summary>
    /// Method called when event is raised
    /// </summary>
    /// <param name="sender">Source of that raised the event</param>
    /// <param name="eventArgs">Arguments of the raised event</param>
    protected virtual void OnEventRaised([DisallowNull] object sender, [DisallowNull] EventArgs eventArgs)
    {
        if (this.Command is null)
        {
            return;
        }

        var parameter = this.CommandParameter;

        if (parameter is null && !string.IsNullOrEmpty(this.EventArgsParameterPath))
        {
            //Walk the ParameterPath for nested properties.
            var propertyPathParts = this.EventArgsParameterPath.Split('.');
            object? propertyValue = eventArgs;

            foreach (var propertyPathPart in propertyPathParts)
            {
                var propInfo = propertyValue?.GetType()?.GetRuntimeProperty(propertyPathPart);

                if (propInfo is null)
                {
                    throw new MissingMemberException($"Unable to find {this.EventArgsParameterPath}");
                }

                propertyValue = propInfo!.GetValue(propertyValue);
                if (propertyValue is null)
                {
                    break;
                }
            }
            parameter = propertyValue;
        }

        if (parameter is null && eventArgs is not null && eventArgs != EventArgs.Empty && this.EventArgsConverter is not null)
        {
            parameter = this.EventArgsConverter.Convert(eventArgs, typeof(object), this.EventArgsConverterParameter, CultureInfo.CurrentUICulture);
        }

        if (this.Command.CanExecute(parameter))
        {
            this.Command.Execute(parameter);
        }
    }
}
