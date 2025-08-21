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
        /// Return results based on query keywords and meaning
        /// </summary>
        [JsonPropertyName("hybrid")]
        public Hybrid Hybrid { get; set; }

        /// <summary>
        /// Search using a custom query vector
        /// </summary>
        [JsonPropertyName("vector")]
        public IEnumerable<float> Vector { get; set; }

        /// <summary>
        /// Return document vector data
        /// </summary>
        [JsonPropertyName("retrieveVectors")]
        public bool? RetrieveVectors { get; set; }

        /// <summary>
        /// Explicitly specify languages used in a query
        /// </summary>
        [JsonPropertyName("locales")]
        public IEnumerable<string> Locales { get; set; }
    }

    /// <summary>
    /// Configures Meilisearch to return search results based on a query's meaning and context.
    /// </summary>
    public class Hybrid 
    {
        /// <summary>
        /// embedder must be a string indicating an embedder configured with the /settings endpoint. 
        /// </summary>
        [JsonPropertyName("embedder")]
        public string Embedder { get; set; }

        /// <summary>
        /// semanticRatio must be a number between 0.0 and 1.0 indicating 
        /// the proportion between keyword and semantic search results. 
        /// 0.0 causes Meilisearch to only return keyword results. 
        /// 1.0 causes Meilisearch to only return meaning-based results. Defaults to 0.5.
        /// </summary>
        [JsonPropertyName("semanticRatio")]
        public float? SemanticRatio { get; set; }
    }
}
