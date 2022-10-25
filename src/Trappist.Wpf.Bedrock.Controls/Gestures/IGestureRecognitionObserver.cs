using System.Windows;

namespace Trappist.Wpf.Bedrock.Controls.Gestures;

/// <summary>
/// Observer to be associated with a gesture surface (<see cref="UIElement"/>).
/// The observer provides the minimum characteristics that will determine which gesture is going to be recognized for itself.
/// </summary>
internal interface IGestureRecognitionObserver
{

    /// <summary>
    /// Use velocity for tap detection, rather than just gesture translation size. Defaults to <code>false</code>.
    /// </summary>
    bool UseVelocityForTapDetection { get; }
    /// <summary>
    /// Set the tap threshold. If swipes are being recognized as taps lowering this value may help.
    /// </summary>
    int TapThreshold { get; }
    /// <summary>
    /// Method in which the observer is going to be notified when a gesture is detected.
    /// </summary>
    /// <param name="gesture">Recognized gesture</param>
    /// <param name="modifier">Recognized modifier. It can be <see cref="Option.Empty"/> if there is not enough information to determine the modifier, usually related to a gentle tap.</param>
    void OnGestureDetected(Gesture gesture, Option<GestureModifier> modifier);
}
#nullable restore
