// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

using System.IO;
using NuGetUtility.LicenseValidator;
using NuGetUtility.Output.Table;

namespace NuGetUtility.Output.Markdown
{
    public class MarkdownOutputFormatter : TableOutputFormatter
    {
        public MarkdownOutputFormatter(bool printErrorsOnly, bool skipIgnoredPackages, IEnumerable<string>? ignoredColumns = null) : base(printErrorsOnly, skipIgnoredPackages, ignoredColumns)
        {
        }
        
        protected override async Task Print(Stream stream, IList<LicenseValidationResult> results, ColumnDefinition[] relevantColumns)
        {
            await MarkdownPrinterExtensions
                .Create(stream, relevantColumns.Select(d => d.Title))
                .FromValues(
                    results,
                    license => relevantColumns.Select(d => d.PropertyAccessor(license)))
                .Print();
        }

    }
}
