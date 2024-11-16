// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Pro.Common.SourceGenerator.Extensions
{
    public static class AttributeDataExtensions
    {
        /// <summary>
        /// Tries to get a given named argument value from an <see cref="AttributeData"/> instance, if present.
        /// </summary>
        /// <typeparam name="T">The type of argument to check.</typeparam>
        /// <param name="attributeData">The target <see cref="AttributeData"/> instance to check.</param>
        /// <param name="name">The name of the argument to check.</param>
        /// <param name="value">The resulting argument value, if present.</param>
        /// <returns>Whether or not <paramref name="attributeData"/> contains an argument named <paramref name="name"/> with a valid value.</returns>
        public static bool TryGetNamedArgument<T>(this AttributeData attributeData, string name, [NotNullWhen(true)] out T? value)
        {
            foreach (System.Collections.Generic.KeyValuePair<string, TypedConstant> properties in attributeData.NamedArguments)
            {
                if (properties.Key == name && properties.Value.Value is T extractedValue)
                {
                    value = extractedValue;
                    return true;
                }
            }

            value = default;

            return false;
        }

        /// <summary>
        /// Tries to get a given constructor value from an <see cref="AttributeData"/> instance, if present.
        /// </summary>
        /// <note>
        /// This method requires the parameter not to be a collection.
        /// </note>
        /// <typeparam name="T">The type of argument to check.</typeparam>
        /// <param name="attributeData">The target <see cref="AttributeData"/> instance to check.</param>
        /// <param name="name">The name of the argument to check.</param>
        /// <param name="value">The resulting argument value, if present.</param>
        /// <returns>Whether or not <paramref name="attributeData"/> contains an argument named <paramref name="name"/> with a valid value.</returns>
        public static bool TryGetConstructorArgument<T>(this AttributeData attributeData, int index, [NotNullWhen(true)] out T? value)
        {
            TypedConstant argument = attributeData.ConstructorArguments[index];
            if (argument.Value is T extractedValue)
            {
                value = extractedValue;
                return true;
            }

            value = default;

            return false;
        }

        /// <summary>
        /// Tries to get a given constructor values from an <see cref="AttributeData"/> instance, if present.
        /// </summary>
        /// <note>
        /// This method requires the parameter to be a collection of the same type.
        /// </note>
        /// <typeparam name="T">The type of argument to check.</typeparam>
        /// <param name="attributeData">The target <see cref="AttributeData"/> instance to check.</param>
        /// <param name="name">The name of the argument to check.</param>
        /// <param name="value">The resulting argument value, if present.</param>
        /// <returns>Whether or not <paramref name="attributeData"/> contains an argument named <paramref name="name"/> with a valid value.</returns>
        public static bool TryGetConstructorArguments<T>(this AttributeData attributeData, int index, [NotNullWhen(true)] out T[]? values)
        {
            TypedConstant argument = attributeData.ConstructorArguments[index];
            if (argument.Values.All(v => v.Value is T))
            {
                values = argument.Values.Select(v => v.Value).Cast<T>().ToArray();
                return true;
            }

            values = default;

            return false;
        }
    }
}
