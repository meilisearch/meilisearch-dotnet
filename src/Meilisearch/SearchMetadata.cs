using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Additional information returned for search queries when metadata is requested.
    /// </summary>
    public class SearchMetadata
    {
        /// <summary>
        /// Unique identifier of the executed query.
        /// </summary>
        [JsonPropertyName("queryUid")]
        public string QueryUid { get; set; }

        /// <summary>
        /// Index searched by the query.
        /// </summary>
        [JsonPropertyName("indexUid")]
        public string IndexUid { get; set; }

        /// <summary>
        /// Detailed condition outcomes for the inline rule. The shape depends on the conditions.
        /// </summary>
        [JsonPropertyName("inlineRuleConditionOutcomes")]
        public JsonElement? InlineRuleConditionOutcomes { get; set; }
    }
}
