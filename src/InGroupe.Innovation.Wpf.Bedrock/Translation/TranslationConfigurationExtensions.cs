using System;
using System.Diagnostics.CodeAnalysis;
using InGroupe.Innovation.Wpf.Bedrock.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace InGroupe.Innovation.Wpf.Bedrock.Translation;

public static class TranslationConfigurationExtensions
{
    /// <summary>
    /// Adds the RESX translation system.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="configure">The configure.</param>
    /// <returns></returns>
    public static IServiceCollection AddResxTranslation(this IServiceCollection serviceCollection, [DisallowNull] Action<ResxTranslationConfiguration> configure)
    {
        Translations.Instance.AddResxTranslation(configure);
        return serviceCollection.AddSingleton<ITranslation, TranslationService>(_ => Translations.Instance.Translation!);
    }

    /// <summary>
    /// Adds the json translation system.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="configure">The configure.</param>
    /// <returns></returns>
    public static IServiceCollection AddJsonTranslation(this IServiceCollection serviceCollection, [DisallowNull] Action<JsonTranslationConfiguration> configure)
    {
        Translations.Instance.AddJsonTranslation(configure);
        return serviceCollection.AddSingleton<ITranslation, TranslationService>(_ => Translations.Instance.Translation!);
    }
}
