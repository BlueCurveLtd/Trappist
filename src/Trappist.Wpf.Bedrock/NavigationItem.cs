using System;
using System.Diagnostics.CodeAnalysis;

namespace Trappist.Wpf.Bedrock;

/// <summary>
/// Represent a navigation element
/// </summary>
internal sealed class NavigationItem
{
    public Uri Uri { get; }
    public string Name { get; }
    public Type View { get; }
    public Type? ViewModel { get; }
    public string? Alias { get; }
    public bool IsModal { get; }
    public bool IsFullScreenModal { get; }
    public bool ConfirmNavigation { get; }

    internal NavigationItem(
        [DisallowNull] Uri uri,
        [DisallowNull] string name,
        [DisallowNull] Type view,
        [AllowNull] Type? viewModel,
        [AllowNull] string? alias,
        bool isModal,
        bool isFullScreenModal,
        bool confirmNavigation)
    {
        this.Uri = uri;
        this.Name = name;
        this.View = view;
        this.ViewModel = viewModel;
        this.Alias = alias;
        this.IsModal = isModal;
        this.IsFullScreenModal = isFullScreenModal;
        this.ConfirmNavigation = confirmNavigation;
    }
}
