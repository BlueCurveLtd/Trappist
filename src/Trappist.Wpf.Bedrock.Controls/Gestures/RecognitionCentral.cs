using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Trappist.Wpf.Bedrock.Controls.Gestures;

#nullable disable
/// <summary>
/// The RecognitionCentral is a unified place in which to handle subscriptions a given <see cref="UIElement" /> (or set of) associated to a GestureRecognizer,
/// decreasing the load on the number of elements inspecting the state of the <see cref="UIElement"/>.
/// </summary>
internal class RecognitionCentral
{
    private readonly Dictionary<UIElement, HashSet<IGestureRecognitionObserver>> _observersOfElement = new();
    private readonly Dictionary<UIElement, Dictionary<int, int>> _gestureSurfaceTouchRegistry = new();

    /// <summary>
    /// Default instance of the RecognitionCentral.
    /// </summary>
    internal static RecognitionCentral Default = new();

    /// <summary>
    /// Adds a subscription to the element manipulation API of a <see cref="UIElement"/> to then
    /// be able to notify an observer of any gesture detected.
    /// </summary>
    /// <param name="observer">Observer associated to a gesture surface, to process any gesture detected.</param>
    /// <param name="gestureSurface">Surface associated with the observer in which the gesture will take place.</param>
    internal void AddObserver(IGestureRecognitionObserver observer, UIElement gestureSurface)
    {
        // Register the surface first, using the fact the observer registry (_observersOfElement) doesn't have a reference
        // to the surface.
        this.RegisterGestureSurface(gestureSurface);

        // Then you register the observer.
        var observers = this.GetObserversFor(gestureSurface);
        observers = IncludeObserver(observers, observer);
        this._observersOfElement[gestureSurface] = observers;
    }

    /// <summary>
    /// Drops the subscription to the gesture surface and its associated observer.
    /// </summary>
    /// <param name="observer">Observer associated to a gesture surface, to process any gesture detected.</param>
    /// <param name="gestureSurface">Surface associated with the observer in which the gesture will take place.</param>
    internal void RemoveObserver(IGestureRecognitionObserver observer, UIElement gestureSurface)
    {
        // Removing the observer from the subscriptions
        var observers = this.GetObserversFor(gestureSurface);

        observers.RemoveWhere(o => o == observer);

        this._observersOfElement[gestureSurface] = observers;

        // If there is nobody observing the element, let's drop it.
        if (!this._observersOfElement[gestureSurface].Any())
        {
            // clearing the subscriptions
            gestureSurface.ManipulationStarting -= this.HandleManipulationStarting;
            gestureSurface.ManipulationCompleted -= this.HandleManipulationCompleted;
            gestureSurface.ManipulationDelta -= this.HandleManipulationDelta;

            // removing everything else.
            this._observersOfElement.Remove(gestureSurface);
        }
    }

    private static HashSet<IGestureRecognitionObserver> IncludeObserver(HashSet<IGestureRecognitionObserver> observers, IGestureRecognitionObserver observer)
    {
        observers.Add(observer);
        return observers;
    }

    private void RegisterGestureSurface(UIElement gestureSurface)
    {
        if (!this._observersOfElement.ContainsKey(gestureSurface))
        {
            gestureSurface.ManipulationStarting += this.HandleManipulationStarting;
            gestureSurface.ManipulationCompleted += this.HandleManipulationCompleted;
            gestureSurface.ManipulationDelta += this.HandleManipulationDelta;

            this._gestureSurfaceTouchRegistry[gestureSurface] = new Dictionary<int, int>();
        }
    }

    private void HandleManipulationStarting(object sender, ManipulationStartingEventArgs e)
    {
        sender.AsOption<UIElement>().Do(
            el =>
            {
                e.ManipulationContainer = el;
                e.Mode = ManipulationModes.All;
                e.Handled = true;

                if (this._gestureSurfaceTouchRegistry.ContainsKey(el))
                {
                    this._gestureSurfaceTouchRegistry[el] = new Dictionary<int, int>();
                }
            });
    }

