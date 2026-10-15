using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Settings for merging facet distributions across federated search indexes.
    /// </summary>
    public class MultiSearchFederationMergeFacetsOptions
    {
        /// <summary>
        /// Maximum number of values returned for each merged facet.
        /// </summary>
        [JsonPropertyName("maxValuesPerFacet")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? MaxValuesPerFacet { get; set; }
    }
}
