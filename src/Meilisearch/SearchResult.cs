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
        /// <summary>
        /// Create a new search result where the documents are of type <typeparamref name="T"/>
        /// </summary>
        /// <param name="hits"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="estimatedTotalHits"></param>
        /// <param name="facetDistribution"></param>
        /// <param name="processingTimeMs"></param>
        /// <param name="query"></param>
        /// <param name="matchesPosition"></param>
        /// <param name="facetStats"></param>
        /// <param name="indexUid"></param>
        public SearchResult(IReadOnlyCollection<T> hits, int offset, int limit, int estimatedTotalHits,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> facetDistribution,
            int processingTimeMs, string query,
            IReadOnlyDictionary<string, IReadOnlyCollection<MatchPosition>> matchesPostion,
            IReadOnlyDictionary<string, FacetStat> facetStats,
            string indexUid)
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
            IndexUid = indexUid;
        }

        /// <inheritdoc/>
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
        public IReadOnlyDictionary<string, IReadOnlyCollection<MatchPosition>> MatchesPostion { get; }

        /// <inheritdoc/>
        [JsonPropertyName("facetStats")]
        public IReadOnlyDictionary<string, FacetStat> FacetStats { get; }

        /// <inheritdoc/>
        [JsonPropertyName("indexUid")]
        public string IndexUid { get; }
    }
}
