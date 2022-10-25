
using System.Collections.Generic;

using DemoApp.Views;

using Trappist.Wpf.Bedrock.Abstractions;

using Trappist.Wpf.Bedrock;
using Trappist.Wpf.Bedrock.Command;
using Trappist.Wpf.Bedrock.Navigation;

namespace DemoApp.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private readonly INavigation navigation;

    public RelayCommand NavigateByTypeCommand { get; }

    public RelayCommand NavigateByAliasCommand { get; }

    public HomeViewModel(INavigation navigation)
    {
        this.NavigateByTypeCommand = new RelayCommand(this.NavigateByType);
        this.NavigateByAliasCommand = new RelayCommand(this.NavigateByAlias);
        this.navigation = navigation;
    }

    private void NavigateByAlias()
    {
        this.navigation.Navigate("/nav/navigated", ("NavigationType", "Alias"));
    }

    private void NavigateByType()
    {
        this.navigation.Navigate<NavigatedWith>(("NavigationType", "Type"));
    }
}
