using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using InGroupe.Innovation.Wpf.Bedrock.Abstractions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InGroupe.Innovation.Wpf.Bedrock;

/// <summary>
/// Base bedrock application model
/// </summary>
/// <seealso cref="System.Windows.Application" />
public abstract partial class BedrockApplication : Application
{
    // see: BCL/System/Diagnostics/Assert.cs
    private const int COR_E_FAILFAST = unchecked((int)0x80131623);

    private readonly HostBuilder hostBuilder;       

    private static IHost? Host;

    internal static IServiceProvider Container => Host!.Services;

    public string ApplicationName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BedrockApplication"/> class.
    /// </summary>
    /// <param name="applicationName">The application name.</param>
    protected BedrockApplication(string applicationName)
        :base()
    {
        if (string.IsNullOrWhiteSpace(applicationName))
        {
            throw new ArgumentException($"'{nameof(applicationName)}' cannot be null or whitespace.", nameof(applicationName));
        }

        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

        this.ApplicationName = applicationName;

        AppDomain.CurrentDomain.UnhandledException += this.currentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += this.taskScheduler_UnobservedTaskException;

        this.hostBuilder = new HostBuilder();
        this.hostBuilder.ConfigureHostConfiguration(this.ConfigureHostConfigurationInternal);
        this.hostBuilder.ConfigureAppConfiguration(this.ConfigureAppConfigurationInternal);
        this.hostBuilder.ConfigureLogging(this.ConfigureLogging);
        this.hostBuilder.ConfigureServices(this.ConfigureServicesInternal);

        Host = this.hostBuilder.Build();
    }

    private void taskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        var now = DateTime.Now.ToString("O");

        if (Host?.Services is not null)
        {
            Container.GetRequiredService<ILogger<BedrockApplication>>().LogError(e.Exception, $"Unhandled Task exception. A fail fast will be launch. [Source]: 'TaskScheduler' - [Time]: '{now}'");
        }

        FailFast($"Unhandled exception. [ApplicationName]: '{this.ApplicationName}'", new BedrockApplicationException($"An error occured before the construction of the application host. [Source]: 'TaskScheduler' - [Time]: '{now}'.", e.Exception));
    }
     
    private void currentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var now = DateTime.Now.ToString("O");

        if (Host?.Services is not null)
        {
            Container.GetRequiredService<ILogger<BedrockApplication>>().LogError((Exception)e.ExceptionObject, $"Unhandled exception. A fail fast will be launch.  [Source]: 'AppDomain' - [Time]: '{now}'");
        }

        FailFast($"Unhandled exception. [ApplicationName]: '{this.ApplicationName}'", new BedrockApplicationException($"An error occured before the construction of the application host. [Source]: 'AppDomain' - [Time]: '{now}'", (Exception)e.ExceptionObject));
    }                              

    private static void FailFast(string message, Exception e)
    {
        if (Debugger.IsAttached)
        {
            Environment.Exit(COR_E_FAILFAST);
        }
        else
        {
            Environment.FailFast(message, e);
        }
    }

    protected virtual void AppStartup()
    {
    }

    protected virtual void AppExit()
    {
    }

    /// <summary>Configures the application services.</summary>
    /// <param name="hostBuilderContext">The host builder context.</param>
    /// <param name="serviceCollection">The service collection.</param>
    protected virtual void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection serviceCollection)
    {
    }

    /// <summary>Configures the host configuration.</summary>
    /// <param name="configurationBuilder">The configuration builder.</param>
    protected virtual void ConfigureHostConfiguration(IConfigurationBuilder configurationBuilder)
    {

    }

    /// <summary>Gets the environments variable to look at, in order to load the attended setting file.</summary>
    /// <param name="hostBuilderContext">The host builder context.</param>
    /// <returns>IImmutableList&lt;ConfigurationEnvironment&gt;.</returns>
    [return: NotNull]
    protected virtual IImmutableList<ConfigurationEnvironment> GetEnvironments(HostBuilderContext hostBuilderContext) 
        => ImmutableList<ConfigurationEnvironment>.Empty;

    /// <summary>Configures the application configuration.</summary>
    /// <param name="hostBuilderContext">The host builder context.</param>
    /// <param name="configurationBuilder">The configuration builder.</param>
    protected virtual void ConfigureAppConfiguration(HostBuilderContext hostBuilderContext, IConfigurationBuilder configurationBuilder)
    {
    }

    /// <summary>Configures the logging.</summary>
    /// <param name="hostBuilderContext">The host builder context.</param>
    /// <param name="loggingBuilder">The logging builder.</param>
    protected virtual void ConfigureLogging(HostBuilderContext hostBuilderContext, ILoggingBuilder loggingBuilder)
    {
    }

    /// <summary>Before the wpf app startup.</summary>
    /// <param name="serviceProvider">The service provider.</param>
    protected virtual void BeforeStartup(IServiceProvider serviceProvider)
    {
    }

    /// <summary>Afters the wpf app startup.</summary>
    /// <param name="navigationService">The navigation service.</param>
    protected abstract void AfterStartup(INavigation navigationService);

    /// <summary>Registers the navigation service.</summary>
    /// <param name="serviceCollection">The service collection.</param>
    protected virtual void RegisterNavigationService(IServiceCollection serviceCollection) => serviceCollection.AddNavigationService();

    /// <summary>Registers the master page.</summary>
    /// <param name="serviceCollection">The service collection.</param>
    protected virtual void RegisterMasterPage(IServiceCollection serviceCollection)
    {
        serviceCollection.AddMasterPage((provider) => this.MainWindow);
    }

    /// <summary>Registers page for navigation.</summary>
    /// <param name="navigationRegistry">The navigation registry.</param>
    protected abstract void RegisterForNavigation(INavigationRegistry navigationRegistry);
}
