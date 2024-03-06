using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Trappist.Wpf.Bedrock;

namespace Trappist.Wpf.Bedrock;

internal static class ViewModelLocatorProvider
{
    /// <summary>
    /// Resolves the view model.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="factory">The factory.</param>
    public static void ResolveViewModel([DisallowNull] object view, [DisallowNull] Action<object, object?> factory)
    {
        var viewModelType = NavigationItemContainer.GetViewModel(view.GetType());

        if (viewModelType is not null)
        {
            var constructors = viewModelType.GetConstructors();

            object? viewModel = null;

            if (constructors.Length == 0 || constructors.Length == 1 && constructors[0].GetParameters().Length == 0)
            {
                viewModel = Activator.CreateInstance(viewModelType);
            }
            else
            {
                viewModel = Activator.CreateInstance(viewModelType, constructors[0].GetParameters().Select(x => TrappistApplication.Container.GetService(x.ParameterType)).ToArray());
            }

            factory(view, viewModel);
        }
    }
}
