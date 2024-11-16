// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

using Microsoft.CodeAnalysis;

namespace Pro.Common.SourceGenerator.Extensions
{
    public static class ISymbolExtensions
    {
        public static bool HasAttributeWithFullyQualifiedMetadataName(this ISymbol symbol, string name)
        {
            foreach (AttributeData attribute in symbol.GetAttributes())
            {
                if (attribute.AttributeClass?.HasFullyQualifiedMetadataName(name) == true)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
