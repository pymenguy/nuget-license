// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

using System;

namespace NuGetUtility.SourceGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class PropertyDescriptionAttribute : Attribute
    {
        public string Description { get; }
        public PropertyDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
