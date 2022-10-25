namespace Trappist.Wpf.Bedrock.Abstractions;

public interface IConfirmNavigation
{
    bool CanNavigate(INavigationParameters parameters);
}
