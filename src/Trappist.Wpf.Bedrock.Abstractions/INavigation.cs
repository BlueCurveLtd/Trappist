using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace Trappist.Wpf.Bedrock.Abstractions;

/// <summary>
/// Navigation service contract.
/// </summary>
public interface INavigation
{
    /// <summary>
    /// Navigates to the specified page.
    /// </summary>
    /// <typeparam name="TView">The view.</typeparam>
    /// <param name="canRefreshIfSameView"><c>True</c> to allow refresh of the current view if we re-navigated to itself.</param>
    void Navigate<TView>(bool canRefreshIfSameView = false) where TView : notnull, FrameworkElement;

    /// <summary>
    /// Navigates to the specified view.
    /// </summary>
    /// <param name="alias">The view alias.</param>
    /// <param name="canRefreshIfSameView"><c>True</c> to allow refresh of the current view if we re-navigated to itself.</param>
    /// <exception cref="ArgumentException">The view alias cannot be null, empty or white spaces., <paramref name="alias"/></exception>
    void Navigate([DisallowNull] string alias, bool canRefreshIfSameView = false);

    /// <summary>
    /// Navigates to the specified view.
    /// </summary>
    /// <param name="viewType">The type of the view.</param>
    /// <param name="canRefreshIfSameView"><c>True</c> to allow refresh of the current view if we re-navigated to itself.</param> 
    /// <exception cref="ArgumentNullException">The view type cannot be null. <paramref name="viewType"/></exception>
    void Navigate([DisallowNull] Type viewType, bool canRefreshIfSameView = false);
}
