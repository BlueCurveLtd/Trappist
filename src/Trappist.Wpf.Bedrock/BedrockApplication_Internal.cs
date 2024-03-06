using System;
using System.Diagnostics;
using System.Windows;

using ConfigurationSubstitution;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

using Trappist.Wpf.Bedrock.Abstractions;
using Trappist.Wpf.Bedrock.InstanceControl;

namespace Trappist.Wpf.Bedrock;

public abstract partial class TrappistApplication
{
    private void ConfigureServicesInternal(HostBuilderContext hostBuilderContext, IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddSingleton(ServiceLocator.Initialize);

        this.ConfigureServices(hostBuilderContext, serviceCollection);

        this.RegisterNavigationService(serviceCollection);

        this.RegisterMasterPage(serviceCollection);
    }

    private void ConfigureAppConfigurationInternal(HostBuilderContext hostBuilderContext, IConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .AddEnvironmentVariables()
            .AddJsonFile($"appsettings.json", false, true)
            .AddJsonFile($"appsettings.{hostBuilderContext.HostingEnvironment.EnvironmentName}.json", true, true);

        foreach (var environment in this.GetEnvironments(hostBuilderContext))
        {
            var value = Environment.GetEnvironmentVariable(environment.Name, environment.Target);

            if (!string.IsNullOrWhiteSpace(value))
            {
                configurationBuilder.AddJsonFile($"appsettings.{value}.json", false, true);
            }
        }

        this.ConfigureAppConfiguration(hostBuilderContext, configurationBuilder);

        configurationBuilder.EnableSubstitutions("%", "%");
    }

    private void ConfigureHostConfigurationInternal(IConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.SetBasePath(AppContext.BaseDirectory);

        this.ConfigureHostConfiguration(configurationBuilder);
    }
             
    protected sealed override void OnStartup(StartupEventArgs e)
    {
        this.RegisterForNavigation(Container.GetService<INavigationRegistry>()!);

        var singleInstanceApp = Container.GetService<ISingleInstanceApp>();

        if (singleInstanceApp is not null && singleInstanceApp.AlreadyStarted())
        {
            if (singleInstanceApp is MachineSingleInstance machineSingleInstance)
            {
                machineSingleInstance.Release();
            }

            Process.GetCurrentProcess().Kill();
        }
        else
        {
            this.BeforeStartup(Container);

            _ = Host.RunAsync();

            this.AfterStartup(Container.GetService<INavigation>()!);
        }
    }         

    protected sealed override void OnExit(ExitEventArgs e)
    {
        var singleInstanceApp = Container.GetService<ISingleInstanceApp>();

        singleInstanceApp?.Release();

        this.AppExit();

        if (Host?.Services is not null)
        {
            var hostApplicationLifetime = Host!.Services.GetService<IHostApplicationLifetime>()!;
            hostApplicationLifetime.StopApplication();
        }
        else
        {
            Environment.Exit(0);
        }
    }
}
