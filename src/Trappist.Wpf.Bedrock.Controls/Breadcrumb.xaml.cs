using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml.Linq;

using Trappist.Wpf.Bedrock.Abstractions;
using Trappist.Wpf.Bedrock.Bus;
using Trappist.Wpf.Bedrock.Controls.Routing;
using Trappist.Wpf.Bedrock.Controls.ViewModels;

namespace Trappist.Wpf.Bedrock.Controls;

public partial class Breadcrumb : UserControl
{
    private readonly SubscriptionToken subscriptionToken;

    /// <summary>
    /// The node separator property.
    /// </summary>
    public static readonly DependencyProperty NodeSeparatorProperty = DependencyProperty.Register(
        nameof(NodeSeparator),
        typeof(FrameworkElement),
        typeof(Breadcrumb));   

    /// <summary>
    /// Gets or sets the node separator.
    /// </summary>
    /// <value>
    /// The node separator.
    /// </value>
    public FrameworkElement? NodeSeparator
    {
        get => this.GetValue(NodeSeparatorProperty) as FrameworkElement;
        set => this.SetValue(NodeSeparatorProperty, value);
    }

    public static readonly DependencyProperty NodesProperty = DependencyProperty.Register(
        nameof(Nodes),
        typeof(List<BreadcrumbNode>),
        typeof(Breadcrumb),
        new PropertyMetadata(new List<BreadcrumbNode>()));

    /// <summary>
    /// The list of nodes.
    /// </summary>
    public List<BreadcrumbNode> Nodes
    {
        get => (List<BreadcrumbNode>)this.GetValue(NodesProperty);
        set => this.SetValue(NodesProperty, value);        
    }


    //public static readonly DependencyProperty RoutingProperty = DependencyProperty.Register(
    //    nameof(Routing),
    //    typeof(List<Route>),
    //    typeof(Breadcrumb),
    //    new PropertyMetadata(new List<Route>()));

    ///// <summary>
    ///// The list of nodes.
    ///// </summary>
    //[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0016:Prefer returning collection abstraction instead of implementation", Justification = "WPF")]
    //public List<Route>? Routing
    //{
    //    get => this.GetValue(RoutingProperty) as List<Route>;
    //    set => this.SetValue(RoutingProperty, value);
    //}
     


    public Breadcrumb()
    {
        this.InitializeComponent();
        this.DataContext = new BreadcrumbViewModel();
        this.Nodes.Clear();
        this.subscriptionToken = NavigationEventBus.Instance.Subscribe(currentViewInfo =>
        {
            if (currentViewInfo is { ViewType: not null })
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.ShowElement(currentViewInfo.ViewType);
                });
            }
        });
    }

    public void ShowElement(Type page)
    {
        this.AddOrRemoveElement(page);

        this.listView.UpdateLayout();

        var offset = 0D;

        for (int i = 0; i < this.listView.Items.Count; i++)
        {
            var element = (FrameworkElement)this.listView.ItemContainerGenerator.ContainerFromIndex(i);

            offset += element.RenderSize.Width;

            var record = element.DataContext as InternalBreadcrumbNode;
            if (!record!.IsSeparator && record!.Page!.Equals(page))
            {
                offset -= element.RenderSize.Width;
                break;
            }
        }

        if (this.scrollViewer.HorizontalOffset > offset)
        {
            this.scrollViewer.ScrollToHorizontalOffset(-offset);
        }
        else
        {
            this.scrollViewer.ScrollToHorizontalOffset(offset);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "CA1508:Avoid dead conditional code", Justification = "Analyzer is lost.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0075:Simplify conditional expression", Justification = "Clarity")]
    private void AddOrRemoveElement(Type page)
    {
        if (typeof(IModal).IsAssignableFrom(page))
        {
            return;
        }


        var node = this.Nodes.FirstOrDefault(x => x.Page!.Equals(page));

        var dataContext = (BreadcrumbViewModel)this.DataContext;
        dataContext.AddOrRemove(
            new InternalBreadcrumbNode 
            {
                Page = page, 
                View = node switch {
                    not null => XamlClone(node.View!)!,
                    _ => new TextBlock { Text = "[Unknown]" }
                }
            },
            new InternalBreadcrumbNode 
            {
                View = XamlClone(this.NodeSeparator!), 
                IsSeparator = true 
            },
            ShouldClear(node));
    }

    private static bool ShouldClear(BreadcrumbNode? breadcrumbNode)
    {
        if (breadcrumbNode?.ClearRule is not null)
        {
            return breadcrumbNode.ClearRule.IsSatisfied();
        }

        return false;
    }

    private static T? XamlClone<T>(T original) where T : class
    {
        if (original is not null)
        {
            using var stream = new MemoryStream();
            XamlWriter.Save(original, stream);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)XamlReader.Load(stream);
        }

        return null;
    }

    public sealed class InternalBreadcrumbNode : IEquatable<InternalBreadcrumbNode>
    {
        public FrameworkElement? View { get; init; }

        public Type? Page { get; init; }

        public bool IsSeparator { get; init; }

        internal InternalBreadcrumbNode()
        {

        }

        public override bool Equals(object? obj)
        {
            if (!this.IsSeparator && obj is InternalBreadcrumbNode { IsSeparator: false } other)
            {
                return this.Page!.Equals(other.Page!);
            }

            return false;
        }

        public bool Equals(InternalBreadcrumbNode? other)
        {
            if (!this.IsSeparator && other is { IsSeparator: false })
            {
                return this.Page!.Equals(other.Page!);
            }

            return false;
        }

        public override int GetHashCode()
            => unchecked(HashCode.Combine(this.Page, this.IsSeparator));
    }

    private void BreadcrumbControl_Unloaded(object sender, RoutedEventArgs e) => NavigationEventBus.Instance.Unsubscribe(this.subscriptionToken);

    private void Grid_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
    {

    }

    private void Grid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }
}

/// <summary>
/// Defines a node of the breadcrumb.
/// </summary>
public sealed class BreadcrumbNode
{
    /// <summary>
    /// Gets the view.
    /// </summary>
    /// <value>The view.</value>
    public FrameworkElement? View { get; init; }

    /// <summary>
    /// Gets the page.
    /// </summary>
    /// <value>The page.</value>
    public Type? Page { get; init; }

    /// <summary>
    /// Gets the page alias.
    /// </summary>
    /// <value>The page.</value>
    public string? Alias { get; init; }

    /// <summary>
    /// Gets the clear. if <c>True</c> the breadcrumb will to clear prior to show the new element that has been pushed.
    /// </summary>
    /// <value>The clear.</value>
    public bool Clear { get; init; }


    /// <summary>
    /// Gets the clear rule, if set get precedence over the Clear property.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0016:Prefer returning collection abstraction instead of implementation", Justification = "WPF")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "WPF")]
    public ClearRule ClearRule { get; init; }
}

public abstract class ClearRule : DependencyObject
{
    public static readonly DependencyProperty NavigationInfoProperty = DependencyProperty.Register(
        nameof(NavigationInfo),
        typeof(IImmutableDictionary<string, object?>),
        typeof(ClearRule));

    public IImmutableDictionary<string, object?>? NavigationInfo
    {
        get => this.GetValue(NavigationInfoProperty) as IImmutableDictionary<string, object?>;
        set => this.SetValue(NavigationInfoProperty, value);
    }

    public abstract bool IsSatisfied();
}
