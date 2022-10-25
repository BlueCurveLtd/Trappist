using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Trappist.Wpf.Bedrock.Abstractions;

using Newtonsoft.Json;

namespace Trappist.Wpf.Bedrock.Translation;

public sealed class JsonTranslationProvider : ITranslationProvider
{
    private readonly ImmutableDictionary<string, Translation> translations;
    private readonly Translation defaultTranslation;

    internal JsonTranslationProvider([DisallowNull] string rootPath)
    {
        if (string.IsNullOrEmpty(rootPath))
        {
            throw new ArgumentNullException(nameof(rootPath));
        }

        this.translations = (from file in Directory.EnumerateFiles(rootPath, "*.json")
                             let json = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(file, Encoding.UTF8))
                             let code = (json?["Meta"]?["Code"] as Newtonsoft.Json.Linq.JValue)?.Value as string
                             let @default = json["Meta"]?["Default"]?.Value ?? false
                             select new Translation(code, @default, json)).ToImmutableDictionary(x => x.Code, x => x);

        this.defaultTranslation = this.translations.FirstOrDefault(x => x.Value.Default).Value;
    }

    private string GetDefaultTranslation(string key, string category) => this.defaultTranslation?.Json?[category]?[key]?.Value ?? string.Empty;

    [return: NotNull]
    public string Translate([AllowNull] CultureInfo? culture, [DisallowNull] string key, [DisallowNull] string category)
    {
        var code = culture?.Name ?? CultureInfo.CurrentUICulture.Name;

        return code switch
        {
            null => this.GetDefaultTranslation(key, category),
            _ => this.translations[code]?.Json?[category]?[key]?.Value ?? this.GetDefaultTranslation(key, category)
        };
    }

    private sealed record Translation(string Code, bool Default, dynamic Json);
}
