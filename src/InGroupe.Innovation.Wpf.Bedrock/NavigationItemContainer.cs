using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using InGroupe.Innovation.Wpf.Bedrock.Abstractions;

namespace InGroupe.Innovation.Wpf.Bedrock;

internal static class NavigationItemContainer
{
    private static readonly Dictionary<string, NavigationItem> navigationItems = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, string> navigationAliasItems = new(StringComparer.OrdinalIgnoreCase);


    public static NavigationItem? GetByViewModel<TViewModel>()
    {
        return navigationItems.FirstOrDefault(x => x.Value.ViewModel == typeof(TViewModel)).Value;
    }

    /// <summary>
    /// Gets the meta data associated with the specified view.
    /// </summary>
    /// <typeparam name="TView">The type of view.</typeparam>
    /// <returns>The <see cref="NavigationItem"/> meta data.</returns>
    /// <exception cref="ViewNotFoundException">The specified view has not been found.</exception>
    public static NavigationItem GetView<TView>()
    {
        if (navigationItems.TryGetValue(typeof(TView).FullName!, out var navigationItem))
        {
            return navigationItem;
        }

        throw new ViewNotFoundException($"Unable to find the view. [View]: '{typeof(TView).FullName}'");
    }

    /// <summary>
    /// Gets the meta data associated with the specified view.
    /// </summary>
    /// <param name="viewType">The type of the view.</param>
    /// <returns>The <see cref="NavigationItem"/> meta data.</returns>
    /// <exception cref="ViewNotFoundException">The specified view has not been found.</exception>
    public static NavigationItem GetView(Type viewType)
    {
        if (navigationItems.TryGetValue(viewType.FullName!, out var navigationItem))
        {
            return navigationItem;
        }

        throw new ViewNotFoundException($"Unable to find the view. [View]: '{viewType.FullName}'");
    }

    /// <summary>
    /// Gets the meta data associated with the specified view.
    /// </summary>
    /// <param name="alias">The view alias.</param>
    /// <returns>The <see cref="NavigationItem"/> meta data.</returns>
    /// <exception cref="ViewNotFoundException">The specified view has not been found.</exception>
    public static NavigationItem GetView(string alias)
    {
        if (navigationAliasItems.TryGetValue(alias, out var viewType) && viewType is not null && navigationItems.TryGetValue(viewType, out var navigationItem))
        {
            return navigationItem;
        }

        throw new ViewNotFoundException($"Unable to find the view. [Alias]: '{alias}'");
    }

    /// <summary>
    /// Adds the specified view with its view model.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <param name="builder">The view URI builder.</param>
    public static void Add<TView, TViewModel>([DisallowNull] Func<Uri> builder)
        where TView : class
        where TViewModel : class
        => Add<TView, TViewModel>(typeof(TView).FullName!, builder, null);

    /// <summary>
    /// Adds the specified view.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <param name="builder">The URI builder.</param>
    public static void Add<TView>([DisallowNull] Func<Uri> builder)
        where TView : class
        => Add<TView>(typeof(TView).FullName!, builder, null);

    /// <summary>
    /// Adds the specified view with its view model and alias.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <param name="builder">The builder.</param>
    /// <param name="alias">The alias.</param>
    /// <exception cref="InGroupe.Innovation.Wpf.Bedrock.NavigationRegistrationException">The view alias cannot be empty or white spaces.</exception>
    /// <exception cref="InGroupe.Innovation.Wpf.Bedrock.ViewAliasAlreadyExistsException">A view with the alias '{alias}' already exists.</exception>
    public static void Add<TView, TViewModel>([DisallowNull] Func<Uri> builder, [DisallowNull] string alias)
        where TView : class
        where TViewModel : class
    {
        if(string.IsNullOrWhiteSpace(alias))
        {
            throw new NavigationRegistrationException("The view alias cannot be empty or white spaces.");
        }

        Add<TView, TViewModel>(typeof(TView).FullName!, builder, alias); 
    }

    /// <summary>
    /// Adds the specified view and its alias.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <param name="builder">The builder.</param>
    /// <param name="alias">The alias.</param>
    /// <exception cref="InGroupe.Innovation.Wpf.Bedrock.NavigationRegistrationException">The view alias cannot be empty or white spaces.</exception>
    /// <exception cref="InGroupe.Innovation.Wpf.Bedrock.ViewAliasAlreadyExistsException">A view with the alias '{alias}' already exists.</exception>
    public static void Add<TView>([DisallowNull] Func<Uri> builder, [DisallowNull] string alias)
        where TView : class
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            throw new NavigationRegistrationException("The view alias cannot be empty or white spaces.");
        }

        Add<TView>(typeof(TView).FullName!, builder, alias);

        if (!navigationAliasItems.TryAdd(alias, typeof(TView).FullName!))
        {
            throw new ViewAliasAlreadyExistsException($"A view with the alias '{alias}' already exists.");
        }
    }

    [SuppressMessage("Performance", "HLQ005:Avoid Single() and SingleOrDefault()", Justification = "A view model must be used by only one view.")]
    [return: MaybeNull]
    public static Type? GetViewModel([DisallowNull] Type view)
        => navigationItems.Values.Where(x => x.View.Equals(view)).Select(x => x.ViewModel).SingleOrDefault();

    private static void Add<TView, TViewModel>([DisallowNull] string name, [DisallowNull] Func<Uri> builder, [AllowNull] string? alias)
        where TView : class
        where TViewModel : class
    {
        navigationItems.Add(name, new NavigationItem(builder(), name, typeof(TView), typeof(TViewModel), alias, IsModal<TView>(), IsFullScreenModal<TView>(), MustConfirmNavigation<TViewModel>()));

        if (!string.IsNullOrWhiteSpace(alias))
        {
            navigationAliasItems.Add(alias, typeof(TView).FullName!);
        }
    }

    private static void Add<TView>([DisallowNull] string name, [DisallowNull] Func<Uri> builder, [AllowNull] string? alias)
        where TView : class
    {
        navigationItems.Add(name, new NavigationItem(builder(), name, typeof(TView), null, alias, IsModal<TView>(), IsFullScreenModal<TView>(), false));

        if (!string.IsNullOrWhiteSpace(alias))
        {
            navigationAliasItems.Add(alias, typeof(TView).FullName!);
        }
    }

    private static bool IsModal<TView>() => typeof(TView).GetCustomAttributes<ModalAttribute>().FirstOrDefault() is not null;

    private static bool IsFullScreenModal<TView>()
    {
        var attribute = typeof(TView).GetCustomAttributes<ModalAttribute>().FirstOrDefault();

        if (attribute is not null)
        {
            return attribute.FullScreen;
        }

        return false;
    }

    private static bool MustConfirmNavigation<TViewModel>() where TViewModel : class => typeof(IConfirmNavigation).IsAssignableFrom(typeof(TViewModel));
}
