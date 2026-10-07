using System.Text.Json.Serialization;

namespace Meilisearch.QueryParameters
{
    /// <summary>
    /// A class that handles the creation of a query body for DynamicSearchRules.
    /// </summary>
    public class DynamicSearchRulesQuery
    {
        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        [JsonPropertyName("offset")]
        public int? Offset { get; set; }

        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        [JsonPropertyName("limit")]
        public int? Limit { get; set; }

        /// <summary>
        /// An optional filter to restrict which rules are returned
        /// </summary>
        [JsonPropertyName("filter")]
        public DSRQFilter Filter { get; set; }

        /// <summary>
        /// A class that handles the creation of a filter parameter for query string for DynamicSearchRules.
        /// </summary>
        public class DSRQFilter
        {
            /// <summary>
            /// Only includes rules whose description or query words match this query.
            /// </summary>
            [JsonPropertyName("query")]
            public string Query { get; set; }

            /// <summary>
            /// An option to include only active or not active rules.
            /// </summary>
            [JsonPropertyName("active")]
            public bool? Active { get; set; }
        }
    }
}
