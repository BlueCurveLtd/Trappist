using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace InGroupe.Innovation.Wpf.Bedrock.ObjectModel;

public abstract class ValidableObjectBase : INotifyDataErrorInfo, INotifyPropertyChanged, INotifyPropertyChanging
{
    protected readonly ConcurrentDictionary<string, List<ValidationResult>> Errors = new();

    /// <summary>
    /// Gets a value indicating whether this object has errors.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this object has errors; otherwise, <c>false</c>.
    /// </value>
    public bool HasErrors => !this.Errors.IsEmpty;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    public event PropertyChangingEventHandler? PropertyChanging;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidableObjectBase"/> class.
    /// </summary>
    /// <param name="validateOnInitialization">if set to <c>true</c> first validation happen when object is built.</param>
    protected ValidableObjectBase(bool validateOnInitialization = false)
    {
        if (validateOnInitialization)
        {
            this.Validate();
        }
    }

    /// <summary>
    /// Raises the <see cref="E:PropertyChanged"/> event for the specified property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    protected void OnPropertyChanged([DisallowNull] string propertyName)
        => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// Raises the <see cref="E:PropertyChanged" /> event.
    /// </summary>
    /// <param name="propertyChangedEventArgs">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
    protected void OnPropertyChanged(PropertyChangedEventArgs propertyChangedEventArgs)
        => this.PropertyChanged?.Invoke(this, propertyChangedEventArgs);

    /// <summary>
    /// Raises the <see cref="E:PropertyChanging"/> event.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected void OnPropertyChanging([DisallowNull] string propertyName)
        => this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

