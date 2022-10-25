using System;

namespace InGroupe.Innovation.Wpf.Bedrock.Translation;

// Anti pattern use here because the ITanslation service is loaded after Xaml parsing
// therefore a null ref is returned if we try to resolve the service in a MarkupExtension

internal sealed class Translations
{
    public static readonly Translations Instance = new ();

    private Translations()
    {
    }

    internal TranslationService? Translation { get; private set; }

    public void AddResxTranslation(Action<ResxTranslationConfiguration> configure)
    {
        var resxTranslationConfiguration = new ResxTranslationConfiguration();
        configure(resxTranslationConfiguration);
        this.Translation = new TranslationService(
            new ResxTranslationProvider(
                resxTranslationConfiguration.ResourceId!,
                resxTranslationConfiguration.Assembly!,
                resxTranslationConfiguration.FallbackCulture));
    }

    public void AddJsonTranslation(Action<JsonTranslationConfiguration> configure)
    {
        var jsonTranslationConfiguration = new JsonTranslationConfiguration();
        configure(jsonTranslationConfiguration);

        this.Translation = new TranslationService(
            new JsonTranslationProvider(jsonTranslationConfiguration.RootPath!));
    }
}
