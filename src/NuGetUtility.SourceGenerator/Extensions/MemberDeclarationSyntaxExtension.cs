// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pro.Common.SourceGenerator.Extensions
{
    internal static class MemberDeclarationSyntaxExtension
    {
        private static readonly NameSyntax s_generatedCodeAttribute = ParseName(typeof(GeneratedCodeAttribute).FullName);

        public static T AddGeneratedCodeAttribute<T>(this T syntax) where T : MemberDeclarationSyntax
        {
            return (T)syntax.AddAttributeLists(AttributeList(SingletonSeparatedList(
                Attribute(s_generatedCodeAttribute)
                    .AddArgumentListArguments(
                        AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(MemberDeclarationSyntaxExtension).Assembly.GetName().Name.ToString()))),
                        AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(MemberDeclarationSyntaxExtension).Assembly.GetName().Version.ToString())))))));
        }
    }
}
