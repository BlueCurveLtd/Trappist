using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Trappist.Wpf.Bedrock.Controls.Animations;

#nullable disable
public class BrushAnimation : AnimationTimeline
{
    public static readonly DependencyProperty FromProperty = DependencyProperty.Register(nameof(From), typeof(Brush), typeof(BrushAnimation));
    public static readonly DependencyProperty ToProperty = DependencyProperty.Register(nameof(To), typeof(Brush), typeof(BrushAnimation));

    public override Type TargetPropertyType => typeof(Brush);

    public Brush From
    {
        get => (Brush)this.GetValue(FromProperty);
        set => this.SetValue(FromProperty, value);
    }
    public Brush To
    {
        get => (Brush)this.GetValue(ToProperty);
        set => this.SetValue(ToProperty, value);
    }

    public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        => this.GetCurrentValue(defaultOriginValue as Brush, defaultDestinationValue as Brush, animationClock);

    public object GetCurrentValue(Brush defaultOriginValue, Brush defaultDestinationValue, AnimationClock animationClock)
    {
        if (!animationClock.CurrentProgress.HasValue)
        {
            return Brushes.Transparent;
        }

        // use the standard values if From and To are not set 
        // (it is the value of the given property)
        defaultOriginValue = this.From ?? defaultOriginValue;
        defaultDestinationValue = this.To ?? defaultDestinationValue;

        if (animationClock.CurrentProgress.Value == 0)
        {
            return defaultOriginValue;
        }

        if (animationClock.CurrentProgress.Value == 1)
        {
            return defaultDestinationValue;
        }

        return new VisualBrush(new Border
        {
            Width = 1,
            Height = 1,
            Background = defaultOriginValue,
            Child = new Border
            {
                Background = defaultDestinationValue,
                Opacity = animationClock.CurrentProgress.Value,
            }
        });
    }

    protected override Freezable CreateInstanceCore() => new BrushAnimation();
}
#nullable restore
