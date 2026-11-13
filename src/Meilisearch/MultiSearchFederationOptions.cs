using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

using Meilisearch.Converters;

namespace Meilisearch
{
    /// <summary>
    /// Federation options in federated multi-index search
    /// </summary>
    public class MultiSearchFederationOptions
    {
        /// <summary>
        /// Number of documents to skip
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        /// <summary>
        /// Maximum number of documents returned
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        /// <summary>
        /// Facets requested for each index, keyed by index UID.
        /// </summary>
        [JsonPropertyName("facetsByIndex")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string[]> FacetsByIndex { get; set; }

        /// <summary>
        /// Options for merging facet distributions across indexes.
        /// </summary>
        [JsonPropertyName("mergeFacets")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public MultiSearchFederationMergeFacetsOptions MergeFacets { get; set; }
    }
}
