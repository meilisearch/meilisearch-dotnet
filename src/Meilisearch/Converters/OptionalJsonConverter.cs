using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch.Converters
{
    public struct Optional<T>
    {
        public bool HasValue { get; private set; }

        private T _value;

        public T Value
        {
            get =>
                HasValue
                    ? _value
                    : throw new InvalidOperationException("Value is not set");
            set
            {
                HasValue = true;
                _value = value;
            }
        }

        public static Optional<T> None => default;
        public static implicit operator Optional<T>(T value) => new Optional<T> { Value = value };
    }

    public class OptionalJsonConverter<T> : JsonConverter<Optional<T>>
    {
        public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => JsonSerializer.Deserialize<T>(ref reader, options);

        public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
        {
            if (!value.HasValue)
            {
                writer.WriteNullValue();
                return;
            }

            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }

    public class OptionalJsonConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
            => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Optional<>);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(OptionalJsonConverter<>)
                .MakeGenericType(typeToConvert.GetGenericArguments()[0]);
            return (JsonConverter)Activator.CreateInstance(converterType);
        }
    }
}
