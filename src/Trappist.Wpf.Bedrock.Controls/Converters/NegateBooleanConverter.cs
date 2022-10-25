using System;
using System.Globalization;
using System.Windows.Data;

namespace Trappist.Wpf.Bedrock.Controls.Converters;

public sealed class NegateBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value switch
    {
        bool boolean => !boolean,
        _ => false
    };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
