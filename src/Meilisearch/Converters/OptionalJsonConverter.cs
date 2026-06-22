using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch.Converters
{
    /// <summary>
    /// Class that helps to differ nullable and not provided values
    /// </summary>
    /// <typeparam name="T">Possible type of provided value</typeparam>
    public struct Optional<T>
    {
        /// <summary>
        /// Indicates whether a value was explicitly provided
        /// </summary>
        public bool HasValue { get; private set; }

        private T _value;

        /// <summary>
        /// Provided value
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws when no value has been provided</exception>
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

        /// <summary>
        /// Instance with no value provided
        /// </summary>
        public static Optional<T> None => default;

        /// <summary>
        /// Implicitly converts a value into an <see cref="Optional{T}"/> instance
        /// </summary>
        /// <param name="value">Provided value</param>
        /// <returns>An <see cref="Optional{T}"/> containing the specified value</returns>
        public static implicit operator Optional<T>(T value) => new Optional<T> { Value = value };
    }

    /// <summary>
    /// Converter for <see cref="Optional{T}"/>
    /// </summary>
    /// <typeparam name="T">Possible type of provided value</typeparam>
    public class OptionalJsonConverter<T> : JsonConverter<Optional<T>>
    {
        /// <inheritdoc/>
        public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => JsonSerializer.Deserialize<T>(ref reader, options);

        /// <inheritdoc/>
        /// <summary>
        /// Writes given <paramref name="value"/> as JSON into <paramref name="writer"/>. If the value was not provided, writes <c>null</c>.
        /// Recommended to use with JsonIgnoreAttribute with ignoring when value is default:
        /// <code>[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]</code>
        /// </summary>
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

    /// <summary>
    /// Factory that automatically creates converters for all <see cref="Optional{T}"/>
    /// </summary>
    public class OptionalJsonConverterFactory : JsonConverterFactory
    {
        /// <inheritdoc/>
        public override bool CanConvert(Type typeToConvert)
            => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Optional<>);

        /// <inheritdoc/>
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var converterType = typeof(OptionalJsonConverter<>)
                .MakeGenericType(typeToConvert.GetGenericArguments()[0]);
            return (JsonConverter)Activator.CreateInstance(converterType);
        }
    }
}
