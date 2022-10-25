using System.Diagnostics.CodeAnalysis;

using InGroupe.Innovation.Wpf.Bedrock;
using InGroupe.Innovation.Wpf.Bedrock.Abstractions;
using InGroupe.Innovation.Wpf.Bedrock.Navigation;

namespace DemoApp.ViewModels
{
    public class NavigatedWithViewModel : ViewModelBase, INavigationAware
    {
        public string? NavigationType { get; private set; }

        public void NavigateFrom([DisallowNull] NavigationParameters navigationParameters)
        {
             
        }

        public void NavigateTo([DisallowNull] NavigationParameters navigationParameters)
        {
            this.NavigationType = $"Type of navigation used: {navigationParameters.Get<string>("NavigationType")!}";
        }
    }
}
