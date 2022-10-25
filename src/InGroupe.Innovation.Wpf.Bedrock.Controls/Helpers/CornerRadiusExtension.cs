using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace InGroupe.Innovation.Wpf.Bedrock.Controls.Helpers
{
    public class CornerRadiusExtension
    {
        public static CornerRadius GetCornerRadius(DependencyObject obj) => (CornerRadius)obj.GetValue(CornerRadiusProperty);

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value) => obj.SetValue(CornerRadiusProperty, value);

        public static readonly DependencyProperty CornerRadiusProperty =  DependencyProperty.RegisterAttached(nameof(Border.CornerRadius), typeof(CornerRadius), typeof(CornerRadiusExtension), new UIPropertyMetadata(new CornerRadius(), CornerRadiusChangedCallback));

        public static void CornerRadiusChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as Control;

            if (control != null)
            {
                control.Loaded += Control_Loaded;
            }
        }

        private static void Control_Loaded(object sender, EventArgs e)
        {
            var control = sender as Control;

            if (control?.Template == null)
            {
                return;
            }

            control.ApplyTemplate();

            CornerRadius cornerRadius = GetCornerRadius(control);

            var toggleButton = control.Template.FindName("toggleButton", control) as Control;

            if (control is ComboBox && toggleButton != null)
            {
                toggleButton.ApplyTemplate();

                // Set border radius for border radius border
                var toggleButtonBorder = (Border)toggleButton.Template.FindName("templateRoot", toggleButton);
                toggleButtonBorder.CornerRadius = cornerRadius;

                // Expand padding for combobox to avoid text clipping by border radius
                control.Padding = new Thickness(
                        control.Padding.Left + cornerRadius.BottomLeft,
                        control.Padding.Top,
                        control.Padding.Right + cornerRadius.BottomRight,
                        control.Padding.Bottom);

                // Decrease width of dropdown and center it to avoid showing "sticking" dropdown corners
                var popup = control.Template.FindName("PART_Popup", control) as Popup;              

                if (popup != null)
                {
                    double offset = cornerRadius.BottomLeft - 1;
                    if (offset > 0)
                    {
                        popup.HorizontalOffset = offset;
                    }
                }

                var shadowChrome = control.Template.FindName("shadow", control) as Decorator;

                if (shadowChrome != null)
                {
                    double minWidth = control.ActualWidth - cornerRadius.BottomLeft - cornerRadius.BottomRight;
                    if (minWidth > 0)
                    {
                        shadowChrome.MinWidth = minWidth;
                    }
                }
            }

            // setting borders for non-combobox controls
            var border = control.Template.FindName("border", control) as Border;

            if (border == null)
            {
                return;
            }

            border.CornerRadius = cornerRadius;
        }
    }
}
