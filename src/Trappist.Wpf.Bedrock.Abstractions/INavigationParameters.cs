using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Trappist.Wpf.Bedrock.Abstractions;

public interface INavigationParameters
{
    /// <summary>
    /// Gets the <see cref="object"/> with the specified key.
    /// </summary>
    /// <value>
    /// The <see cref="object"/>.
    /// </value>
    /// <param name="key">The key.</param>
    /// <returns>The value.</returns>
    [MaybeNull]
    object? this[[DisallowNull] string key] { get; set; }
    /// <summary>
    /// Gets a value indicating the number of elements in the bad.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Gets a value indicating whether this instance is empty.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
    /// </value>
    bool IsEmpty { get; }
    /// <summary>
    /// Gets the keys.
    /// </summary>
    IImmutableList<string> Keys { get; }
    /// <summary>
    /// Adds the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>The navigation parameters.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/></exception>
    [return: NotNull]
    INavigationParameters AddOrUpdate([DisallowNull] string key, [AllowNull] object? value);
    /// <summary>
    /// Gets the parameters as a immutable dictionary.
    /// </summary>
    /// <returns>The parameters.</returns>
    IImmutableDictionary<string, object?> AsDictionary();
    /// <summary>
    /// Clear the navigation parameters.
    /// </summary>
    void Clear();
    T? Get<T>([DisallowNull] string key);
    /// <summary>
    /// Merges the specified entries into the <see cref="INavigationParameters"/>.
    /// </summary>
    /// <param name="mergeParameters">The merge parameters.</param>
    /// <returns>The merged <see cref="INavigationParameters"/></returns>
    [return: NotNull]
    INavigationParameters Merge([DisallowNull] IReadOnlyDictionary<string, object?> mergeParameters);
    /// <summary>
    /// Merges the specified entries into the <see cref="INavigationParameters"/>.
    /// </summary>
    /// <param name="mergeParameters">The merge parameters.</param>
    /// <returns>The merged <see cref="INavigationParameters"/></returns>
    [return: NotNull]
    INavigationParameters Merge([DisallowNull] params (string Key, object? Value)[] mergeParameters);
    /// <summary>
    /// Removes the value with the specified key.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> cannot be null or empty.</exception>
    void Remove([DisallowNull] string key);
}
