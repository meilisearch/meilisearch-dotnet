using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch.Converters
{
    /// <summary>
    /// Always include property in json. MultiSearchFederationOptions will be serialized as "{}"
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MultiSearchFederationOptionsConverter : JsonConverter<MultiSearchFederationOptions>
    {
        public override MultiSearchFederationOptions Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<MultiSearchFederationOptions>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, MultiSearchFederationOptions value,
            JsonSerializerOptions options)
        {
            if (value == null || !HasAnyValueSet(value))
            {
                WriteEmptyObject(writer);
            }
            else
            {
                var sanitizedOptions =
                    RemoveSelfFromSerializerOptions(options); //Prevents getting stuck in a loop during serialization
                JsonSerializer.Serialize(writer, value, sanitizedOptions);
            }
        }

        private static JsonSerializerOptions RemoveSelfFromSerializerOptions(JsonSerializerOptions options)
        {
            var sanitizedOptions = new JsonSerializerOptions(options);
            sanitizedOptions.Converters.Remove(sanitizedOptions.Converters.First(c =>
                c.GetType() == typeof(MultiSearchFederationOptionsConverter)));
            return sanitizedOptions;
        }

        private static void WriteEmptyObject(Utf8JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteEndObject();
        }

        private bool HasAnyValueSet(MultiSearchFederationOptions value)
        {
            foreach (var property in
                     value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var propertyValue = property.GetValue(value);
                var defaultValue = GetDefaultValue(property.PropertyType);

                if (!Equals(propertyValue, defaultValue))
                {
                    return true;
                }
            }

            return false;
        }

        private object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
