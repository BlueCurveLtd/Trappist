using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using InGroupe.Innovation.Wpf.Bedrock.Abstractions;

namespace InGroupe.Innovation.Wpf.Bedrock.Translation;

public sealed class TranslationService : ITranslation
{
    private readonly ITranslationProvider translationProvider;

    internal TranslationService(ITranslationProvider translationProvider)
        => this.translationProvider = translationProvider;

    [return: NotNull]
    public string Translate([DisallowNull] string key, [DisallowNull] string category)
        => this.translationProvider.Translate(CultureInfo.CurrentUICulture, key, category);
}
