using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Xaml.Behaviors;

namespace Trappist.Wpf.Bedrock.Behaviors;

public sealed class NoScrollingBehavior : Behavior<UIElement>
{
    public NoScrollingBehavior()
    {
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        this.AssociatedObject.PreviewMouseWheel += this.PreviewMouseWheel;
    }

    protected override void OnDetaching()
    {
        this.AssociatedObject.PreviewMouseWheel -= this.PreviewMouseWheel;
        base.OnDetaching();
    }

    private void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        e.Handled = true;
        var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
            RoutedEvent = UIElement.MouseWheelEvent,
            Source = sender
        };
        var parent = ((Control)sender).Parent as UIElement;
        parent?.RaiseEvent(eventArg);
    }
}


