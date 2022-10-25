using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Trappist.Wpf.Bedrock.Abstractions;

public interface ITranslationProvider
{
    /// <summary>
    /// Find the translation of the specified <paramref name="key"/> for a specific culture and category.
    /// </summary>
    /// <param name="culture">The culture.</param>
    /// <param name="key">The key.</param>
    /// <param name="category">The category.</param>
    /// <returns>The translation.</returns>
    [return: NotNull]
    string Translate([AllowNull] CultureInfo? culture, [DisallowNull] string key, [DisallowNull] string category);
}
