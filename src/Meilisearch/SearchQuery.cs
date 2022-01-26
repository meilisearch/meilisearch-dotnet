namespace Meilisearch
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Search Query for meilisearch class.
    /// </summary>
    public class SearchQuery
    {
        /// <summary>
        /// Gets or sets query string.
        /// </summary>
        [JsonPropertyName("q")]
        public string Q { get; set; }

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
        /// Gets or sets the facets distribution for the query.
        /// </summary>
        [JsonPropertyName("facetsDistribution")]
        public IEnumerable<string> FacetsDistribution { get; set; }

        /// <summary>
        /// Gets or sets matches. It defines whether an object that contains information about the matches should be returned or not.
        /// </summary>
        [JsonPropertyName("matches")]
        public bool? Matches { get; set; }

        /// <summary>
        /// Gets or sets the sorted attributes.
        /// </summary>
        [JsonPropertyName("sort")]
        public IEnumerable<string> Sort { get; set; }
    }
}
