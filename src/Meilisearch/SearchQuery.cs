using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Search Query for Meilisearch class.
    /// </summary>
    public class SearchQuery : SearchQueryBase
    {
        // pagination:

        /// <summary>
        /// Gets or sets offset for the Query.
        /// </summary>
        [JsonPropertyName("offset")]
        public int? Offset { get; set; }

        /// <summary>
        /// Gets or sets limits the number of results.
        /// </summary>
        [JsonPropertyName("limit")]
        public int? Limit { get; set; }


        /// <summary>
        /// Gets or sets hitsPerPage.
        /// </summary>
        [JsonPropertyName("hitsPerPage")]
        public int? HitsPerPage { get; set; }

        /// <summary>
        /// Gets or sets page.
        /// </summary>
        [JsonPropertyName("page")]
        public int? Page { get; set; }

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
        /// Gets or sets locales.
        /// </summary>
        [JsonPropertyName("locales")]
        public IEnumerable<string> Locales { get; set; }
    }
}
