using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;

namespace InGroupe.Innovation.Wpf.Bedrock.Controls.Converters
{
    public sealed class TextTransformConverter : IValueConverter
    {
        public TextTransform Transform { get; set; }

        public object Convert([AllowNull] object value, [AllowNull] Type targetType, [AllowNull] object parameter, [AllowNull] CultureInfo culture)
        {
            var text = value as string; 

            if (text is not null)
            {
                return this.Transform switch
                {
                    TextTransform.LowerCase => text.ToLower(CultureInfo.CurrentUICulture),
                    TextTransform.UpperCase => text.ToUpper(CultureInfo.CurrentUICulture),
                    TextTransform.TitleCase => CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(text),
                    TextTransform.CapitalizeFirstLetter => CapitalizeFirstLetter(text),
                    _ => text
                };
            }

            return string.Empty;
        }

        private static string CapitalizeFirstLetter( [AllowNull] string? text)
            => text is { Length: > 1 } ? char.ToUpper(text[0], CultureInfo.CurrentUICulture) + text[1..].ToLower(CultureInfo.CurrentUICulture) : string.Empty;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum TextTransform
    {
        None = 0,
        LowerCase = 1,
        UpperCase = 2,
        TitleCase = 3,
        CapitalizeFirstLetter = 4
    }
}
