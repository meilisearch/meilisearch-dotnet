using System;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Search query for similar documents.
    /// </summary>
    public class SimilarDocumentsQuery
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SimilarDocumentsQuery"/> class.
        /// </summary>
        /// <param name="id"></param>
        public SimilarDocumentsQuery(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            Id = id;
        }

        /// <summary>
        /// Gets the document id.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; }

        /// <summary>
        /// Gets or sets the embedder.
        /// </summary>
        [JsonPropertyName("embedder")]
        public string Embedder { get; set; }

        /// <summary>
        /// Gets or sets the attributes to retrieve.
        /// </summary>
        [JsonPropertyName("attributesToRetrieve")]
        public string[] AttributesToRetrieve { get; set; } = new[] { "*" };

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; set; } = 0;

        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; } = 20;

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        [JsonPropertyName("filter")]
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets whether to show the ranking score.
        /// </summary>
        [JsonPropertyName("showRankingScore")]
        public bool ShowRankingScore { get; set; }

        /// <summary>
        /// Gets or sets whether to show the ranking score details.
        /// </summary>
        [JsonPropertyName("showRankingScoreDetails")]
        public bool ShowRankingScoreDetails { get; set; }

        /// <summary>
        /// Gets or sets the ranking score threshold.
        /// </summary>
        [JsonPropertyName("rankingScoreThreshold")]
        public decimal? RankingScoreThreshold { get; set; }

        /// <summary>
        /// Gets or sets whether to retrieve the vectors.
        /// </summary>
        [JsonPropertyName("retrieveVectors")]
        public bool RetrieveVectors { get; set; }
    }
}
