using System.Collections.Generic;
using System.Text.Json.Serialization;

using Meilisearch.Converters;

namespace Meilisearch
{
    /// <summary>
    /// Helper to call Meilisearch API for updating or creating dynamic search rule
    /// </summary>
    public class PatchDynamicSearchRule
    {
        /// <summary>
        /// Actions to apply when dynamic search rule matches
        /// </summary>
        [JsonPropertyName("actions")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<IEnumerable<DSRAction>> Actions { get; set; }

        /// <summary>
        /// Optional field. Gets or sets the description
        /// </summary>
        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<string> Description { get; set; }

        /// <summary>
        /// Precedence of the dynamic search rule.
        /// Lower numeric values take precedence over higher ones.
        /// <list type="bullets">
        ///     <item> If the same document is selected by multiple rules, the smallest <i>priority</i> number wins  </item>
        ///     <item> If different documents are pinned to the same position, they are ordered by ascending <i>priority</i> </item>
        /// </list>
        /// </summary>
        [JsonPropertyName("priority")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<ulong?> Priority { get; set; }

        /// <summary>
        /// Whether the dynamic search rule is active
        /// </summary>
        [JsonPropertyName("active")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<bool?> Active { get; set; }

        /// <summary>
        /// Conditions that must match before the dynamic search rule applies
        /// </summary>
        [JsonPropertyName("conditions")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<IEnumerable<BaseCondition>> Conditions { get; set; }
    }
}
