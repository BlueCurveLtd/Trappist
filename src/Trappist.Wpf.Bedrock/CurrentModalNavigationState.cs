namespace Trappist.Wpf.Bedrock;

internal static class CurrentModalNavigationState
{
    public static object? CurrentView { get; private set; }

    public static object? CurrentViewModel { get; private set; }

    public static void Update(object? currentView, object? currentViewModel)
    {
        CurrentView = currentView;
        CurrentViewModel = currentViewModel;
    }
}
