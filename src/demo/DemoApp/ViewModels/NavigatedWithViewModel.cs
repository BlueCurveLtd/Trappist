using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

using DemoApp.Views;

using Trappist.Wpf.Bedrock;
using Trappist.Wpf.Bedrock.Abstractions;
using Trappist.Wpf.Bedrock.Command;
using Trappist.Wpf.Bedrock.Command.Abstractions;

namespace DemoApp.ViewModels
{
    public class NavigatedWithViewModel : ViewModelBase, INavigationAware
    {
        public string? NavigationType { get; private set; }


        public ICommand GoBack { get; }

        public NavigatedWithViewModel(INavigation navigation)
        {
            this.GoBack = new RelayCommand(() =>
            {
                navigation.Navigate<Home>();
            });
        }


        public void NavigateFrom([DisallowNull] INavigationParameters navigationParameters)
        {
             
        }

        public void NavigateTo([DisallowNull] INavigationParameters navigationParameters)
        {
            this.NavigationType = $"Type of navigation used: {navigationParameters.Get<string>("NavigationType")!}";
        }
    }
}
