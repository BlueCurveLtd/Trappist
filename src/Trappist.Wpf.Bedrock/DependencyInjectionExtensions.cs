using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

using Trappist.Wpf.Bedrock.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Trappist.Wpf.Bedrock.InstanceControl;
using Trappist.Wpf.Bedrock.Messaging;

namespace Trappist.Wpf.Bedrock;

public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registers the navigation service.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="configure">Configuration delegate.</param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddNavigationService(this IServiceCollection serviceCollection, Action<NavigationServiceConfiguration>? configure = null)
    {
        var navigationConfiguration = new NavigationServiceConfiguration();
        configure?.Invoke(navigationConfiguration);

        serviceCollection.TryAddSingleton(navigationConfiguration);
        serviceCollection.TryAddSingleton<INavigationRegistry>(new NavigationRegistry());
        serviceCollection.TryAddSingleton<INavigation>(serviceProvider => new NavigationService(
            serviceProvider.GetRequiredService<NavigationServiceConfiguration>(),
            serviceProvider
            ));

        return serviceCollection;
    }

    /// <summary>
    /// Adds the messaging service.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddMessaging(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddSingleton<IMessenger>(new Messenger());

        return serviceCollection;
    }

    /// <summary>
    /// Adds the navigation observer service.
    /// </summary>
    /// <typeparam name="TNavigationObserver">The type of the navigation observer.</typeparam>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddNavigationObservability<TNavigationObserver>(this IServiceCollection serviceCollection)
        where TNavigationObserver : class, INavigationObserver
        => serviceCollection.AddSingleton<INavigationObserver, TNavigationObserver>();

    /// <summary>
    /// Registers the application master page.
    /// </summary>
    /// <typeparam name="TWindow">The type of the window.</typeparam>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="factory">The factory.</param>
    public static void AddMasterPage<TWindow>(this IServiceCollection serviceCollection, [DisallowNull] Func<IServiceProvider, TWindow> factory)
        where TWindow : Window
    {
        serviceCollection.TryAddSingleton(factory);
    }

    /// <summary>
    /// Registers the ui thread service?
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddUiThread(this IServiceCollection serviceCollection) => serviceCollection.AddTransient<IUiThread>();


    /// <summary>
    /// Adds the modal navigation system.
    /// </summary>
    /// <typeparam name="TModalNavigationContainer">The type of the modal navigation container.</typeparam>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="factory">The factory.</param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddModalNavigation<TModalNavigationContainer>(this IServiceCollection serviceCollection, [DisallowNull] Func<IServiceProvider, TModalNavigationContainer> factory)
        where TModalNavigationContainer : class, IModalNavigationProvider
    {
        serviceCollection.TryAddSingleton<IModalNavigationProvider>(serviceProvider => factory(serviceProvider));

        return serviceCollection;
    }


    /// <summary>
    /// Adds the modal navigation system.
    /// </summary>
    /// <typeparam name="TModalNavigationContainer">The type of the modal navigation container.</typeparam>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddModalNavigation<TModalNavigationContainer>(this IServiceCollection serviceCollection)
        where TModalNavigationContainer : class, IModalNavigationProvider
    {
        serviceCollection.TryAddSingleton<IModalNavigationProvider, TModalNavigationContainer>();

        return serviceCollection;
    }

    /// <summary>
    /// Ensures the application is single instance at user session level.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection UseUserSingleInstance(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddSingleton<ISingleInstanceApp>(new UserSingleInstanceApp());

        return serviceCollection;
    }

    /// <summary>
    /// Ensures the application is single instance at machine level.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="identifierProvider">The identifier provider.</param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection UseMachineSingleInstance(this IServiceCollection serviceCollection, Func<string> identifierProvider)
    {
        serviceCollection.TryAddSingleton<ISingleInstanceApp>(new MachineSingleInstance(identifierProvider()));

        return serviceCollection;
    }
}
