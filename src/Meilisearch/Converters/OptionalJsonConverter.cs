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
        /// Indicates whether value was provided or not
        /// </summary>
        public bool HasValue { get; private set; }

        private T _value;

        /// <summary>
        /// Provided value
        /// </summary>
        /// <exception cref="InvalidOperationException">Throws when value was not provided</exception>
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
        /// Default value of Optional&lt;<typeparamref name="T"/>&gt;
        /// </summary>
        public static Optional<T> None => default;

        /// <summary>
        /// Implicit converter of provided <typeparamref name="T"/> type value into Optional&lt;<typeparamref name="T"/>&gt;
        /// </summary>
        /// <param name="value">Provided value</param>
        /// <returns></returns>
        public static implicit operator Optional<T>(T value) => new Optional<T> { Value = value };
    }

    /// <summary>
    /// Converter for Optional&lt;<typeparamref name="T"/>&gt;
    /// </summary>
    /// <typeparam name="T">Possible type of provided value</typeparam>
    public class OptionalJsonConverter<T> : JsonConverter<Optional<T>>
    {
        /// <inheritdoc/>
        public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => JsonSerializer.Deserialize<T>(ref reader, options);

        /// <inheritdoc/>
        /// <summary>
        /// Writes given <paramref name="value"/> as JSON into <paramref name="writer"/>. Writes null, when value wasn't provided.
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
    /// Factory for automatic creation of OptionalJsonConverter&lt;T&gt; for all incoming types of Optional&lt;T&gt; implicitly
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
