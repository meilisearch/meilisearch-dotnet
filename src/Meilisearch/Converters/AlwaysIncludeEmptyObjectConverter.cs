using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch.Converters
{
    /// <summary>
    /// Always include property in json. MultiSearchFederationOptions will be serialized as "{}"
    /// </summary>
    public class MultiSearchFederationOptionsConverter : JsonConverter<MultiSearchFederationOptions>
    {
        /// <summary>
        /// Would override the default read logic, but here we use the default
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override MultiSearchFederationOptions Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<MultiSearchFederationOptions>(ref reader, options);
        }

        /// <summary>
        /// Write json for MultiSearchFederationOptions and include it always as empty object
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, MultiSearchFederationOptions value,
            JsonSerializerOptions options)
        {
            if (value == null || !HasAnyValueSet(value))
            {
                WriteEmptyObject(writer);
            }
            else
            {
                JsonSerializer.Serialize(writer, value);
            }
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
