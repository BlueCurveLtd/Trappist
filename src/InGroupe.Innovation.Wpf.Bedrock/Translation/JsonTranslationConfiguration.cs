using System.Diagnostics.CodeAnalysis;

namespace InGroupe.Innovation.Wpf.Bedrock.Translation;

public sealed class JsonTranslationConfiguration
{
    /// <summary>
    /// The root path where to find the translations files.
    /// </summary>
    [AllowNull]
    [MaybeNull]
    public string? RootPath { get; set; }

    internal JsonTranslationConfiguration()
    {

    }
}
