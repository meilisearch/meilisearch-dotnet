using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Wrapper for FacetSearchResponse
    /// </summary>
    public class FacetSearchResult
    {
        /// <summary>
        /// Gets or sets the facetHits property
        /// </summary>
        [JsonPropertyName("facetHits")]
        public IEnumerable<FacetHit> FacetHits { get; set; }

        /// <summary>
        /// Gets or sets the facet query
        /// </summary>
        [JsonPropertyName("facetQuery")]
        public string FacetQuery { get; set; }

        /// <summary>
        /// Gets or sets the processingTimeMs property
        /// </summary>
        [JsonPropertyName("processingTimeMs")]
        public int ProcessingTimeMs { get; set; }

        /// <summary>
        /// Wrapper for Facet Hit
        /// </summary>
        public class FacetHit
        {
            /// <summary>
            /// Gets or sets the value property
            /// </summary>
            [JsonPropertyName("value")]
            public string Value { get; set; }

            /// <summary>
            /// Gets or sets the count property
            /// </summary>
            [JsonPropertyName("count")]
            public int Count { get; set; }
        }
    }
}
