// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Pro.Common.SourceGenerator
{
    [Generator(LanguageNames.CSharp)]
    public class AttributesGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(i =>
            {
                System.Reflection.Assembly assembly = typeof(AttributesGenerator).Assembly;
                foreach (string? name in assembly.GetManifestResourceNames())
                {
                    using System.IO.Stream input = assembly.GetManifestResourceStream(name);
                    i.AddSource(name.Replace(".cs", ".g.cs"), SourceText.From(input, Encoding.UTF8, canBeEmbedded: true));
                }
            });
        }
    }
}
