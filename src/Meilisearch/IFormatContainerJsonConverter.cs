using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    public class IFormatContainerJsonConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsInterface
                && typeToConvert.IsGenericType
                && (typeToConvert.GetGenericTypeDefinition() == typeof(IFormatContainer<,>));
        }

        public override JsonConverter CreateConverter(
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var genericArgs = typeToConvert.GetGenericArguments();
            var converterType = typeof(IFormatContainerJsonConverter<,>).MakeGenericType(
                genericArgs[0],
                genericArgs[1]
            );
            var converter = (JsonConverter)Activator.CreateInstance(converterType);
            return converter;
        }
    }

    public class IFormatContainerJsonConverter<TOriginal, TFormatted>
        : JsonConverter<IFormatContainer<TOriginal, TFormatted>>
        where TFormatted : class
        where TOriginal : class
    {
        public override IFormatContainer<TOriginal, TFormatted> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var document = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
            var original = document.Deserialize<TOriginal>(options);

            if (document.TryGetProperty("_formatted", out var formattedElement))
            {
                var formatted = formattedElement.Deserialize<TFormatted>(options);
                return new DefaultFormattable<TOriginal, TFormatted>(original, formatted);
            }
            return new DefaultFormattable<TOriginal, TFormatted>(original, null);
        }

        public override void Write(
            Utf8JsonWriter writer,
            IFormatContainer<TOriginal, TFormatted> value,
            JsonSerializerOptions options
        )
        {
            throw new NotImplementedException();
        }
    }
}
