using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Markup;

using Trappist.Wpf.Bedrock;
using Trappist.Wpf.Bedrock.Abstractions;
using Trappist.Wpf.Bedrock.Navigation.Xaml;

using Microsoft.Extensions.DependencyInjection;

using Trappist.Wpf.Bedrock.Command;

namespace Trappist.Wpf.Bedrock.Navigation.Xaml;

[ContentProperty(nameof(ViewAlias))]
[MarkupExtensionReturnType(typeof(NavigateTo))]
public sealed class NavigateTo : CommandExtension<NavigateTo>
{
    /// <summary>
    /// Gets or sets the view alias.
    /// </summary>
    /// <value>The view alias.</value>
    [AllowNull]
    [MaybeNull]
    public string? ViewAlias { get; set; }

    /// <summary>
    /// Gets or sets the type of the view.
    /// </summary>
    /// <value>The type of the view.</value>
    [AllowNull]
    [MaybeNull]
    public Type? ViewType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the history must be cleared before navigate.
    /// </summary>
    /// <value>if <c>true</c> clear the history; otherwise, <c>false</c>.</value>
    public bool ClearHistory { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the navigation parameters must be cleared before navigate.
    /// </summary>
    /// <value>if <c>true</c> clear the history; otherwise, <c>false</c>.</value>
    public bool ClearParameters { get; set; }

    /// <summary>
    /// Gets or sets the navigation parameters to merge.
    /// </summary>
    public NavigationParameter[] Parameters { get; set; }

    /// <summary>
    /// Indicates if the current view can be refresh if we re-navigate to itself.
    /// </summary>
    public bool RefreshIfSameView { get; set; }

    public override void Execute([AllowNull] object? parameter)
    {
        var navigationService = TrappistApplication.Container.GetService<INavigation>()!;

        if (string.IsNullOrWhiteSpace(this.ViewAlias) && this.ViewType is null)
        {
            throw new NavigationMarkupException($"Invalid navigation markup configuration. At least one of {nameof(this.ViewAlias)} -or- {nameof(this.ViewType)} must be provided.");
        }

        if (!string.IsNullOrWhiteSpace(this.ViewAlias) && this.ViewType is not null)
        {
            throw new NavigationMarkupException($"Invalid navigation markup configuration. Parameters {nameof(this.ViewAlias)} -and- {nameof(this.ViewType)} cannot be sets at the same time.");
        }

        //if (this.ClearHistory)
        //{
        //    NavigationHistory.Clear();
        //}

        if (this.ClearParameters)
        {
            NavigationParameters.Instance.Clear();
        }

        if (this.Parameters?.Length > 0)
        {
            NavigationParameters.Instance.Merge(
                this.Parameters.Where(x => x.Key is not null).ToDictionary(x => x.Key!, x => x.Value, StringComparer.OrdinalIgnoreCase)
                );
        }

        if (!string.IsNullOrWhiteSpace(this.ViewAlias))
        {
            navigationService.Navigate(this.ViewAlias, this.RefreshIfSameView);
        }

        if (this.ViewType is not null)
        {
            navigationService.Navigate(this.ViewType, this.RefreshIfSameView);
        }
    }

    /// <summary>Initializes a new instance of the <see cref="T:Trappist.Wpf.Bedrock.Navigation.Xaml.NavigateTo" /> class.</summary>
    public NavigateTo()
    {
    }

    public NavigateTo([AllowNull] string? viewAlias) => this.ViewAlias = viewAlias;

    public NavigateTo([AllowNull] Type? viewType) => this.ViewType = viewType;

    [return: NotNull]
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
