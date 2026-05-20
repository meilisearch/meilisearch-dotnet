using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch.Converters
{
    internal class FilterableAttributeConverter : JsonConverter<FilterableAttribute>
    {
        public override FilterableAttribute Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return new FilterableAttribute { Attribute = reader.GetString() };
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var result = new FilterableAttribute();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return result;
                    }

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException("Expected property name in filterable attribute object.");
                    }

                    var propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case "attributePatterns":
                            result.AttributePatterns = JsonSerializer.Deserialize<IEnumerable<string>>(ref reader, options);
                            break;
                        case "features":
                            result.Features = JsonSerializer.Deserialize<FilterableAttributeFeatures>(ref reader, options);
                            break;
                        default:
                            reader.Skip();
                            break;
                    }
                }

                throw new JsonException("Unexpected end of JSON while reading filterable attribute object.");
            }

            throw new JsonException($"Unexpected token {reader.TokenType} when reading FilterableAttribute.");
        }

        public override void Write(Utf8JsonWriter writer, FilterableAttribute value, JsonSerializerOptions options)
        {
            if (value.Attribute != null)
            {
                writer.WriteStringValue(value.Attribute);
                return;
            }

            if (value.AttributePatterns == null)
            {
                throw new JsonException(
                    "FilterableAttribute must have either Attribute or AttributePatterns set.");
            }

            writer.WriteStartObject();

            writer.WritePropertyName("attributePatterns");
            JsonSerializer.Serialize(writer, value.AttributePatterns, options);

            if (value.Features != null)
            {
                writer.WritePropertyName("features");
                JsonSerializer.Serialize(writer, value.Features, options);
            }

            writer.WriteEndObject();
        }
    }
}
