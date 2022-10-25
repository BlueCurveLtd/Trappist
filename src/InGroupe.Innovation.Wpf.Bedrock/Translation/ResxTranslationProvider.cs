using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Resources;
using InGroupe.Innovation.Wpf.Bedrock.Abstractions;

namespace InGroupe.Innovation.Wpf.Bedrock.Translation;

public sealed class ResxTranslationProvider : ITranslationProvider
{
    private readonly ResourceManager resourceManager;
    private readonly CultureInfo? fallbackCulture;

    internal ResxTranslationProvider([DisallowNull] string resourceId, [DisallowNull] Assembly assembly, [AllowNull] CultureInfo? fallbackCulture)
    {
        this.resourceManager = new ResourceManager(resourceId, assembly);
        this.fallbackCulture = fallbackCulture;
    }

    [return: NotNull]
    public string Translate([AllowNull] CultureInfo? culture, [DisallowNull] string key, [DisallowNull] string category)
    {
        var translation = this.resourceManager.GetString($"{category}_{key}", culture ?? CultureInfo.CurrentUICulture);

        if (translation is null && this.fallbackCulture != null)
        {
            translation = this.resourceManager.GetString($"{category}_{key}", this.fallbackCulture!);
        }

        return translation ?? string.Empty;
    }
}
