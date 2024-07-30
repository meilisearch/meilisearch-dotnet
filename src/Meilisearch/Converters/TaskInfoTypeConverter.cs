using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Meilisearch.Converters
{
    public class TaskInfoTypeConverter : JsonConverter<TaskInfoType>
    {
        public override TaskInfoType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string enumValue = reader.GetString();
                if (Enum.TryParse<TaskInfoType>(enumValue, true, out var taskInfoType))
                {
                    return taskInfoType;
                }
            }

            // If we reach here, it means we encountered an unknown value
            return TaskInfoType.Unknown;
        }

        public override void Write(Utf8JsonWriter writer, TaskInfoType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
