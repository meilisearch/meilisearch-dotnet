using System;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Meilisearch API dynamic search rule wrapper
    /// </summary>
    public class DynamicSearchRule
    {
        /// <summary>
        /// Gets or sets the uid.
        /// </summary>
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        /// <summary>
        /// Date and time of the last update of this rule.
        /// </summary>
        [JsonPropertyName("lastUpdatedAt")]
        public DateTimeOffset? LastUpdatedAt { get; set; }

        /// <summary>
        /// Actions applied when the dynamic search rule matches.
        /// Pins documents to a fixed position and scales selected documents' relevancy.
        /// </summary>
        [JsonPropertyName("actions")]
        public DSRActions Actions { get; set; }

        /// <summary>
        /// Optional field. Gets or sets the description
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Precedence of the dynamic search rule.
        /// Lower numeric values take precedence over higher ones.
        /// <list type="bullets">
        ///     <item> If the same document is selected by multiple rules, the smallest <i>precedence</i> number wins  </item>
        ///     <item> If different documents are pinned to the same position, they are ordered by ascending <i>precedence</i> </item>
        /// </list>
        /// </summary>
        [JsonPropertyName("precedence")]
        public ulong? Precedence { get; set; }

        /// <summary>
        /// Whether the dynamic search rule is active
        /// </summary>
        [JsonPropertyName("active")]
        public bool Active { get; set; }

        /// <summary>
        /// Conditions that must match before the dynamic search rule applies
        /// </summary>
        [JsonPropertyName("conditions")]
        public DynamicSearchRuleConditions Conditions { get; set; }
    }
}
