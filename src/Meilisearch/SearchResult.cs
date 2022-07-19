using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Wrapper for Search Results.
    /// </summary>
    /// <typeparam name="T">Hit type.</typeparam>
    public class SearchResult<T>
    {
        public SearchResult(IReadOnlyCollection<T> hits, int offset, int limit, int estimatedTotalHits,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> facetDistribution,
            int processingTimeMs, string query,
            IReadOnlyDictionary<string, IReadOnlyCollection<MatchPosition>> matchesPostion)
        {
            Hits = hits;
            Offset = offset;
            Limit = limit;
            EstimatedTotalHits = estimatedTotalHits;
            FacetDistribution = facetDistribution;
            ProcessingTimeMs = processingTimeMs;
            Query = query;
            MatchesPostion = matchesPostion;
        }

        /// <summary>
        /// Results of the query.
        /// </summary>
        public IReadOnlyCollection<T> Hits { get; }

        /// <summary>
        /// Number of documents skipped.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Number of documents to take.
        /// </summary>
        public int Limit { get; }

        /// <summary>
        /// Gets the estimated total number of hits returned by the search.
        /// </summary>
        public int EstimatedTotalHits { get; }

        /// <summary>
        /// Returns the number of documents matching the current search query for each given facet.
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> FacetDistribution { get; }

        /// <summary>
        /// Processing time of the query.
        /// </summary>
        public int ProcessingTimeMs { get; }

        /// Query originating the response.
        /// </summary>
        public string Query { get; }

        /// <summary>
        /// Contains the location of each occurrence of queried terms across all fields.
        /// </summary>
        [JsonPropertyName("_matchesPosition")]
        public IReadOnlyDictionary<string, IReadOnlyCollection<MatchPosition>> MatchesPostion { get; }
    }

    public class MatchPosition
    {
        public MatchPosition(int start, int length)
        {
            Start = start;
            Length = length;
        }

        /// <summary>
        /// The beginning of a matching term within a field.
        /// WARNING: This value is in bytes and not the number of characters. For example, ü represents two bytes but one character.
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// The length of a matching term within a field.
        /// WARNING: This value is in bytes and not the number of characters. For example, ü represents two bytes but one character.
        /// </summary>
        public int Length { get; }
    }
}
