using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    public class ISearchableJsonConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsInterface
                && typeToConvert.IsGenericType
                && (typeToConvert.GetGenericTypeDefinition() == typeof(ISearchable<>));
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var genericArgs = typeToConvert.GetGenericArguments();
            var converterType = typeof(ISearchableJsonConverter<>).MakeGenericType(
                genericArgs[0]
            );
            var converter = (JsonConverter)Activator.CreateInstance(converterType);
            return converter;
        }
    }

    public class ISearchableJsonConverter<T> : JsonConverter<ISearchable<T>> where T : class
    {
        public override ISearchable<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var document = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
            return document.TryGetProperty("page", out _) || document.TryGetProperty("hitsPerPage", out _)
                ? document.Deserialize<PaginatedSearchResult<T>>(options)
                : (ISearchable<T>)document.Deserialize<SearchResult<T>>(options);
        }

        public override void Write(Utf8JsonWriter writer, ISearchable<T> value, JsonSerializerOptions options)
        {
            if (value is PaginatedSearchResult<T> paginated)
            {
                JsonSerializer.Serialize(writer, paginated, options);
            }
            else if (value is SearchResult<T> normal)
            {
                JsonSerializer.Serialize(writer, normal, options);
            }
            else
            {
                JsonSerializer.Serialize(writer, (object)value, options);
            }
        }
    }
}
