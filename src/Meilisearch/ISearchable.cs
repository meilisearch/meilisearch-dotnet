using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Wrapper for Search Results.
    /// </summary>
    /// <typeparam name="T">Hit type.</typeparam>
    public interface ISearchable<T>
    {
        /// <summary>
        /// Results of the query.
        /// </summary>
        [JsonPropertyName("hits")]
        IReadOnlyCollection<T> Hits { get; }

        /// <summary>
        /// Returns the number of documents matching the current search query for each given facet.
        /// </summary>
        [JsonPropertyName("facetDistribution")]
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> FacetDistribution { get; }

        /// <summary>
        /// Processing time of the query.
        /// </summary>
        [JsonPropertyName("processingTimeMs")]
        int ProcessingTimeMs { get; }

        /// <summary>
        /// Query originating the response.
        /// </summary>
        [JsonPropertyName("query")]
        string Query { get; }

        /// <summary>
        /// Contains the location of each occurrence of queried terms across all fields.
        /// </summary>
        [JsonPropertyName("_matchesPosition")]
        IReadOnlyDictionary<string, IReadOnlyCollection<MatchPosition>> MatchesPostion { get; }

        /// <summary>
        /// Returns the numeric min and max values per facet of the hits returned by the search query.
        /// </summary>
        [JsonPropertyName("facetStats")]
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, float>> FacetStats { get; }
    }
}
