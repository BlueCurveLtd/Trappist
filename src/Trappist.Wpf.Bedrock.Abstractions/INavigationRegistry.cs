using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace Trappist.Wpf.Bedrock.Abstractions;

/// <summary>
/// Navigation registry contract
/// </summary>
public interface INavigationRegistry
{
    /// <summary>
    /// Registers the specified page to the navigation service.
    /// </summary>
    /// <typeparam name="TPage">The type of the page.</typeparam>
    void RegisterForNavigation<TPage>()
        where TPage : notnull, FrameworkElement;

    /// <summary>
    /// Registers the specified page to the navigation service.
    /// </summary>
    /// <typeparam name="TPage">The type of the page.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    void RegisterForNavigation<TPage, TViewModel>()
        where TPage : notnull, FrameworkElement
        where TViewModel : class;

    /// <summary>
    /// Registers the specified page to the navigation service.
    /// </summary>
    /// <typeparam name="TPage">The type of the page.</typeparam>
    /// <param name="alias">The page alias.</param>
    /// <exception cref="ArgumentNullException"><paramref name="alias"/></exception>
    void RegisterForNavigation<TPage>([DisallowNull] string alias)
        where TPage : notnull, FrameworkElement;

    /// <summary>
    /// Registers the specified page to the navigation service.
    /// </summary>
    /// <typeparam name="TPage">The type of the page.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <param name="alias">The page alias.</param>
    /// <exception cref="ArgumentNullException"><paramref name="alias"/></exception>
    void RegisterForNavigation<TPage, TViewModel>([DisallowNull] string alias)
        where TPage : notnull, FrameworkElement
        where TViewModel : class;
}
