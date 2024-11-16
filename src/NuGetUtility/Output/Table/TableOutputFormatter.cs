// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

using NuGetUtility.LicenseValidator;

namespace NuGetUtility.Output.Table
{
    public class TableOutputFormatter : IOutputFormatter
    {
        private readonly bool _printErrorsOnly;
        private readonly bool _skipIgnoredPackages;
        private readonly HashSet<LicenseValidationResultProperties> _ignoredColumns;

        public TableOutputFormatter(bool printErrorsOnly, bool skipIgnoredPackages, IEnumerable<LicenseValidationResultProperties> ignoredColumns)
        {
            _printErrorsOnly = printErrorsOnly;
            _skipIgnoredPackages = skipIgnoredPackages;
            _ignoredColumns = ignoredColumns.ToHashSet();
        }

        public async Task Write(Stream stream, IList<LicenseValidationResult> results)
        {
            ColumnDefinition[] columnDefinitions = new[]
            {
                new ColumnDefinition("Package", license => license.PackageId, license => true, true),
                new ColumnDefinition("Version", license => license.PackageVersion, license => true, true),
                GetIgnorableColumnDefinition(LicenseValidationResultProperties.LicenseInformationOrigin,license => license.LicenseInformationOrigin),
                new ColumnDefinition("License Expression", license => license.License, license => license.License != null),
                GetIgnorableColumnDefinition(LicenseValidationResultProperties.LicenseUrl,license => license.LicenseUrl),
                GetIgnorableColumnDefinition(LicenseValidationResultProperties.Copyright,license => license.Copyright),
                GetIgnorableColumnDefinition(LicenseValidationResultProperties.Authors,license => license.Authors),
                GetIgnorableColumnDefinition(LicenseValidationResultProperties.PackageProjectUrl,license => license.PackageProjectUrl),
                new ColumnDefinition("Error", license => license.ValidationErrors.Select(e => e.Error), license => license.ValidationErrors.Any()),
                new ColumnDefinition("Error Context", license => license.ValidationErrors.Select(e => e.Context), license => license.ValidationErrors.Any()),
            };

            foreach (LicenseValidationResult license in results)
            {
                foreach (ColumnDefinition? definition in columnDefinitions)
                {
                    definition.Enabled |= definition.IsRelevant(license);
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
            await TablePrinterExtensions
                .Create(stream, relevantColumns.Select(d => d.Title))
                .FromValues(
                    results,
                    license => relevantColumns.Select(d => d.PropertyAccessor(license)))
                .Print();
        }

        private static ColumnDefinition GetIgnorableColumnDefinition(LicenseValidationResultProperties property, Func<LicenseValidationResult, object?> propertyAccess)
        {
            return new ColumnDefinition(property.GetDescription(), propertyAccess, license => propertyAccess(license) != null);
        }

        private sealed record ColumnDefinition(string Title, Func<LicenseValidationResult, object?> PropertyAccessor, Func<LicenseValidationResult, bool> IsRelevant, bool Enabled = false)
        {
            public bool Enabled { get; set; } = Enabled;
        }
    }
}
