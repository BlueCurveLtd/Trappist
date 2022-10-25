using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using InGroupe.Innovation.Wpf.Bedrock.Abstractions;

namespace InGroupe.Innovation.Wpf.Bedrock.Navigation;

[DebuggerDisplay("IsEmpty = {IsEmpty}")]
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(NavigationParametersDebuggerProxy))]
public sealed class NavigationParameters : INavigationParameters
{
    private readonly Dictionary<string, object?> parameters = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationParameters"/> class.
    /// </summary>
    private NavigationParameters() { }

    public static readonly NavigationParameters Instance = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationParameters"/> class.
    /// </summary>
    /// <param name="navigationParameters">The navigation parameters.</param>
    /// <exception cref="ArgumentNullException"><paramref name="navigationParameters"/></exception>
    public NavigationParameters([DisallowNull] (string Key, object? Value)[] navigationParameters)
        => this.parameters = navigationParameters.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationParameters"/> class.
    /// </summary>
    /// <param name="navigationParameters">The navigation parameters.</param>
    internal NavigationParameters([DisallowNull] NavigationParameters navigationParameters)
        => this.parameters = new Dictionary<string, object?>(navigationParameters.parameters, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the <see cref="object"/> with the specified key.
    /// </summary>
    /// <value>
    /// The <see cref="object"/>.
    /// </value>
    /// <param name="key">The key.</param>
    /// <returns>The value.</returns>
    [MaybeNull]
    public object? this[[DisallowNull] string key]
    {
        get => this.parameters.TryGetValue(key, out var value) ? value : default;
        set => this.parameters[key] = value;
    }

    /// <summary>
    /// Gets a value indicating whether this instance is empty.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
    /// </value>
    public bool IsEmpty => this.parameters.Count == 0;
    /// <summary>
    /// Gets a value indicating the number of elements in the bad.
    /// </summary>
    public int Count => this.parameters.Count;
    /// <summary>
    /// Gets the keys.
    /// </summary>
    public IImmutableList<string> Keys => this.parameters.Keys.ToImmutableList();
    /// <summary>
    /// Gets the parameters as a immutable dictionary.
    /// </summary>
    /// <returns>The parameters.</returns>
    public IImmutableDictionary<string, object?> AsDictionary() => this.parameters.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Adds the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>The navigation parameters.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/></exception>
    [return: NotNull]
    public INavigationParameters AddOrUpdate([DisallowNull] string key, [AllowNull] object? value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        this.parameters[key] = value;

        return this;
    }

    /// <summary>
    /// Gets the specified key.
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    /// <param name="key">The key.</param>
    /// <returns>Value of {T} if the key exists, otherwise <c>default(T)</c></returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/></exception>
    [return: MaybeNull]
    public T? Get<T>([DisallowNull] string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (this.parameters.TryGetValue(key, out var value) && value is not null)
        {
            return (T)value;
        }

        return default;
    }

    /// <summary>
    /// Removes the value with the specified key.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> cannot be null or empty.</exception>
    public void Remove([DisallowNull] string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        this.parameters.Remove(key);
    }

    /// <summary>
    /// Clear the navigation parameters.
    /// </summary>
    public void Clear() => this.parameters.Clear();

    /// <summary>
    /// Merges the specified entries into the <see cref="NavigationParameters"/>.
    /// </summary>
    /// <param name="mergeParameters">The merge parameters.</param>
    /// <returns>The merged <see cref="NavigationParameters"/></returns>
    [return: NotNull]
    public INavigationParameters Merge([DisallowNull] IReadOnlyDictionary<string, object?> mergeParameters)
    {
        if (mergeParameters is null)
        {
            throw new NavigationParametersMergeException("The merge parameters cannot be null");
        }

        foreach (var key in mergeParameters.Keys)
        {
            this.parameters[key] = mergeParameters[key];
        }

        return this;
    }

    /// <summary>
    /// Merges the specified entries into the <see cref="NavigationParameters"/>.
    /// </summary>
    /// <param name="mergeParameters">The merge parameters.</param>
    /// <returns>The merged <see cref="NavigationParameters"/></returns>
    [return: NotNull]
    public INavigationParameters Merge([DisallowNull] params (string Key, object? Value)[] mergeParameters)
    {
        if (mergeParameters is null)
        {
            throw new NavigationParametersMergeException("The merge parameters cannot be null");
        }

        foreach (var (key, value) in mergeParameters)
        {
            this.parameters[key] = value;
        }

        return this;
    }

    internal sealed class NavigationParametersDebuggerProxy
    {
        public List<(string Key, object? Value)> Parameters { get; }

        public NavigationParametersDebuggerProxy(NavigationParameters navigationParameters)
            => this.Parameters = navigationParameters.parameters.Select(x => (x.Key, x.Value)).ToList();
    }
}
