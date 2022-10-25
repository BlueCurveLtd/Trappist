using System.Diagnostics.CodeAnalysis;

namespace Trappist.Wpf.Bedrock.Abstractions;

/// <summary>
/// Navigation awareness contract.
/// </summary>
public interface INavigationAware
{
    /// <summary>
    /// Navigates to the current view.
    /// </summary>
    /// <param name="navigationParameters">The navigation parameters.</param>
    void NavigateTo([DisallowNull] INavigationParameters navigationParameters);

    /// <summary>
    /// Navigates from the current view.
    /// </summary>
    void NavigateFrom([DisallowNull] INavigationParameters navigationParameters);
}
