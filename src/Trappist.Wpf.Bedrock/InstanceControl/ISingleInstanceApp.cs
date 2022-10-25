namespace Trappist.Wpf.Bedrock.InstanceControl;

/// <summary>
/// Contract for single app handling.
/// </summary>
public interface ISingleInstanceApp
{
    /// <summary>
    /// Checks if the application is already started.
    /// </summary>
    /// <returns><c>true</c> if the application is already started, <c>false</c> otherwise.</returns>
    bool AlreadyStarted();
    /// <summary>
    /// Release.
    /// </summary>
    void Release();
}
