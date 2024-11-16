// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

using System.Text;
using Microsoft.CodeAnalysis;

namespace Pro.Common.SourceGenerator.Extensions
{
    public static class ITypeSymbolExtensions
    {
        public static bool HasFullyQualifiedMetadataName(this ITypeSymbol symbol, string name)
        {
            var builder = new StringBuilder();

            symbol.AppendFullyQualifiedMetadataName(in builder);

            return builder.ToString() == name;
        }

        public static string GetFullyQualifiedMetadataName(this ITypeSymbol symbol)
        {
            var builder = new StringBuilder();

            symbol.AppendFullyQualifiedMetadataName(in builder);

            return builder.ToString();
        }

        private static void AppendFullyQualifiedMetadataName(this ITypeSymbol symbol, in StringBuilder builder)
        {
            static void BuildFrom(ISymbol? symbol, in StringBuilder builder)
            {
                switch (symbol)
                {
                    // Namespaces that are nested also append a leading '.'
                    case INamespaceSymbol { ContainingNamespace.IsGlobalNamespace: false }:
                        BuildFrom(symbol.ContainingNamespace, in builder);
                        builder.Append('.');
                        builder.Append(symbol.MetadataName);
                        break;

                    // Other namespaces (ie. the one right before global) skip the leading '.'
                    case INamespaceSymbol { IsGlobalNamespace: false }:
                        builder.Append(symbol.MetadataName);
                        break;

                    // Types with no namespace just have their metadata name directly written
                    case ITypeSymbol { ContainingSymbol: INamespaceSymbol { IsGlobalNamespace: true } }:
                        builder.Append(symbol.MetadataName);
                        break;

                    // Types with a containing non-global namespace also append a leading '.'
                    case ITypeSymbol { ContainingSymbol: INamespaceSymbol namespaceSymbol }:
                        BuildFrom(namespaceSymbol, in builder);
                        builder.Append('.');
                        builder.Append(symbol.MetadataName);
                        break;

                    // Nested types append a leading '+'
                    case ITypeSymbol { ContainingSymbol: ITypeSymbol typeSymbol }:
                        BuildFrom(typeSymbol, in builder);
                        builder.Append('+');
                        builder.Append(symbol.MetadataName);
                        break;
                    default:
                        break;
                }
            }

            BuildFrom(symbol, in builder);
        }
    }
}
