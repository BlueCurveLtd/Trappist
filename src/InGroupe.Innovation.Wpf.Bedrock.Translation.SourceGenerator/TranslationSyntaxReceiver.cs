using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace InGroupe.Innovation.Wpf.Bedrock.Translation.SourceGenerator
{
    // https://andrewlock.net/using-source-generators-with-a-custom-attribute--to-generate-a-nav-component-in-a-blazor-app/
    // https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md

    internal class TranslationSyntaxReceiver : ISyntaxReceiver
    {
        private static readonly Regex NamespaceRegex = new("namespace\\s+(\\w+)", RegexOptions.Compiled | RegexOptions.Singleline);

        public ClassDeclarationSyntax? ClassToAugment { get; private set; }
        
        public string? Namespace { get; private set; }
        
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // && cds.Identifier.ValueText == "Fuck" &&  namespace MobileSuitCase

            if (syntaxNode is ClassDeclarationSyntax cds && cds.AttributeLists.SelectMany(x => x.Attributes).Any(attr => attr.Name.ToString() == "GenerateTranslation"))
            {
                this.Namespace = GetNamespace(((Microsoft.CodeAnalysis.SyntaxNode)cds)!.Parent!.ToString());

                this.ClassToAugment = cds;
            }
        }

        private static string? GetNamespace(string value) => NamespaceRegex.Match(value).Groups[1].Value;
    }
}
