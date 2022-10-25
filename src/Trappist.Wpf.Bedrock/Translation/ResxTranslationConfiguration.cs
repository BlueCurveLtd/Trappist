using System.Globalization;
using System.Reflection;

namespace Trappist.Wpf.Bedrock.Translation;

public class ResxTranslationConfiguration
{
    /// <summary>
    /// The resource identifier.
    /// </summary>
    public string? ResourceId { get; set; }
    /// <summary>
    /// The assembly where to find the translations.
    /// </summary>
    public Assembly? Assembly { get; set; }
    /// <summary>
    /// The fallback UI culture.
    /// </summary>
    public CultureInfo? FallbackCulture { get; set; }

    internal ResxTranslationConfiguration()
    {

    }
}
