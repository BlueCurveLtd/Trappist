using System.Windows;

namespace Trappist.Wpf.Bedrock.Abstractions;

/// <summary>
/// Navigation observer.
/// </summary>
public interface INavigationObserver
{
    void OnNext<TView>(Uri uri, string? alias) where TView : notnull, FrameworkElement;
}
