using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch.Converters
{
    public class SortFacetValuesConverter : JsonConverter<SortFacetValuesByType>
    {
        public override SortFacetValuesByType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var enumValue = reader.GetString();
                if (Enum.TryParse<SortFacetValuesByType>(enumValue, true, out var sortFacetValues))
                {
                    return sortFacetValues;
                }
            }

            // If we reach here, it means we encountered an unknown value, so we'll use meilisearch default of Alpha
            return SortFacetValuesByType.Alpha;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Writes the facet sort value as a JSON string, preserving the API's <c>sampledCount</c> casing.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, SortFacetValuesByType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value == SortFacetValuesByType.SampledCount ? "sampledCount" : value.ToString().ToLower());
        }
    }
}
