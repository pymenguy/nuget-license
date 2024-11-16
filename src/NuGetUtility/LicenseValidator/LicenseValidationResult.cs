// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

using NuGetUtility.SourceGenerator.Attributes;
using NuGetUtility.Wrapper.NuGetWrapper.Versioning;

namespace NuGetUtility.LicenseValidator
{
    [GeneratePropertyEnum]
    public record LicenseValidationResult(
        string PackageId,
        INuGetVersion PackageVersion,
        [property: PropertyDescription("Package Project Url")]
        string? PackageProjectUrl,
        string? License,
        string? LicenseUrl,
        [property: PropertyDescription("Copyright")]
        string? Copyright,
        [property: PropertyDescription("Authors")]
        string? Authors,
        LicenseInformationOrigin LicenseInformationOrigin,
        List<ValidationError>? ValidationErrors = null)
    {
        public List<ValidationError> ValidationErrors { get; } = ValidationErrors ?? new List<ValidationError>();

        public string? License { get; set; } = License;
        [PropertyDescription("License Url")]
        public string? LicenseUrl { get; set; } = LicenseUrl;
        [PropertyDescription("License Information Origin")]
        public LicenseInformationOrigin LicenseInformationOrigin { get; set; } = LicenseInformationOrigin;
    }
}
