// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

using Microsoft.CodeAnalysis;
using Pro.Common.SourceGenerator.Extensions;

namespace Pro.Common.SourceGenerator.Model
{
    internal sealed partial record HierarchyInfo(string FilenameHint, string MetadataName, string Namespace, TypeInfo[] Hierarchy)
    {
        public static HierarchyInfo From(INamedTypeSymbol typeSymbol)
        {
            var hierarchy = new System.Collections.Generic.List<TypeInfo>();

            for (INamedTypeSymbol? parent = typeSymbol;
                 parent is not null;
                 parent = parent.ContainingType)
            {
                hierarchy.Add(new TypeInfo(
                    parent.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                    parent.TypeKind,
                    parent.IsRecord));
            }

            return new(
                typeSymbol.GetFullyQualifiedMetadataName(),
                typeSymbol.MetadataName,
                typeSymbol.ContainingNamespace.ToDisplayString(new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces)),
                hierarchy.ToArray());
        }
    }
}
