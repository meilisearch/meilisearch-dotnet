using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Wrapper for Search Results.
    /// </summary>
    /// <typeparam name="T">Hit type.</typeparam>
    public class SearchResult<T> : ISearchable<T>
    {
        public SearchResult(IReadOnlyCollection<T> hits, int offset, int limit, int estimatedTotalHits,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> facetDistribution,
            int processingTimeMs, string query,
            IReadOnlyDictionary<string, IReadOnlyCollection<MatchPosition>> matchesPostion,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, float>> facetStats)
        {
            Hits = hits;
            Offset = offset;
            Limit = limit;
            EstimatedTotalHits = estimatedTotalHits;
            FacetDistribution = facetDistribution;
            ProcessingTimeMs = processingTimeMs;
            Query = query;
            MatchesPostion = matchesPostion;
            FacetStats = facetStats;
        }

        /// <summary>
        /// Results of the query.
        /// </summary>
        [JsonPropertyName("hits")]
        public IReadOnlyCollection<T> Hits { get; }

        /// <summary>
        /// Number of documents skipped.
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; }

        /// <summary>
        /// Number of documents to take.
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; }

        /// <summary>
        /// Gets the estimated total number of hits returned by the search.
        /// </summary>
        [JsonPropertyName("estimatedTotalHits")]
        public int EstimatedTotalHits { get; }

        /// <summary>
        /// Returns the number of documents matching the current search query for each given facet.
        /// </summary>
        [JsonPropertyName("facetDistribution")]
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> FacetDistribution { get; }

        /// <summary>
        /// Processing time of the query.
        /// </summary>
        [JsonPropertyName("processingTimeMs")]
        public int ProcessingTimeMs { get; }

        /// <summary>
        /// Query originating the response.
        /// </summary>
        [JsonPropertyName("query")]
        public string Query { get; }

        /// <summary>
        /// Contains the location of each occurrence of queried terms across all fields.
        /// </summary>
        [JsonPropertyName("_matchesPosition")]
        public IReadOnlyDictionary<string, IReadOnlyCollection<MatchPosition>> MatchesPostion { get; }

        /// <summary>
        /// Returns the numeric min and max values per facet of the hits returned by the search query.
        /// </summary>
        [JsonPropertyName("facetStats")]
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, float>> FacetStats { get; }
    }
}
