using System.Text.Json;
using System;
using System.Text.Json.Serialization;

namespace Meilisearch.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class EmbedderSourceConverter: JsonConverter<EmbedderSource>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override EmbedderSource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            switch (value)
            {
                case "openAi":
                    return EmbedderSource.OpenAi;
                case "huggingFace":
                    return EmbedderSource.HuggingFace;
                case "ollama":
                    return EmbedderSource.Ollama;
                case "rest":
                    return EmbedderSource.Rest;
                case "userProvided":
                    return EmbedderSource.UserProvided;
                default:
                    return EmbedderSource.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, EmbedderSource value, JsonSerializerOptions options)
        {
            string stringValue;
            switch (value)
            {
                case EmbedderSource.OpenAi:
                    stringValue = "openAi";
                    break;
                case EmbedderSource.HuggingFace:
                    stringValue = "huggingFace";
                    break;
                case EmbedderSource.Ollama:
                    stringValue = "ollama";
                    break;
                case EmbedderSource.Rest:
                    stringValue = "rest";
                    break;
                case EmbedderSource.UserProvided:    
                    stringValue = "userProvided";
                    break;
                default:    
                    stringValue = string.Empty;
                    break;
            }
            writer.WriteStringValue(stringValue);
        }
    }
}
