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
        /// Facets to return per index
        /// </summary>
        [JsonPropertyName("facetsByIndex")]
        public Dictionary<string, IEnumerable<string>> FacetsByIndex { get; set; }

        /// <summary>
        /// Options for merging facets across indexes
        /// </summary>
        [JsonPropertyName("mergeFacets")]
        public MergeFacets MergeFacets { get; set; }

        /// <summary>
        /// Attribute whose value must be different for each returned document
        /// </summary>
        [JsonPropertyName("distinct")]
        public string Distinct { get; set; }
    }

    /// <summary>
    /// Options for merging facets in federated search
    /// </summary>
    public class MergeFacets
    {
        /// <summary>
        /// Maximum number of facet values returned for each facet
        /// </summary>
        [JsonPropertyName("maxValuesPerFacet")]
        public int MaxValuesPerFacet { get; set; }
    }
}
