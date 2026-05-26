using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch.Converters
{
    public class EnumToCamelCaseConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
    {
        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException($"Expected string for ConditionType, but found {reader.TokenType}.");

            var enumString = reader.GetString();
            return Enum.TryParse<TEnum>(enumString, true, out var enumValue)
                ? enumValue
                : throw new JsonException($"Invalid {typeof(TEnum)} value: '{enumValue}'.");
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            var valueStr = JsonNamingPolicy.CamelCase.ConvertName(value.ToString());
            writer.WriteStringValue(valueStr);
        }
    }
}
