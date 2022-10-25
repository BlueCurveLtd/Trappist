using System.Diagnostics.CodeAnalysis;

namespace Trappist.Wpf.Bedrock.Abstractions;

public interface ITranslation
{
    /// <summary>
    /// Find the translation of the specified <paramref name="key"/> for a specific culture and category.
    /// </summary>
    /// <remarks>The UI culture of the application will be use.</remarks>
    /// <param name="key">The key.</param>
    /// <param name="category">The category.</param>
    /// <returns>The translation.</returns>
    [return: NotNull]
    string Translate([DisallowNull] string key, [DisallowNull] string category);
}
