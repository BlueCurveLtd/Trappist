using System;

using Microsoft.Extensions.DependencyInjection;

namespace Trappist.Wpf.Bedrock;

public sealed class ServiceLocator
{
    private static ServiceLocator? Locator;
    private readonly IServiceProvider serviceProvider;         

    private ServiceLocator(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        this.serviceProvider = serviceProvider;
    }

    public static TService? GetService<TService>() => Locator!.serviceProvider.GetService<TService>();  

    internal static ServiceLocator Initialize(IServiceProvider serviceProvider) => Locator = new ServiceLocator(serviceProvider);
}
