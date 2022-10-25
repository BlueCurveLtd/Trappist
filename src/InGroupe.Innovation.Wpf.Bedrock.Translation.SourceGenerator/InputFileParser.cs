using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;

using Humanizer;

using Microsoft.CodeAnalysis;

namespace InGroupe.Innovation.Wpf.Bedrock.Translation.SourceGenerator
{
    internal class InputFileParser
    {
        private const string MetaDataHeader = "Meta";
        private const string DictionaryFullName = "global::System.Collections.Generic.Dictionary";

        // lang, category, key, value
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, string?>>> translations = new(StringComparer.OrdinalIgnoreCase);
        private readonly GeneratorExecutionContext context;
        private readonly string className;
        private readonly string modifier;
        private string? defaultLang;

        public InputFileParser(GeneratorExecutionContext context, string className, string modifier)
        {
            this.context = context;
            this.className = className;
            this.modifier = modifier;
        }

        public void Parse(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            var document = JsonDocument.Parse(stream);

            using var enumerator = document.RootElement.EnumerateObject();

            var metaFound = false;
            string? lang = null;
 
            while (enumerator.MoveNext() && !metaFound)
            {
                // Head
                if (enumerator.Current.Name.Equals(MetaDataHeader, StringComparison.Ordinal))
                {
                    var metaData = TryReadMeta(enumerator.Current.Value);

                    metaFound = !string.IsNullOrWhiteSpace(metaData?.Code);
                    lang = metaData?.Code;

                    if (metaData is { Default: true })
                    {
                        this.defaultLang = metaData.Code;
                    }
                }
            }             

            enumerator.Reset();

            if (metaFound && !string.IsNullOrWhiteSpace(lang))
            {
                if (!this.translations.ContainsKey(lang!))
                {
                    this.translations[lang!] = new(StringComparer.OrdinalIgnoreCase);
                }

                while (enumerator.MoveNext())
                {
                    if (!enumerator.Current.Name.Equals(MetaDataHeader, StringComparison.Ordinal))
                    {
                        var category = enumerator.Current.Name;

                        if (!this.translations[lang!].ContainsKey(category))
                        {
                            this.translations[lang!][category] = new(StringComparer.OrdinalIgnoreCase);
                        }

                        using var properties = enumerator.Current.Value.EnumerateObject();

                        while (properties.MoveNext())
                        {
                            this.translations[lang!][category][properties.Current.Name] = properties.Current.Value.GetString();
                        }
                    }
                }
            }
        }

        private static Meta? TryReadMeta(JsonElement jsonElement)
        {
            try
            {
                return jsonElement.Deserialize<Meta>();
            }
            catch (Exception)
            {
            }

            return default;
        }

        public (string Code, bool IsValid) Generate()
        {
            if (this.translations.Count == 0)
            {
                return (string.Empty, false);
            }

            var sb = new StringBuilder()
                .AppendLine($@"[global::System.CodeDom.Compiler.GeneratedCode(""BedrockTranslationSourceGenerator"", ""{Assembly.GetExecutingAssembly().GetName().Version}"")]")
                .AppendLine("[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]")
                .AppendLine($"{this.modifier} class {this.className}")
                .AppendLine("{")
                .AppendLine()
                .AppendLine($"    private static readonly {DictionaryFullName}<string, {DictionaryFullName}<string, {DictionaryFullName}<string, string?>>> translations = new (global::System.StringComparer.OrdinalIgnoreCase);")
                .AppendLine()
                .AppendLine($"    static {this.className}() {{");

            foreach (var translation in this.translations)
            {
                sb.AppendLine($"        translations[\"{ translation.Key}\"] = new(global::System.StringComparer.OrdinalIgnoreCase);"); // culture

                foreach (var category in translation.Value)
                {
                    sb.AppendLine($"        translations[\"{translation.Key}\"][\"{category.Key}\"] = new(global::System.StringComparer.OrdinalIgnoreCase);"); // category

                    foreach (var key in category.Value)
                    {
                        sb.AppendLine($"        translations[\"{translation.Key}\"][\"{category.Key}\"][\"{key.Key}\"] = \"{key.Value}\";"); // key
                    }
                }
            }

            sb.AppendLine("    }");

            // ------------------------

            sb.AppendLine(@"
                        private static string? GetValue(string category, string key) 
                        {
                            return translations[global::System.Globalization.CultureInfo.CurrentUICulture.Name][category][key];
                        }
                    ");

            // ------------------------ 
            
            var categories = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var category in this.translations.Values)
            {
                foreach (var cat in category)
                {
                    if (!categories.ContainsKey(cat.Key))
                    {
                        categories[cat.Key] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    }

                    foreach (var value in cat.Value)
                    {
                        categories[cat.Key].Add(value.Key);
                    }
                }
            }

            // Generate sub classes
            foreach (var category in categories)
            {
                var categoryStringBuilder = new StringBuilder($"    public static class {category.Key.Pascalize()}Translations").AppendLine("    {");

                //foreach (var entry in category.Value)
                //{
                //    categoryStringBuilder.AppendLine($"        private static string? _{entry.ToLowerInvariant()};");
                //}

                foreach (var entry in category.Value)
                {
                    categoryStringBuilder.AppendLine($"        public static string? {entry.Pascalize()} => GetValue(\"{category.Key}\", \"{entry}\");");
                }


                //categoryStringBuilder
                //    .AppendLine()
                //    .AppendLine("        private struct TranslationHolder")
                //    .AppendLine("        {");


                //foreach (var translation in this.translations)
                //{
                //    foreach (var entry in category.Value)
                //    {
                //        categoryStringBuilder.AppendLine("            public ");
                //    }
                //}

                //categoryStringBuilder.AppendLine("        }"); // end of struct


                categoryStringBuilder.AppendLine("    }");   // end of class

                sb.AppendLine().Append(categoryStringBuilder.ToString()).AppendLine();
            }

            sb.AppendLine("}"); // eof

            return (sb.ToString(), true); 
        }
    }
}
