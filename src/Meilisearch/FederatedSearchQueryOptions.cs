using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Per-query federation options in federated multi-index search.
    /// <remarks>
    /// Distinct from <see cref="MultiSearchFederationOptions"/>, which carries the federation-wide
    /// <c>limit</c> and <c>offset</c>. Meilisearch accepts only <c>weight</c>, <c>remote</c> and
    /// <c>queryPosition</c> inside a query's own <c>federationOptions</c>.
    /// </remarks>
    /// </summary>
    public class FederatedSearchQueryOptions
    {
        /// <summary>
        /// Factor applied to this query's ranking score when merging results, defaults to 1.0.
        /// </summary>
        [JsonPropertyName("weight")]
        public decimal? Weight { get; set; }

        /// <summary>
        /// Name of the remote instance to run this query on, as declared in the network settings.
        /// </summary>
        [JsonPropertyName("remote")]
        public string Remote { get; set; }

        /// <summary>
        /// Position reported as <c>_federation.queriesPosition</c> for this query's hits.
        /// </summary>
        [JsonPropertyName("queryPosition")]
        public int? QueryPosition { get; set; }
    }
}
