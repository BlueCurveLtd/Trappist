using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Windows;

using Trappist.Wpf.Bedrock.Abstractions;
using Trappist.Wpf.Bedrock.Navigation;

namespace Trappist.Wpf.Bedrock.Navigation;

public static class NavigationParametersExtensions
{
    /// <summary>
    /// Converts to properties marked with <see cref="NavigableAttribute"/> to navigation parameters.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
    /// <param name="viewModel">The view model.</param>
    /// <returns>NavigationParameters.</returns>
    public static void NavigationParametersFromViewModel<TViewModel>(this TViewModel viewModel)
        where TViewModel : class
    {
        var properties = viewModel.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var navigationParameters = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        if (properties is not null)
        {
            foreach (var property in properties)
            {
                if (property is not null)
                {
                    var attribute = property.GetCustomAttributes(typeof(NavigableAttribute), true)?.FirstOrDefault() as NavigableAttribute;

                    if (attribute is not null)
                    {
                        navigationParameters[attribute.Name ?? property.Name] = property.GetValue(viewModel);
                    }
                }
            }
        }

        NavigationParameters.Instance.Merge(navigationParameters.ToImmutableDictionary());
    }

    /// <summary>
    /// Navigates to the specified page.
    /// </summary>
    /// <param name="navigation">The navigation service.</param>
    /// <param name="mergeParameters">The merge parameters.</param>
    /// <typeparam name="TView">The view.</typeparam>
    public static void Navigate<TView>(this INavigation navigation, [DisallowNull] IReadOnlyDictionary<string, object?> mergeParameters)
        where TView : notnull, FrameworkElement
    {
        Merge(mergeParameters);

        navigation.Navigate<TView>();
    }

    /// <summary>
    /// Navigates to the specified view.
    /// </summary>
    /// <param name="navigation">The navigation service.</param>
    /// <param name="alias">The view alias.</param>
    /// <param name="mergeParameters">The merge parameters.</param>
    /// <exception cref="ArgumentException">The view alias cannot be null, empty or white spaces., <paramref name="alias"/></exception>
    public static void Navigate(this INavigation navigation, [DisallowNull] string alias, [DisallowNull] IReadOnlyDictionary<string, object?> mergeParameters)
    {
        Merge(mergeParameters);

        navigation.Navigate(alias);
    }

    /// <summary>
    /// Navigates to the specified view.
    /// </summary>
    /// <param name="navigation">The navigation service.</param>
    /// <param name="viewType">The type of the view.</param>
    /// <param name="mergeParameters">The merge parameters.</param>
    /// <exception cref="ArgumentNullException">The view type cannot be null. <paramref name="viewType"/></exception>
    public static void Navigate(this INavigation navigation, [DisallowNull] Type viewType, [DisallowNull] IReadOnlyDictionary<string, object?> mergeParameters)
    {
        Merge(mergeParameters);

        navigation.Navigate(viewType);
    }

    /// <summary>
    /// Navigates to the specified page.
    /// </summary>
    /// <param name="navigation">The navigation service.</param>
    /// <param name="mergeParameters">The merge parameters.</param>
    /// <typeparam name="TView">The view.</typeparam>
    public static void Navigate<TView>(this INavigation navigation, [DisallowNull] params (string Key, object? Value)[] mergeParameters)
        where TView : notnull, FrameworkElement
    {
        Merge(mergeParameters);

        navigation.Navigate<TView>();
    }

    /// <summary>
    /// Navigates to the specified view.
    /// </summary>
    /// <param name="navigation">The navigation service.</param>
    /// <param name="alias">The view alias.</param>
    /// <param name="mergeParameters">The merge parameters.</param>
    /// <exception cref="ArgumentException">The view alias cannot be null, empty or white spaces., <paramref name="alias"/></exception>
    public static void Navigate(this INavigation navigation, [DisallowNull] string alias, [DisallowNull] params (string Key, object? Value)[] mergeParameters)
    {
        Merge(mergeParameters);

        navigation.Navigate(alias);
    }

    /// <summary>
    /// Navigates to the specified view.
    /// </summary>
    /// <param name="navigation">The navigation service.</param>
    /// <param name="viewType">The type of the view.</param>
    /// <param name="mergeParameters">The merge parameters.</param>
    /// <exception cref="ArgumentNullException">The view type cannot be null. <paramref name="viewType"/></exception>
    public static void Navigate(this INavigation navigation, [DisallowNull] Type viewType, [DisallowNull] params (string Key, object? Value)[] mergeParameters)
    {
        Merge(mergeParameters);

        navigation.Navigate(viewType);
    }

    private static void Merge(IReadOnlyDictionary<string, object?> mergeParameters)
    {
        if (mergeParameters is null)
        {
            throw new NavigationParametersMergeException("The merge parameters cannot be null");
        }

        NavigationParameters.Instance.Merge(mergeParameters);
    }

    private static void Merge((string Key, object? Value)[] mergeParameters)
    {
        if (mergeParameters is null)
        {
            throw new NavigationParametersMergeException("The merge parameters cannot be null");
        }

        NavigationParameters.Instance.Merge(mergeParameters);
    }
}
