using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Wrapper for facet search query
    /// </summary>
    public class FacetSearchQuery
    {
        /// <summary>
        /// Gets or sets the facetName property
        /// </summary>
        [JsonPropertyName("facetName")]
        public string FacetName { get; set; }

        /// <summary>
        /// Gets or sets the facetQuery property
        /// </summary>
        [JsonPropertyName("facetQuery")]
        public string FacetQuery { get; set; }

        /// <summary>
        /// Gets or sets the q property
        /// </summary>
        [JsonPropertyName("q")]
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets the filter property
        /// </summary>
        [JsonPropertyName("filter")]
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets the matchingStrategy property, can be <c>last</c>, <c>all</c> or <c>frequency</c>.
        /// </summary>
        [JsonPropertyName("matchingStrategy")]
        public string MatchingStrategy { get; set; }

        /// <summary>
        /// Gets or sets the attributesToSearchOn property
        /// </summary>
        [JsonPropertyName("attributesToSearchOn")]
        public IEnumerable<string> AttributesToSearchOn { get; set; }
    }
}
