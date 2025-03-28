using System.Text.Json.Serialization;

using Meilisearch.Converters;

namespace Meilisearch
{
    /// <summary>
    /// Embedder source.
    /// </summary>
    [JsonConverter(typeof(EmbedderSourceConverter))]
    public enum EmbedderSource
    {
        /// <summary>
        /// OpenAI
        /// </summary>
        OpenAi,

        /// <summary>
        /// Hugging Face
        /// </summary>
        HuggingFace,

        /// <summary>
        /// Ollama
        /// </summary>
        Ollama,

        /// <summary>
        /// REST
        /// </summary>
        Rest,

        /// <summary>
        /// User-provided
        /// </summary>
        UserProvided,

        /// <summary>
        /// Unknown
        /// </summary>
        Unknown
    }
}
