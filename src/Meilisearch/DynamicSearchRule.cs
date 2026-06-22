using System.Collections.Generic;
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
        /// Actions to apply when dynamic search rule matches
        /// </summary>
        [JsonPropertyName("actions")]
        public IEnumerable<DSRAction> Actions { get; set; }

        /// <summary>
        /// Optional field. Gets or sets the description
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Precedence of the dynamic search rule.
        /// Lower numeric values take precedence over higher ones.
        /// <list type="bullets">
        ///     <item> If the same document is selected by multiple rules, the smallest <i>priority</i> number wins  </item>
        ///     <item> If different documents are pinned to the same position, they are ordered by ascending <i>priority</i> </item>
        /// </list>
        /// </summary>
        [JsonPropertyName("priority")]
        public ulong? Priority { get; set; }

        /// <summary>
        /// Whether the dynamic search rule is active
        /// </summary>
        [JsonPropertyName("active")]
        public bool Active { get; set; }

        /// <summary>
        /// Conditions that must match before the dynamic search rule applies
        /// </summary>
        [JsonPropertyName("conditions")]
        public IEnumerable<BaseCondition> Conditions { get; set; }
    }
}