    private void HandleManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
        sender.AsOption<UIElement>().Do(
            el =>
            {
                var touches = el.TouchesOver.Count();
                var touchRegistry = this._gestureSurfaceTouchRegistry[el];
                if (!touchRegistry.ContainsKey(touches))
                {
                    touchRegistry[touches] = 1;
                }
                else
                {
                    touchRegistry[touches]++;
                }
            });
    }

    private void HandleManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
        sender.AsOption<UIElement>().Do(
            element =>
            {
                e.Handled = true;
                var modifier =
                    GetMostRepresentativeNumberOfTouchPoints(this._gestureSurfaceTouchRegistry[element])
                        .Map(ToGestureModifier);

                var totalTranslation = e.TotalManipulation.Translation;
                var finalLinearVelocity = e.FinalVelocities.LinearVelocity;
                var observers = this._observersOfElement[element];
                var observersPairedWithGesture =
                    observers.Select(
                        o =>
                            new
                            {
                                Observer = o,
                                Gesture =
                                    ToSwipeGesture(totalTranslation, finalLinearVelocity,
                                        o.UseVelocityForTapDetection, o.TapThreshold),
                                Modifier = modifier
                            });

                foreach (var observerWithGesture in observersPairedWithGesture)
                {
                    observerWithGesture.Observer.OnGestureDetected(observerWithGesture.Gesture, observerWithGesture.Modifier);
                }
            });
    }

    private HashSet<IGestureRecognitionObserver> GetObserversFor(UIElement gestureSurface) => this._observersOfElement.ContainsKey(gestureSurface) ? this._observersOfElement[gestureSurface] : new HashSet<IGestureRecognitionObserver>();

    private static GestureModifier ToGestureModifier(int numberOfTouchInputs) => numberOfTouchInputs switch
    {
        1 => GestureModifier.OneFinger,
        2 => GestureModifier.TwoFingers,
        3 => GestureModifier.ThreeFingers,
        4 => GestureModifier.FourFingers,
        _ => GestureModifier.FiveFingers,
    };

    private static Option<int> GetMostRepresentativeNumberOfTouchPoints(IDictionary<int, int> touchRegistry)
    {
        if (touchRegistry.Any())
        {
            var touchesOrderedByAppearance = touchRegistry.OrderByDescending(pair => pair.Value);
            return Option.Full(touchesOrderedByAppearance.First().Key);
        }
        return Option.Empty();
    }

    private static Gesture ToSwipeGesture(Vector translation, Vector linearVelocity, bool useVelocityForTapDetection, int tapThreshold)
    {
        var deltaX = translation.X;
        var deltaY = translation.Y;
        var distX = Math.Abs(deltaX);
        var distY = Math.Abs(deltaY);
        var isTap = useVelocityForTapDetection
            ? DetectTapFromVelocity(translation, linearVelocity)
            : distX <= tapThreshold && distY <= tapThreshold;

        if (isTap)
        {
            return Gesture.Tap;
        }
        else if (distY >= distX) // bias towards vertical swipe over horizontal if distances are equal
        {
            return deltaY > 0 ? Gesture.SwipeDown : Gesture.SwipeUp;
        }
        else
        {
            return deltaX > 0 ? Gesture.SwipeRight : Gesture.SwipeLeft;
        }
    }

    private static bool DetectTapFromVelocity(Vector translation, Vector linearVelocity)
    {
        const double translationLengthCoeff = -0.029;
        const double inertiaLengthCoeff = -0.029;
        const double intercept = 1.638;
        var regressionScore = translationLengthCoeff * translation.Length + inertiaLengthCoeff * linearVelocity.Length +
                                intercept;

        var inverseProbablity = 1 + Math.Exp(-regressionScore);
        return 1 / inverseProbablity > 0.5;
    }
}
#nullable restore
