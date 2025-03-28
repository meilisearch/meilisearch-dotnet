using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch.Converters
{
    internal class EmbedderSourceConverter : JsonConverter<EmbedderSource>
    {
        public override EmbedderSource Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var enumValue = reader.GetString();
                if (Enum.TryParse<EmbedderSource>(enumValue, true, out var embedderSource))
                {
                    return embedderSource;
                }
            }

            return EmbedderSource.Unknown;
        }

        public override void Write(Utf8JsonWriter writer, EmbedderSource value, JsonSerializerOptions options)
        {
            string source;
            switch (value)
            {
                case EmbedderSource.OpenAi:
                    source = "openAi";
                    break;
                case EmbedderSource.HuggingFace:
                    source = "huggingFace";
                    break;
                case EmbedderSource.Ollama:
                    source = "ollama";
                    break;
                case EmbedderSource.Rest:
                    source = "rest";
                    break;
                case EmbedderSource.UserProvided:
                    source = "userProvided";
                    break;
                case EmbedderSource.Unknown:
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

            writer.WriteStringValue(source);
        }
    }
}
