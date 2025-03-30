using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Retrieve documents similar to a specific search result.
    /// </summary>
    public class SimilarDocumentsSearch
    {
        /// <summary>
        /// Identifier of the target document.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Embedder to use when computing recommendations.
        /// </summary>
        [JsonPropertyName("embedder")]
        public string Embedder { get; set; } = "default";

        /// <summary>
        /// Attributes to display in the returned documents.
        /// </summary>
        [JsonPropertyName("attributesToRetrieve")]
        public IEnumerable<string> AttributesToRetrieve { get; set; } = new[] { "*" };

        /// <summary>
        /// Number of documents to skip.
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; set; } = 0;

        /// <summary>
        /// Maximum number of documents returned.
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; } = 20;

        /// <summary>
        /// Filter queries by an attribute's value.
        /// </summary>
        [JsonPropertyName("filter")]
        public string Filter { get; set; } = null;

        /// <summary>
        /// Display the global ranking score of a document.
        /// </summary>
        [JsonPropertyName("showRankingScore")]
        public bool ShowRankingScore { get; set; }

        /// <summary>
        /// Display detailed ranking score information.
        /// </summary>
        [JsonPropertyName("showRankingScoreDetails")]
        public bool ShowRankingScoreDetails { get; set; }

        /// <summary>
        /// Exclude results with low ranking scores.
        /// </summary>
        [JsonPropertyName("rankingScoreThreshold")]
        public decimal? RankingScoreThreshold { get; set; }

        /// <summary>
        /// Return document vector data.
        /// </summary>
        [JsonPropertyName("retrieveVectors")]
        public bool RetrieveVectors { get; set; }
    }
}
