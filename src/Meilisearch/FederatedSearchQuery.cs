using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Search query for federated multi-index search.
    /// <remarks>
    /// Pagination is federation-wide: pass <see cref="MultiSearchFederationOptions.Limit"/> and
    /// <see cref="MultiSearchFederationOptions.Offset"/> on the query's federation options.
    /// Meilisearch rejects the per-query <c>limit</c>, <c>offset</c>, <c>page</c> and
    /// <c>hitsPerPage</c> parameters in a federated request, so they are not exposed here.
    /// </remarks>
    /// </summary>
    public class FederatedSearchQuery : SearchQueryBase
    {
        /// <summary>
        /// Per-query federated search options: weight, remote and query position.
        /// </summary>
        [JsonPropertyName("federationOptions")]
        public FederatedSearchQueryOptions FederationOptions { get; set; }

        /// <summary>
        /// Sets distinct attribute at search time.
        /// </summary>
        [JsonPropertyName("distinct")]
        public string Distinct { get; set; }

        /// <summary>
        /// Gets or sets rankingScoreThreshold, a number between 0.0 and 1.0.
        /// </summary>
        [JsonPropertyName("rankingScoreThreshold")]
        public decimal? RankingScoreThreshold { get; set; }

        /// <summary>
        /// Gets or sets the hybrid search settings.
        /// </summary>
        [JsonPropertyName("hybrid")]
        public HybridSearch Hybrid { get; set; }

        /// <summary>
        /// Gets or sets the vector. Requires <see cref="Hybrid"/> to be set as well.
        /// </summary>
        [JsonPropertyName("vector")]
        public double[] Vector { get; set; }

        /// <summary>
        /// Gets or sets whether to retrieve vectors.
        /// </summary>
        [JsonPropertyName("retrieveVectors")]
        public bool? RetrieveVectors { get; set; }
    }
}
