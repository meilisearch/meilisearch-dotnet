using System.Text.Json.Serialization;

namespace Meilisearch
{
    public class HybridSearch
    {
        /// <summary>
        /// Gets or sets the embedder.
        /// </summary>
        [JsonPropertyName("embedder")]
        public string Embedder { get; set; }

        /// <summary>
        /// Gets or sets the semantic ratio.
        /// </summary>
        [JsonPropertyName("semanticRatio")]
        public double SemanticRatio { get; set; }
    }
}
