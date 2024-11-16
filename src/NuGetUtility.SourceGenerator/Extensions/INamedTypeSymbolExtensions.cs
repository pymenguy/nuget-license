// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

using System.Linq;
using Microsoft.CodeAnalysis;

namespace Pro.Common.SourceGenerator.Extensions
{
    public static class INamedTypeSymbolExtensions
    {
        public static bool InheritsFrom<T>(this ITypeSymbol? symbol) => symbol.InheritsFrom(typeof(T).FullName);

        public static bool InheritsFrom(this ITypeSymbol? symbol, string typeFullName)
        {
            while (symbol is not null)
            {
                if (symbol.HasFullyQualifiedMetadataName(typeFullName))
                {
                    return true;
                }
                if (symbol.AllInterfaces.Any(i => i.HasFullyQualifiedMetadataName(typeFullName)))
                {
                    return true;
                }
                symbol = symbol.BaseType;
            }
            return false;
        }

        public static bool ImplementsInterfaceDirectly<T>(this ITypeSymbol? symbol) => symbol.ImplementsInterfaceDirectly(typeof(T).FullName);

        public static bool ImplementsInterfaceDirectly(this ITypeSymbol? symbol, string typeFullName)
        {
            if (symbol?.BaseType?.AllInterfaces.Any(i => i.HasFullyQualifiedMetadataName(typeFullName)) ?? false)
            {
                return false;
            }
            if (symbol?.AllInterfaces.Any(i => i.HasFullyQualifiedMetadataName(typeFullName)) ?? false)
            {
                return true;
            }
            return false;
        }
    }
}
