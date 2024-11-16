// Licensed to the projects contributors.
// The license conditions are provided in the LICENSE file located in the project root

using System.Text.Json;
using System.Text.Json.Serialization;
using NuGetUtility.LicenseValidator;

namespace NuGetUtility.Serialization
{
    public class ValidatedLicenseJsonConverterWithOmittingIgnoreListAndEmptyErrorList : JsonConverter<LicenseValidationResult>
    {
        private readonly IReadOnlyCollection<string> _omittedProperties;

        public ValidatedLicenseJsonConverterWithOmittingIgnoreListAndEmptyErrorList(IReadOnlyCollection<string> omittedProperties)
        {
            _omittedProperties = omittedProperties;
        }

        public override LicenseValidationResult? Read(ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
        public override void Write(Utf8JsonWriter writer, LicenseValidationResult value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (System.Reflection.PropertyInfo propertyInfo in value.GetType().GetProperties())
            {
                if ((propertyInfo.Name == nameof(value.ValidationErrors) && !value.ValidationErrors.Any())
                    || _omittedProperties.Contains(propertyInfo.Name))
                {
                    continue;
                }
                object? writeValue = propertyInfo.GetValue(value);
                if (writeValue != null)
                {
                    writer.WritePropertyName(propertyInfo.Name);
                    JsonSerializer.Serialize(writer, propertyInfo.GetValue(value), options);
                }
            }
            writer.WriteEndObject();
        }
    }
}
