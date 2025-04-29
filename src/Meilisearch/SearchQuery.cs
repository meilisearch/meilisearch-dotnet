using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Search Query for Meilisearch class.
    /// </summary>
    public class SearchQuery
    {
        /// <summary>
        /// The id of the index
        /// </summary>
        [JsonPropertyName("indexUid")]
        public string IndexUid { get; set; }

        /// <summary>
        /// Gets or sets query string.
        /// </summary>
        [JsonPropertyName("q")]
        public string Q { get; set; }

        /// <summary>
        /// Gets or sets the filter to apply to the query.
        /// </summary>
        [JsonPropertyName("filter")]
        public dynamic Filter { get; set; }

        /// <summary>
        /// Gets or sets attributes to retrieve.
        /// </summary>
        [JsonPropertyName("attributesToRetrieve")]
        public IEnumerable<string> AttributesToRetrieve { get; set; }

        /// <summary>
        /// Gets or sets attributes to crop.
        /// </summary>
        [JsonPropertyName("attributesToCrop")]
        public IEnumerable<string> AttributesToCrop { get; set; }

        /// <summary>
        /// Gets or sets attributes to search on.
        /// </summary>
        [JsonPropertyName("attributesToSearchOn")]
        public IEnumerable<string> AttributesToSearchOn { get; set; }

        /// <summary>
        /// Gets or sets length used to crop field values.
        /// </summary>
        [JsonPropertyName("cropLength")]
        public int? CropLength { get; set; }

        /// <summary>
        /// Gets or sets attributes to highlight.
        /// </summary>
        [JsonPropertyName("attributesToHighlight")]
        public IEnumerable<string> AttributesToHighlight { get; set; }

        /// <summary>
        /// Gets or sets the crop marker to apply before and/or after cropped part selected within an attribute defined in `attributesToCrop` parameter.
        /// </summary>
        [JsonPropertyName("cropMarker")]
        public string CropMarker { get; set; }

        /// <summary>
        /// Gets or sets the tag to put before the highlighted query terms.
        /// </summary>
        [JsonPropertyName("highlightPreTag")]
        public string HighlightPreTag { get; set; }

        /// <summary>
        /// Gets or sets the tag to put after the highlighted query terms.
        /// </summary>
        [JsonPropertyName("highlightPostTag")]
        public string HighlightPostTag { get; set; }

        /// <summary>
        /// Gets or sets the facets for the query.
        /// </summary>
        [JsonPropertyName("facets")]
        public IEnumerable<string> Facets { get; set; }

        /// <summary>
        /// Gets or sets showMatchesPosition. It defines whether an object that contains information about the matches should be returned or not.
        /// </summary>
        [JsonPropertyName("showMatchesPosition")]
        public bool? ShowMatchesPosition { get; set; }

        /// <summary>
        /// Gets or sets the sorted attributes.
        /// </summary>
        [JsonPropertyName("sort")]
        public IEnumerable<string> Sort { get; set; }

        /// <summary>
        /// Gets or sets the words matching strategy.
        /// </summary>
        [JsonPropertyName("matchingStrategy")]
        public string MatchingStrategy { get; set; }

        /// <summary>
        /// Gets or sets showRankingScore parameter. It defines wheter the global ranking score of a document (between 0 and 1) is returned or not.
        /// </summary>
        [JsonPropertyName("showRankingScore")]
        public bool? ShowRankingScore { get; set; }

        /// <summary>
        /// Gets or sets showRankingScoreDetails parameter. It defines whether details on how the ranking score was computed are returned or not.
        /// </summary>
        [JsonPropertyName("showRankingScoreDetails")]
        public bool? ShowRankingScoreDetails { get; set; }

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
