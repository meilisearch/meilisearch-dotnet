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
        /// <summary>
        /// Creates a new paginated search result of type <typeparamref name="T"/>
        /// </summary>
        /// <param name="hits"></param>
        /// <param name="hitsPerPage"></param>
        /// <param name="page"></param>
        /// <param name="totalHits"></param>
        /// <param name="totalPages"></param>
        /// <param name="facetDistribution"></param>
        /// <param name="processingTimeMs"></param>
        /// <param name="query"></param>
        /// <param name="matchesPosition"></param>
        /// <param name="facetStats"></param>
        /// <param name="indexUid"></param>
        public PaginatedSearchResult(
            IReadOnlyCollection<T> hits,
            int hitsPerPage,
            int page,
            int totalHits,
            int totalPages,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> facetDistribution,
            int processingTimeMs,
            string query,
            IReadOnlyDictionary<string, IReadOnlyCollection<MatchPosition>> matchesPosition,
            IReadOnlyDictionary<string, FacetStat> facetStats,
            string indexUid
        )
        {
            Hits = hits;
            HitsPerPage = hitsPerPage;
            Page = page;
            TotalHits = totalHits;
            TotalPages = totalPages;
            FacetDistribution = facetDistribution;
            ProcessingTimeMs = processingTimeMs;
            Query = query;
            MatchesPosition = matchesPosition;
            FacetStats = facetStats;
            IndexUid = indexUid;
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
        /// Total number of documents' pages.
        /// </summary>
        [JsonPropertyName("totalPages")]
        public int TotalPages { get; }

        /// <inheritdoc/>
        [JsonPropertyName("hits")]
        public IReadOnlyCollection<T> Hits { get; }

        /// <summary>
        /// Gets the total number of hits returned by the search.
        /// </summary>
        [JsonPropertyName("totalHits")]
        public int TotalHits { get; }

        /// <inheritdoc/>
        [JsonPropertyName("facetDistribution")]
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> FacetDistribution { get; }

        /// <inheritdoc/>
        [JsonPropertyName("processingTimeMs")]
        public int ProcessingTimeMs { get; }

        /// <inheritdoc/>
        [JsonPropertyName("query")]
        public string Query { get; }

        /// <inheritdoc/>
        [JsonPropertyName("_matchesPosition")]
        public IReadOnlyDictionary<string, IReadOnlyCollection<MatchPosition>> MatchesPosition { get; }

        /// <inheritdoc/>
        [JsonPropertyName("facetStats")]
        public IReadOnlyDictionary<string, FacetStat> FacetStats { get; }

        /// <inheritdoc/>
        [JsonPropertyName("indexUid")]
        public string IndexUid { get; }
    }
}
