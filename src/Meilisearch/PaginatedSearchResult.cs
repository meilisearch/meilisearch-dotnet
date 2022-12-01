using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Wrapper for Search Results with finite pagination.
    /// </summary>
    /// <typeparam name="T">Hit type.</typeparam>
    public class PaginatedSearchResult<T> : ISearchable<T>
    {
        public PaginatedSearchResult(IReadOnlyCollection<T> hits, int hitsPerPage, int page, int total,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> facetDistribution,
            int processingTimeMs, string query,
            IReadOnlyDictionary<string, IReadOnlyCollection<MatchPosition>> matchesPostion)
        {
            Hits = hits;
            HitsPerPage = hitsPerPage;
            Page = page;
            Total = total;
            FacetDistribution = facetDistribution;
            ProcessingTimeMs = processingTimeMs;
            Query = query;
            MatchesPostion = matchesPostion;
        }

        /// <summary>
        /// Number of documents each page.
        /// </summary>
        [JsonPropertyName("hitsPerPage")]
        public int HitsPerPage { get; }

        /// <summary>
        /// Number of documents to take.
        /// </summary>
        [JsonPropertyName("page")]
        public int Page { get; }

        /// <summary>
        /// Results of the query.
        /// </summary>
        [JsonPropertyName("hits")]
        public IReadOnlyCollection<T> Hits { get; }

        /// <summary>
        /// Gets the estimated total number of hits returned by the search.
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; }

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
    }
}