    /// <summary>
    /// Notifies the changes in errors checking for the specified property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    protected virtual void NotifyErrorsChanged([CallerMemberName][AllowNull] string propertyName = null)
        => this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName ?? string.Empty));

    /// <summary>
    /// Sets the value of the specified field and raise <see cref="PropertyChanged"/> if a change has been detected.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <param name="value2">The value2.</param>
    /// <param name="caller">The caller.</param>
    /// <returns></returns>
    protected void SetValue<T>(ref T value, T value2, [CallerMemberName][AllowNull] string? caller = null)
    {
        if (!object.ReferenceEquals(value, value2))
        {
            value = value2;
            this.OnPropertyChanged(caller!);
            this.PropertyUpdated(caller!);
            this.Validate(value2, caller!);
        }
    }

    /// <summary>
    /// Gets the errors associated with the specified field.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>The list of errors.</returns>
    [SuppressMessage("Performance", "HLQ003:Public methods should return highest admissible level interface.", Justification = "Interface implementation.")]
    public IEnumerable GetErrors([AllowNull] string? propertyName)
    {
        if (!string.IsNullOrWhiteSpace(propertyName))
        {
            if (this.Errors.TryGetValue(propertyName!, out var errors))
            {
                return errors;
            }
        }

        return Array.Empty<ValidationResult>();
    }

    /// <summary>
    /// Occurs when the specified property has been updated.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected virtual void PropertyUpdated(string propertyName)
    {

    }

    /// <summary>
    /// Performs validation on the specified field.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> cannot be null or empty.</exception>
    protected void Validate<T>([AllowNull] T value, [CallerMemberName][AllowNull] string? propertyName = null)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        this.ClearCurrentErrors(propertyName);

        if (!this.TryValidate(value, propertyName!, out var validationErrors) && validationErrors is { Count: > 0 })
        {
            this.Errors[propertyName] = validationErrors;

            this.NotifyErrorsChanged(propertyName);
        }

        this.OnPropertyChanged(nameof(this.HasErrors));
        this.ErrorsUpdated();
    }

    /// <summary>
    /// Raised when <see cref="HasErrors"/> as been updated.
    /// </summary>
    /// <returns></returns>
    protected virtual void ErrorsUpdated() { }

    private bool TryValidate<T>([AllowNull] T value, [DisallowNull] string propertyName, out List<ValidationResult> errors)
    {
        errors = new List<ValidationResult>();

        var validationContext = new ValidationContext(this)
        {
            MemberName = propertyName
        };

        return Validator.TryValidateProperty(value, validationContext, errors);
    }        

    private void ClearCurrentErrors([AllowNull] string? propertyName)
    {
        if (propertyName is not null)
        {
            _ = this.Errors.TryRemove(propertyName, out _);
        }
    }

    /// <summary>
    /// Performs validation on all fields.
    /// </summary>
    /// <returns></returns>
    protected bool Validate()
    {
        var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var attributes = property.CustomAttributes.Where(x => x.AttributeType.IsAssignableTo(typeof(ValidationAttribute)));

            if (attributes.Any())
            {
                this.Validate(property.GetValue(this), property.Name);
            }
        }

        return !this.HasErrors;
    }

    /// <summary>
    /// Compares the current and new values for a given field (which should be the backing
    /// field for a property). If the value has changed, raises the <see cref="PropertyChanging"/>
    /// event, updates the field and then raises the <see cref="PropertyChanged"/> event.
    /// The behavior mirrors that of <see cref="SetValue{T}(ref T,T,string)"/>, with the difference being that
    /// this method will also monitor the new value of the property (a generic <see cref="Task"/>) and will also
    /// raise the <see cref="PropertyChanged"/> again for the target property when it completes.
    /// This can be used to update bindings observing that <see cref="Task"/> or any of its properties.
    /// This method and its overload specifically rely on the <see cref="TaskNotifier"/> type, which needs
    /// to be used in the backing field for the target <see cref="Task"/> property. The field doesn't need to be
    /// initialized, as this method will take care of doing that automatically. The <see cref="TaskNotifier"/>
    /// type also includes an implicit operator, so it can be assigned to any <see cref="Task"/> instance directly.
    /// Here is a sample property declaration using this method:
    /// <code>
    /// private TaskNotifier myTask;
    ///
    /// public Task MyTask
    /// {
    ///     get => myTask;
    ///     private set => SetAndNotifyOnCompletion(ref myTask, value);
    /// }
    /// </code>
    /// </summary>
    /// <param name="taskNotifier">The field notifier to modify.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// The <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> events are not raised if the current
    /// and new value for the target property are the same. The return value being <see langword="true"/> only
    /// indicates that the new value being assigned to <paramref name="taskNotifier"/> is different than the previous one,
    /// and it does not mean the new <see cref="Task"/> instance passed as argument is in any particular state.
    /// </remarks>
    protected bool SetPropertyAndNotifyOnCompletion([NotNull] ref TaskNotifier? taskNotifier, Task? newValue, [CallerMemberName] string? propertyName = null)
    {
        // We invoke the overload with a callback here to avoid code duplication, and simply pass an empty callback.
        // The lambda expression here is transformed by the C# compiler into an empty closure class with a
        // static singleton field containing a closure instance, and another caching the instantiated Action<TTask>
        // instance. This will result in no further allocations after the first time this method is called for a given
        // generic type. We only pay the cost of the virtual call to the delegate, but this is not performance critical
        // code and that overhead would still be much lower than the rest of the method anyway, so that's fine.
        return this.SetPropertyAndNotifyOnCompletion(taskNotifier ??= new(), newValue, static _ => { }, propertyName);
    }

    /// <summary>
    /// Compares the current and new values for a given field (which should be the backing
    /// field for a property). If the value has changed, raises the <see cref="PropertyChanging"/>
    /// event, updates the field and then raises the <see cref="PropertyChanged"/> event.
    /// This method is just like <see cref="SetPropertyAndNotifyOnCompletion(ref TaskNotifier,Task,string)"/>,
    /// with the difference being an extra <see cref="Action{T}"/> parameter with a callback being invoked
    /// either immediately, if the new task has already completed or is <see langword="null"/>, or upon completion.
    /// </summary>
    /// <param name="taskNotifier">The field notifier to modify.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="callback">A callback to invoke to update the property value.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// The <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> events are not raised
    /// if the current and new value for the target property are the same.
    /// </remarks>
    protected bool SetPropertyAndNotifyOnCompletion([NotNull] ref TaskNotifier? taskNotifier, Task? newValue, Action<Task?> callback, [CallerMemberName] string? propertyName = null)
        => this.SetPropertyAndNotifyOnCompletion(taskNotifier ??= new(), newValue, callback, propertyName);

    /// <summary>
    /// Compares the current and new values for a given field (which should be the backing
    /// field for a property). If the value has changed, raises the <see cref="PropertyChanging"/>
    /// event, updates the field and then raises the <see cref="PropertyChanged"/> event.
    /// The behavior mirrors that of <see cref="SetProperty{T}(ref T,T,string)"/>, with the difference being that
    /// this method will also monitor the new value of the property (a generic <see cref="Task"/>) and will also
    /// raise the <see cref="PropertyChanged"/> again for the target property when it completes.
    /// This can be used to update bindings observing that <see cref="Task"/> or any of its properties.
    /// This method and its overload specifically rely on the <see cref="TaskNotifier{T}"/> type, which needs
    /// to be used in the backing field for the target <see cref="Task"/> property. The field doesn't need to be
    /// initialized, as this method will take care of doing that automatically. The <see cref="TaskNotifier{T}"/>
    /// type also includes an implicit operator, so it can be assigned to any <see cref="Task"/> instance directly.
    /// Here is a sample property declaration using this method:
    /// <code>
    /// private TaskNotifier&lt;int&gt; myTask;
    ///
    /// public Task&lt;int&gt; MyTask
    /// {
    ///     get => myTask;
    ///     private set => SetAndNotifyOnCompletion(ref myTask, value);
    /// }
    /// </code>
    /// </summary>
    /// <typeparam name="T">The type of result for the <see cref="Task{TResult}"/> to set and monitor.</typeparam>
    /// <param name="taskNotifier">The field notifier to modify.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// The <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> events are not raised if the current
    /// and new value for the target property are the same. The return value being <see langword="true"/> only
    /// indicates that the new value being assigned to <paramref name="taskNotifier"/> is different than the previous one,
    /// and it does not mean the new <see cref="Task{TResult}"/> instance passed as argument is in any particular state.
    /// </remarks>
    protected bool SetPropertyAndNotifyOnCompletion<T>([NotNull] ref TaskNotifier<T>? taskNotifier, Task<T>? newValue, [CallerMemberName] string? propertyName = null)
        => this.SetPropertyAndNotifyOnCompletion(taskNotifier ??= new(), newValue, static _ => { }, propertyName);

    /// <summary>
    /// Compares the current and new values for a given field (which should be the backing
    /// field for a property). If the value has changed, raises the <see cref="PropertyChanging"/>
    /// event, updates the field and then raises the <see cref="PropertyChanged"/> event.
    /// This method is just like <see cref="SetPropertyAndNotifyOnCompletion{T}(ref TaskNotifier{T},Task{T},string)"/>,
    /// with the difference being an extra <see cref="Action{T}"/> parameter with a callback being invoked
    /// either immediately, if the new task has already completed or is <see langword="null"/>, or upon completion.
    /// </summary>
    /// <typeparam name="T">The type of result for the <see cref="Task{TResult}"/> to set and monitor.</typeparam>
    /// <param name="taskNotifier">The field notifier to modify.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="callback">A callback to invoke to update the property value.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// The <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> events are not raised
    /// if the current and new value for the target property are the same.
    /// </remarks>
    protected bool SetPropertyAndNotifyOnCompletion<T>([NotNull] ref TaskNotifier<T>? taskNotifier, Task<T>? newValue, Action<Task<T>?> callback, [CallerMemberName] string? propertyName = null)
        => this.SetPropertyAndNotifyOnCompletion(taskNotifier ??= new(), newValue, callback, propertyName);

    /// <summary>
    /// Implements the notification logic for the related methods.
    /// </summary>
    /// <typeparam name="TTask">The type of <see cref="Task"/> to set and monitor.</typeparam>
    /// <param name="taskNotifier">The field notifier.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="callback">A callback to invoke to update the property value.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    private bool SetPropertyAndNotifyOnCompletion<TTask>(ITaskNotifier<TTask> taskNotifier, TTask? newValue, Action<TTask?> callback, [CallerMemberName] string? propertyName = null)
        where TTask : Task
    {
        if (ReferenceEquals(taskNotifier.Task, newValue))
        {
            return false;
        }

        // Check the status of the new task before assigning it to the
        // target field. This is so that in case the task is either
        // null or already completed, we can avoid the overhead of
        // scheduling the method to monitor its completion.
        bool isAlreadyCompletedOrNull = newValue?.IsCompleted ?? true;

        this.OnPropertyChanging(propertyName!);

        taskNotifier.Task = newValue;

        this.OnPropertyChanged(propertyName!);

        // If the input task is either null or already completed, we don't need to
        // execute the additional logic to monitor its completion, so we can just bypass
        // the rest of the method and return that the field changed here. The return value
        // does not indicate that the task itself has completed, but just that the property
        // value itself has changed (ie. the referenced task instance has changed).
        // This mirrors the return value of all the other synchronous Set methods as well.
        if (isAlreadyCompletedOrNull)
        {
            callback(newValue);

            return true;
        }

        // We use a local async function here so that the main method can
        // remain synchronous and return a value that can be immediately
        // used by the caller. This mirrors Set<T>(ref T, T, string).
        // We use an async void function instead of a Task-returning function
        // so that if a binding update caused by the property change notification
        // causes a crash, it is immediately reported in the application instead of
        // the exception being ignored (as the returned task wouldn't be awaited),
        // which would result in a confusing behavior for users.
        async void MonitorTask()
        {
            try
            {
                // Await the task and ignore any exceptions
                await newValue!;
            }
            catch
            {
            }

            // Only notify if the property hasn't changed
            if (ReferenceEquals(taskNotifier.Task, newValue))
            {
                this.OnPropertyChanged(propertyName!);
            }

            callback(newValue);
        }

        MonitorTask();

        return true;
    }

    /// <summary>
    /// An interface for task notifiers of a specified type.
    /// </summary>
    /// <typeparam name="TTask">The type of value to store.</typeparam>
    private interface ITaskNotifier<TTask>
        where TTask : Task
    {
        /// <summary>
        /// Gets or sets the wrapped <typeparamref name="TTask"/> value.
        /// </summary>
        TTask? Task { get; set; }
    }

    /// <summary>
    /// A wrapping class that can hold a <see cref="Task"/> value.
    /// </summary>
    protected sealed class TaskNotifier : ITaskNotifier<Task>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskNotifier"/> class.
        /// </summary>
        internal TaskNotifier()
        {
        }

        private Task? task;

        /// <inheritdoc/>
        Task? ITaskNotifier<Task>.Task
        {
            get => this.task;
            set => this.task = value;
        }

        /// <summary>
        /// Unwraps the <see cref="Task"/> value stored in the current instance.
        /// </summary>
        /// <param name="notifier">The input <see cref="TaskNotifier{TTask}"/> instance.</param>
        [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Implicit conversion")]
        public static implicit operator Task?(TaskNotifier? notifier)
        {
            return notifier?.task;
        }

        [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>")]

        public Task? ToTask() => this.task;
    }


    /// <summary>
    /// A wrapping class that can hold a <see cref="Task{T}"/> value.
    /// </summary>
    /// <typeparam name="T">The type of value for the wrapped <see cref="Task{T}"/> instance.</typeparam>
    protected sealed class TaskNotifier<T> : ITaskNotifier<Task<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskNotifier{TTask}"/> class.
        /// </summary>
        internal TaskNotifier()
        {
        }

        private Task<T>? task;

        /// <inheritdoc/>
        Task<T>? ITaskNotifier<Task<T>>.Task
        {
            get => this.task;
            set => this.task = value;
        }

        /// <summary>
        /// Unwraps the <see cref="Task{T}"/> value stored in the current instance.
        /// </summary>
        /// <param name="notifier">The input <see cref="TaskNotifier{TTask}"/> instance.</param>
        public static implicit operator Task<T>?(TaskNotifier<T>? notifier)
        {
            return notifier?.task;
        }


        [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>")]

        public Task? ToTask() => this.task;
    }
}
