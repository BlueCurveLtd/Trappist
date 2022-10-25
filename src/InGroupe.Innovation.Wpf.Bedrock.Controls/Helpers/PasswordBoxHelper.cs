using System.Windows;
using System.Windows.Controls;

namespace InGroupe.Innovation.Wpf.Bedrock.Controls.Helpers
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPassword = DependencyProperty.RegisterAttached(nameof(BoundPassword), typeof(string), typeof(PasswordBoxHelper), new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static readonly DependencyProperty BindPassword = DependencyProperty.RegisterAttached(nameof(BindPassword), typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false, OnBindPasswordChanged));

        private static readonly DependencyProperty UpdatingPassword = DependencyProperty.RegisterAttached(nameof(UpdatingPassword), typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false));

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            

            // only handle this event when the property is attached to a PasswordBox
            // and when the BindPassword attached property has been set to true
            if (d is PasswordBox box && GetBindPassword(d))
            {
                // avoid recursive updating by ignoring the box's changed event
                box.PasswordChanged -= HandlePasswordChanged;

                string newPassword = (string)e.NewValue;

                if (!GetUpdatingPassword(box))
                {
                    box.Password = newPassword;
                }

                box.PasswordChanged += HandlePasswordChanged;
            }
        }

        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            // when the BindPassword attached property is set on a PasswordBox,
            // start listening to its PasswordChanged event
             
            if (dp is PasswordBox box)
            {
                var wasBound = (bool)(e.OldValue);
                var needToBind = (bool)(e.NewValue);

                if (wasBound)
                {
                    box.PasswordChanged -= HandlePasswordChanged;
                }

                if (needToBind)
                {
                    box.PasswordChanged += HandlePasswordChanged;
                }
            }
        }

        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            var box = sender as PasswordBox;

            // set a flag to indicate that we're updating the password
            SetUpdatingPassword(box, true);
            // push the new password into the BoundPassword property
            SetBoundPassword(box, box?.Password);
            SetUpdatingPassword(box, false);
        }

        public static void SetBindPassword(DependencyObject dp, bool value)
            => dp.SetValue(BindPassword, value);

        public static bool GetBindPassword(DependencyObject dp)
            => (bool)dp.GetValue(BindPassword);

        public static string GetBoundPassword(DependencyObject dp)
            => (string)dp.GetValue(BoundPassword);

        public static void SetBoundPassword(DependencyObject? dp, string? value)
            => dp?.SetValue(BoundPassword, value);

        private static bool GetUpdatingPassword(DependencyObject dp)
            => (bool)dp.GetValue(UpdatingPassword);

        private static void SetUpdatingPassword(DependencyObject? dp, bool value)
            => dp?.SetValue(UpdatingPassword, value);
    }
}
