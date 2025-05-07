using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Base properties of search query
    /// </summary>
    public class SearchQueryBase
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
        /// Gets or sets showRankingScore parameter. It defines whether the global ranking score of a document (between 0 and 1) is returned or not.
        /// </summary>
        [JsonPropertyName("showRankingScore")]
        public bool? ShowRankingScore { get; set; }

        /// <summary>
        /// Gets or sets showRankingScoreDetails parameter. It defines whether details on how the ranking score was computed are returned or not.
        /// </summary>
        [JsonPropertyName("showRankingScoreDetails")]
        public bool? ShowRankingScoreDetails { get; set; }
    }
}
