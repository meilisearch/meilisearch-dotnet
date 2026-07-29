using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Conditions that must match before a Dynamic Search Rule applies.
    /// </summary>
    public class DynamicSearchRuleConditions
    {
        /// <summary>
        /// Query condition.
        /// </summary>
        [JsonPropertyName("query")]
        public QueryCondition Query { get; set; }

        /// <summary>
        /// Time condition.
        /// </summary>
        [JsonPropertyName("time")]
        public TimeCondition Time { get; set; }

        /// <summary>
        /// Filter condition.
        /// </summary>
        [JsonPropertyName("filter")]
        public FilterCondition Filter { get; set; }
    }

    /// <summary>
    /// Condition for searching the documents by the query using Dynamic Search Rules
    /// </summary>
    public class QueryCondition
    {
        /// <summary>
        /// Gets or sets isEmpty
        /// </summary>
        [JsonPropertyName("isEmpty")]
        public bool? IsEmpty { get; set; }

        /// <summary>
        /// Gets or sets the words that must occur in the search query.
        /// </summary>
        [JsonPropertyName("words")]
        public string Words { get; set; }
    }

    /// <summary>
    /// Condition for searching the documents by the time interval using Dynamic Search Rules
    /// </summary>
    public class TimeCondition
    {
        /// <summary>
        /// Gets or sets start
        /// </summary>
        [JsonPropertyName("start")]
        public DateTimeOffset? Start { get; set; }

        /// <summary>
        /// Gets or sets end
        /// </summary>
        [JsonPropertyName("end")]
        public DateTimeOffset? End { get; set; }
    }

    /// <summary>
    /// Condition for matching filter values using Dynamic Search Rules.
    /// </summary>
    public class FilterCondition
    {
        /// <summary>
        /// Gets or sets expected values keyed by facet name.
        /// Values may contain any JSON-compatible value.
        /// </summary>
        [JsonPropertyName("values")]
        public Dictionary<string, object> Values { get; set; }
    }
}
