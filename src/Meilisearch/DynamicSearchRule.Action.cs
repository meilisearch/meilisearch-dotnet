using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Actions applied when a dynamic search rule matches.
    /// </summary>
    public class DSRActions
    {
        /// <summary>
        /// Documents to pin to a fixed result position.
        /// </summary>
        [JsonPropertyName("pin")]
        public IEnumerable<DSRPin> Pin { get; set; }

        /// <summary>
        /// Documents whose relevancy score is scaled.
        /// Weight greater than 1 boosts, less than 1 demotes, and 0 hides.
        /// </summary>
        [JsonPropertyName("scale")]
        public IEnumerable<DSRScale> Scale { get; set; }
    }

    /// <summary>
    /// Pins a document to a fixed result position.
    /// </summary>
    public class DSRPin
    {
        /// <summary>
        /// Document id to pin.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Zero-based position where the document is pinned.
        /// </summary>
        [JsonPropertyName("position")]
        public int Position { get; set; }

        /// <summary>
        /// Index the pin applies to. When null or omitted, the pin applies to any index containing the document id.
        /// </summary>
        [JsonPropertyName("indexUid")]
        public string IndexUid { get; set; }
    }

    /// <summary>
    /// Scales selected documents' relevancy.
    /// </summary>
    public class DSRScale
    {
        /// <summary>
        /// Multiplicative weight applied to the selected documents' relevancy score.
        /// </summary>
        [JsonPropertyName("weight")]
        public double Weight { get; set; }

        /// <summary>
        /// Document ids to scale.
        /// </summary>
        [JsonPropertyName("ids")]
        public IEnumerable<string> Ids { get; set; }

        /// <summary>
        /// Filter selecting documents to scale. A string or a nested filter array, same shape as search filters.
        /// </summary>
        [JsonPropertyName("filter")]
        public dynamic Filter { get; set; }

        /// <summary>
        /// Index the scale applies to. When null or omitted, the scale applies to any index.
        /// </summary>
        [JsonPropertyName("indexUid")]
        public string IndexUid { get; set; }
    }
}
