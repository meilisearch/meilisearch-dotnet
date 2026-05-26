using System;
using System.Text.Json.Serialization;

using Meilisearch.Converters;

namespace Meilisearch
{
    [JsonConverter(typeof(EnumToCamelCaseConverter<ConditionType>))]
    public enum ConditionType
    {
        Query,
        Time
    }

    [JsonConverter(typeof(DynamicSearchRuleConditionConverter))]
    public abstract class BaseCondition
    {
        public const string ScopePropertyName = "scope";

        /// <summary>
        /// Describes condition type
        /// </summary>
        [JsonPropertyName(ScopePropertyName)]
        public abstract ConditionType Scope { get; }
    }

    public class QueryCondition : BaseCondition
    {
        public override ConditionType Scope => ConditionType.Query;

        /// <summary>
        /// Gets or sets isEmpty
        /// </summary>
        [JsonPropertyName("isEmpty")]
        public bool? IsEmpty { get; set; }

        /// <summary>
        /// Gets or sets contains
        /// </summary>
        [JsonPropertyName("contains")]
        public string Contains { get; set; }
    }

    public class TimeCondition : BaseCondition
    {
        public override ConditionType Scope => ConditionType.Time;

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
}
