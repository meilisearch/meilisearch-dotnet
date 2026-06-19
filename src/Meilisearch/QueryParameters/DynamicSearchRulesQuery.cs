using System.Collections.Generic;
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
            /// Only includes rules whose attribute names match these patterns.
            /// </summary>
            [JsonPropertyName("attribute_patterns")]
            public DSRQFilterPatterns AttributePatterns { get; set; }

            /// <summary>
            /// An option to include only active or not active rules.
            /// </summary>
            [JsonPropertyName("active")]
            public bool? Active { get; set; }

            /// <summary>
            /// A class that handles creation of list of patterns wrapped into object for filter class
            /// </summary>
            public class DSRQFilterPatterns
            {
                /// <summary>
                /// Patterns list
                /// </summary>
                [JsonPropertyName("patterns")]
                public IEnumerable<string> Patterns { get; set; }
            }
        }
    }
}
