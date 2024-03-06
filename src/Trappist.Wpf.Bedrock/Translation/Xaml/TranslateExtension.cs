using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Data;
using System.Windows.Markup;

namespace Trappist.Wpf.Bedrock.Translation.Xaml;

[ContentProperty(nameof(Key))]
[MarkupExtensionReturnType(typeof(string))]
public sealed class TranslateExtension : MarkupExtension
{
    /// <summary>
    /// The translation key.
    /// </summary>
    [MaybeNull]
    [AllowNull]
    public string? Key { get; set; }

    /// <summary>
    /// The translation category.
    /// </summary>
    [MaybeNull]
    [AllowNull]
    public string? Category { get; set; }

    /// <summary>
    /// The converter to use.
    /// </summary>
    [MaybeNull]
    [AllowNull]
    public IValueConverter? Converter { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        string? translation = null;
        if (this.Key is not null && this.Category is not null)
        {
            translation = Translations.Instance.Translation?.Translate(this.Key, this.Category);
        }

        return translation ?? string.Empty;
    }
}
