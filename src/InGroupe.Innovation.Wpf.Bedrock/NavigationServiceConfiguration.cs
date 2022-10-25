namespace InGroupe.Innovation.Wpf.Bedrock;

public sealed class NavigationServiceConfiguration
{
    internal NavigationModels NavigationModel { get; set; } = NavigationModels.NavigationWindow;

    /// <summary>
    /// Uses the frame navigation model.
    /// </summary>
    /// <remarks>The navigation frame must be named <code>Bedrock_NavigationFrame</code></remarks>
    /// <returns><see cref="NavigationServiceConfiguration"/></returns>
    public NavigationServiceConfiguration UseFrameNavigationModel()
    {
        this.NavigationModel = NavigationModels.Frame;

        return this;
    }

    /// <summary>
    /// Uses the navigation window navigation model.
    /// </summary>
    /// <returns><see cref="NavigationServiceConfiguration"/></returns>
    public NavigationServiceConfiguration UseNavigationWindowNavigationModel()
    {
        this.NavigationModel = NavigationModels.NavigationWindow;

        return this;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationServiceConfiguration"/> class.
    /// </summary>
    internal NavigationServiceConfiguration()
    {

    }
}

/// <summary>
/// The navigation models
/// </summary>
public enum NavigationModels
{
    /// <summary>
    /// The navigation window model
    /// </summary>
    NavigationWindow = 0,
    /// <summary>
    /// The frame navigation model
    /// </summary>
    Frame = 1
}
