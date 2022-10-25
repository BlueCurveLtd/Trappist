using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InGroupe.Innovation.Wpf.Bedrock.Controls.Converters
{
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                throw new ArgumentException($"Unexpected null value in '{nameof(BooleanToVisibilityConverter)}'", nameof(value));
            }

            if (!(value is bool))
            {
                throw new ArgumentException($"Unexpected type in '{nameof(BooleanToVisibilityConverter)}'", nameof(value));
            }

            {
                if (parameter is bool invert && invert)
                {
                    return (bool)value ? Visibility.Collapsed : Visibility.Visible;
                }
            }

            {
                if (parameter is string stringValue && bool.TryParse(stringValue, out var invert) && invert)
                {
                    return (bool)value ? Visibility.Collapsed : Visibility.Visible;
                }
            }

            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
