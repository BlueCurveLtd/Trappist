using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace InGroupe.Innovation.Wpf.Bedrock.Translation.SourceGenerator
{
    [Generator]
    public partial class TranslationGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxReceiver = (TranslationSyntaxReceiver)context.SyntaxReceiver!;

            if (syntaxReceiver.ClassToAugment is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                         "BD0001",
                         "Error while search for class to augment",
                         "No class to augment was found",
                         "Parsing",
                         DiagnosticSeverity.Error,
                         true,
                         customTags: new[] { "bedrock", "translation", "generator" }), Location.None, Array.Empty<object>()));

                return;
            }
           
            var inputFileParser = new InputFileParser(
                context,
                syntaxReceiver.ClassToAugment!.Identifier.ValueText,
                syntaxReceiver.ClassToAugment!.Modifiers.ToString());
                                      
            foreach (var translationFile in context.AdditionalFiles)
            {
                if ((File.GetAttributes(translationFile.Path) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    foreach (var file in this.GetFilesFromDir(translationFile.Path))
                    {
                        Parse(context, inputFileParser, file);
                    }
                }
                else
                {
                    Parse(context, inputFileParser, translationFile.Path);
                }
            }

            var (code, valid) = inputFileParser.Generate();

            if (!valid)
            {
                var sb = new StringBuilder()
                    .AppendLine("// error: one or more translation file is not valid")
                    .AppendLine($"namespace {syntaxReceiver.Namespace} {{}}");
                
                context.AddSource("Translations.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
            }
            else
            {
                var sb = new StringBuilder($"namespace {syntaxReceiver.Namespace}")
                .AppendLine("{")
                .AppendLine(code)
                .AppendLine("}");


                Debug.WriteLine(sb.ToString());
                
                context.AddSource("ErrorTranslations_2.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
            }
        }

        private static void Parse(GeneratorExecutionContext context, InputFileParser inputFileParser, string path)
        {
            try
            {
                if (path.EndsWith(".json", StringComparison.Ordinal))
                {
                    inputFileParser.Parse(path);
                }
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                         "BD0002",
                         $"Error parsing translation file '{path}'",
                         e.Message,
                         "Parsing",
                         DiagnosticSeverity.Warning,
                         true,
                         customTags: new[] { "bedrock", "translation", "generator" }), Location.None, Array.Empty<object>()));
            }
        }

        private IEnumerable<string> GetFilesFromDir(string dir) =>
            Directory.EnumerateFiles(dir, "*.json")
                     .Concat(Directory.EnumerateDirectories(dir)
                     .SelectMany(subdir => this.GetFilesFromDir(subdir)));

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new TranslationSyntaxReceiver());

#if DEBUG
            //if (!Debugger.IsAttached)
            //{
            //    Debugger.Launch();
            //}
#endif
        }
    }
}
