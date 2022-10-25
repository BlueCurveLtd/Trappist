using System.Windows;

namespace InGroupe.Innovation.Wpf.Bedrock.Abstractions;

public interface IModalNavigationProvider
{
    /// <summary>
    /// Provides navigation service for "modal"
    /// </summary>
    /// <param name="viewUri">The view uri.</param>
    /// <param name="fullScreen"><c>True</c> if the model must be shown in full screen mode, otherwise <c>False</c>.</param>
    /// <param name="owner">The window owner.</param>
    void Navigate(Uri viewUri, bool fullScreen, Window owner);
    /// <summary>
    /// Close the window.
    /// </summary>
    void Close();
}
