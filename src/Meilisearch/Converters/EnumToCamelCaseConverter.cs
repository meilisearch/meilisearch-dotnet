using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch.Converters
{
    /// <summary>
    /// Converts enum values of <typeparamref name="TEnum"/> into CamelCase string laterals
    /// </summary>
    /// <typeparam name="TEnum">Type of enum to convert</typeparam>
    public class EnumToCamelCaseConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
    {
        /// <inheritdoc/>
        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException($"Expected string for {typeof(TEnum).Name}, but found {reader.TokenType}.");

            var enumString = reader.GetString();
            return Enum.TryParse<TEnum>(enumString, true, out var enumValue)
                ? enumValue
                : throw new JsonException($"Invalid {typeof(TEnum).Name} value: '{enumString}'.");
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            var valueStr = JsonNamingPolicy.CamelCase.ConvertName(value.ToString());
            writer.WriteStringValue(valueStr);
        }
    }
}
