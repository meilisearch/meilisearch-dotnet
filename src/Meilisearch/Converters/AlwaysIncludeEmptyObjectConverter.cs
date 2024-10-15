using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch.Converters
{
    /// <summary>
    /// Always include Property in json objects will be serialized as "{}"
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
                // Write an empty object if the value is null
                writer.WriteStartObject();
                writer.WriteEndObject();
            }
            else
            {
                var newOptions = new JsonSerializerOptions(options);
                newOptions.Converters.Remove(newOptions.Converters.First(c =>
                    c.GetType() == typeof(MultiSearchFederationOptionsConverter)));

                // Serialize the value as usual
                JsonSerializer.Serialize(writer, value, newOptions);
            }
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
