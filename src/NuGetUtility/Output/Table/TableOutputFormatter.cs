// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

using System.IO;
using NuGetUtility.LicenseValidator;

namespace NuGetUtility.Output.Table
{
    public class TableOutputFormatter : IOutputFormatter
    {
        private readonly bool _printErrorsOnly;
        private readonly bool _skipIgnoredPackages;
        private readonly string[]? _ignoredColumns;

        public TableOutputFormatter(bool printErrorsOnly, bool skipIgnoredPackages, IEnumerable<string>? ignoredColumns = null)
        {
            _printErrorsOnly = printErrorsOnly;
            _skipIgnoredPackages = skipIgnoredPackages;
            _ignoredColumns = ignoredColumns?.ToArray();
        }

        public async Task Write(Stream stream, IList<LicenseValidationResult> results)
        {
            var errorColumnDefinition = new ColumnDefinition("Error", license => license.ValidationErrors.Select(e => e.Error), license => license.ValidationErrors.Any());
            ColumnDefinition[] columnDefinitions = new[]
            {
                new ColumnDefinition("Package", license => license.PackageId, license => true, true),
                new ColumnDefinition("Version", license => license.PackageVersion, license => true, true),
                new ColumnDefinition("License Information Origin", license => license.LicenseInformationOrigin, license => true, true),
                new ColumnDefinition("License Expression", license => license.License, license => license.License != null),
                new ColumnDefinition("License Url", license => license.LicenseUrl, license => license.LicenseUrl != null),
                new ColumnDefinition("Copyright", license => license.Copyright, license => license.Copyright != null),
                new ColumnDefinition("Authors", license => license.Authors, license => license.Authors != null),
                new ColumnDefinition("Package Project Url",license => license.PackageProjectUrl, license => license.PackageProjectUrl != null),
                errorColumnDefinition,
                new ColumnDefinition("Error Context", license => license.ValidationErrors.Select(e => e.Context), license => license.ValidationErrors.Any()),
            };

            foreach (LicenseValidationResult license in results)
            {
                foreach (ColumnDefinition? definition in columnDefinitions)
                {
                    definition.Enabled |= definition.IsRelevant(license);
                }
            }

            if (_ignoredColumns is not null)
            {
                foreach (ColumnDefinition? definition in columnDefinitions)
                {
                    definition.Enabled &= !_ignoredColumns.Contains(definition.Title);
                }
            }


            if (_printErrorsOnly)
            {
                results = results.Where(r => r.ValidationErrors.Any()).ToList();
            }
            else if (_skipIgnoredPackages)
            {
                results = results.Where(r => r.LicenseInformationOrigin != LicenseInformationOrigin.Ignored).ToList();
            }

            ColumnDefinition[] relevantColumns = columnDefinitions.Where(c => c.Enabled).ToArray();
            await Print(stream, results, relevantColumns);
        }

        protected virtual Task Print(Stream stream, IList<LicenseValidationResult> results, ColumnDefinition[] relevantColumns)
        {
            return TablePrinterExtensions
                .Create(stream, relevantColumns.Select(d => d.Title))
                .FromValues(
                    results,
                    license => relevantColumns.Select(d => d.PropertyAccessor(license)))
                .Print();
        }

        protected sealed record ColumnDefinition(string Title, Func<LicenseValidationResult, object?> PropertyAccessor, Func<LicenseValidationResult, bool> IsRelevant, bool Enabled = false)
        {
            public bool Enabled { get; set; } = Enabled;
        }
    }
}
