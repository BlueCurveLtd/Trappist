using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows;

using Trappist.Wpf.Bedrock.Abstractions;

namespace Trappist.Wpf.Bedrock;

public sealed class NavigationRegistry : INavigationRegistry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationRegistry"/> class.
    /// </summary>
    internal NavigationRegistry()
    {

    }

    /// <summary>
    /// Registers the specified page to the navigation service.
    /// </summary>
    /// <typeparam name="TPage">The type of the page.</typeparam>
    public void RegisterForNavigation<TPage>() where TPage : notnull, FrameworkElement
    {
        var pageType = typeof(TPage);

        if (TryGetViewModel<TPage>(out var viewModel))
        {
            this.GetType().GetMethod(nameof(this.RegisterForNavigationInternal), BindingFlags.NonPublic | BindingFlags.Instance)
                ?.MakeGenericMethod(pageType!, viewModel!)
                ?.Invoke(this, null);
        }
        else
        {
            if (pageType!.Namespace!.Contains(".Views.", StringComparison.Ordinal))
            {
                var uri = pageType.Namespace.Substring(pageType.Namespace.IndexOf('.')).Replace('.', '/');

                NavigationItemContainer.Add<TPage>(() => new Uri($"{uri}/{pageType.Name}.xaml", UriKind.Relative));
            }
            else
            {
                NavigationItemContainer.Add<TPage>(() => new Uri($"/Views/{pageType.Name}.xaml", UriKind.Relative));
            }
        }
    }

    /// <summary>
    /// Registers the specified page to the navigation service.
    /// </summary>
    /// <typeparam name="TPage">The type of the page.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public void RegisterForNavigation<TPage, TViewModel>()
        where TPage : notnull, FrameworkElement
        where TViewModel : class
    {
        var pageType = typeof(TPage);

        if (pageType!.Namespace!.Contains(".Views.", StringComparison.Ordinal))
        {
            var uri = pageType.Namespace.Substring(pageType.Namespace.IndexOf('.')).Replace('.', '/');

            NavigationItemContainer.Add<TPage, TViewModel>(() => new Uri($"{uri}/{pageType.Name}.xaml", UriKind.Relative));
        }
        else
        {
            NavigationItemContainer.Add<TPage, TViewModel>(() => new Uri($"/Views/{pageType.Name}.xaml", UriKind.Relative));
        }
    }

    /// <summary>
    /// Registers the specified page to the navigation service.
    /// </summary>
    /// <typeparam name="TPage">The type of the page.</typeparam>
    /// <param name="alias">The page alias.</param>
    /// <exception cref="ArgumentNullException">alias</exception>
    public void RegisterForNavigation<TPage>([DisallowNull] string alias) where TPage : notnull, FrameworkElement
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            throw new ArgumentNullException(nameof(alias));
        }

        var pageType = typeof(TPage);

        if (TryGetViewModel<TPage>(out var viewModel))
        {
            this.GetType().GetMethod(nameof(this.RegisterForNavigationInternalWithAlias), BindingFlags.NonPublic | BindingFlags.Instance)
                ?.MakeGenericMethod(pageType!, viewModel!)
                ?.Invoke(this, new[] { alias });
        }
        else
        {
            if (pageType!.Namespace!.Contains(".Views.", StringComparison.Ordinal))
            {
                var uri = pageType.Namespace.Substring(pageType.Namespace.IndexOf('.')).Replace('.', '/');

                NavigationItemContainer.Add<TPage>(() => new Uri($"{uri}/{pageType.Name}.xaml", UriKind.Relative), alias);
            }
            else
            {
                NavigationItemContainer.Add<TPage>(() => new Uri($"/Views/{pageType.Name}.xaml", UriKind.Relative), alias);
            }
        }
    }

    /// <summary>
    /// Registers the specified page to the navigation service.
    /// </summary>
    /// <typeparam name="TPage">The type of the page.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <param name="alias">The page alias.</param>
    /// <exception cref="ArgumentNullException"><paramref name="alias"/></exception>
    public void RegisterForNavigation<TPage, TViewModel>([DisallowNull] string alias)
        where TPage : notnull, FrameworkElement
        where TViewModel : class
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            throw new ArgumentNullException(nameof(alias));
        }

        var pageType = typeof(TPage);

        if (pageType!.Namespace!.Contains(".Views.", StringComparison.Ordinal))
        {
            var uri = pageType.Namespace.Substring(pageType.Namespace.IndexOf('.')).Replace('.', '/');

            NavigationItemContainer.Add<TPage, TViewModel>(() => new Uri($"{uri}/{pageType.Name}.xaml", UriKind.Relative), alias);
        }
        else
        {
            NavigationItemContainer.Add<TPage, TViewModel>(() => new Uri($"/Views/{pageType.Name}.xaml", UriKind.Relative), alias);
        }
    }

    private void RegisterForNavigationInternal<TPage, TViewModel>()
        where TPage : notnull, FrameworkElement
        where TViewModel : class
        => this.RegisterForNavigation<TPage, TViewModel>();

    private void RegisterForNavigationInternalWithAlias<TPage, TViewModel>(string alias)
        where TPage : notnull, FrameworkElement
        where TViewModel : class
        => this.RegisterForNavigation<TPage, TViewModel>(alias);

    private static bool TryGetViewModel<TPage>([MaybeNullWhen(false)] out Type? viewModel) where TPage : notnull, FrameworkElement
    {
        viewModel = null;

        var pageType = typeof(TPage)!;

        var typeName = $"{pageType.Namespace?.Replace(".Views", ".ViewModels", StringComparison.Ordinal)}.{pageType.Name}ViewModel, {pageType.Assembly.FullName}";

        var vmType = Type.GetType(typeName);

        if (vmType is not null)
        {
            viewModel = vmType;

            return true;
        }

        return false;
    }
}
