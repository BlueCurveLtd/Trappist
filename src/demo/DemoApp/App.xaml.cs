    using InGroupe.Innovation.Wpf.Bedrock;
using InGroupe.Innovation.Wpf.Bedrock.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DemoApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : BedrockApplication
    {
        public App()
            : base("TestDemo")
        {
        }

        protected override void RegisterNavigationService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddNavigationService(configure => configure.UseFrameNavigationModel());
        }

        protected override void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection serviceCollection)
        {
        }

        protected override void RegisterForNavigation(INavigationRegistry navigationRegistry)
        {
            navigationRegistry.RegisterForNavigation<Views.Home>("/home");
            navigationRegistry.RegisterForNavigation<Views.NavigatedWith>("/nav/navigated");
        }

        protected override void AfterStartup(INavigation navigationService)
        {
            navigationService.Navigate<Views.Home>();
        }
    }
}
