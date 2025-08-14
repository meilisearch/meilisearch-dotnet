using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Embedder configuration.
    /// </summary>
    public class Embedder
    {
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        [JsonPropertyName("source")]
        public EmbedderSource Source { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        [JsonPropertyName("apiKey")]
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the document template.
        /// </summary>
        [JsonPropertyName("documentTemplate")]
        public string DocumentTemplate { get; set; }

        /// <summary>
        /// Gets or sets the document template max bytes.
        /// </summary>
        [JsonPropertyName("documentTemplateMaxBytes")]
        public int? DocumentTemplateMaxBytes { get; set; }

        /// <summary>
        /// Gets or sets the dimensions.
        /// </summary>
        [JsonPropertyName("dimensions")]
        public int? Dimensions { get; set; }

        /// <summary>
        /// Gets or sets the revision.
        /// </summary>
        [JsonPropertyName("revision")]
        public string Revision { get; set; }

        /// <summary>
        /// Gets or sets the distribution.
        /// </summary>
        [JsonPropertyName("distribution")]
        public EmbedderDistribution Distribution { get; set; }

        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        [JsonPropertyName("request")]
        public Dictionary<string, object> Request { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        [JsonPropertyName("response")]
        public Dictionary<string, object> Response { get; set; }

        /// <summary>
        /// Gets or sets whether the vectors should be compressed.
        /// </summary>
        [JsonPropertyName("binaryQuantized")]
        public bool? BinaryQuantized { get; set; }
    }
}
