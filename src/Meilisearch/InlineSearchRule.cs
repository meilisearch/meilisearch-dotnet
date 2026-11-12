using System.Text.Json.Serialization;

using Meilisearch.Converters;

namespace Meilisearch
{
    /// <summary>
    /// A dynamic search rule to preview inline without saving it on the server.
    /// Only fields supported by the inline-rule API can be provided.
    /// </summary>
    public class InlineSearchRule
    {
        /// <summary>
        /// Identifier of the rule being previewed.
        /// </summary>
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        /// <summary>
        /// Actions to apply if the rule matches the search query.
        /// </summary>
        [JsonPropertyName("actions")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<DSRActions> Actions { get; set; }

        /// <summary>
        /// Priority of the rule when other rules also match.
        /// </summary>
        [JsonPropertyName("precedence")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<ulong?> Precedence { get; set; }

        /// <summary>
        /// Whether the rule is active. Defaults to true on the server when omitted.
        /// </summary>
        [JsonPropertyName("active")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<bool?> Active { get; set; }

        /// <summary>
        /// Conditions that must be satisfied for this rule to apply.
        /// </summary>
        [JsonPropertyName("conditions")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<DynamicSearchRuleConditions> Conditions { get; set; }
    }
}
