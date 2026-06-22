using System;
using System.Text.Json.Serialization;

using Meilisearch.Converters;

namespace Meilisearch
{
    /// <summary>
    /// Enum that indicates condition type for <see cref="BaseCondition"/> objects
    /// </summary>
    [JsonConverter(typeof(EnumToCamelCaseConverter<ConditionType>))]
    public enum ConditionType
    {
        /// <summary>
        /// Condition as query
        /// </summary>
        Query,
        /// <summary>
        /// Condition as time interval
        /// </summary>
        Time
    }

    /// <summary>
    /// Base class of Conditions for Dynamic Search Rules
    /// </summary>
    [JsonConverter(typeof(DynamicSearchRuleConditionConverter))]
    public abstract class BaseCondition
    {
        /// <summary>
        /// Property name that defines type of BaseCondition object
        /// </summary>
        public const string ScopePropertyName = "scope";

        /// <summary>
        /// Describes condition type
        /// </summary>
        [JsonPropertyName(ScopePropertyName)]
        public abstract ConditionType Scope { get; }
    }

    /// <summary>
    /// Condition for searching the documents by the query using Dynamic Search Rules
    /// </summary>
    public class QueryCondition : BaseCondition
    {
        /// <inheritdoc/>
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

    /// <summary>
    /// Condition for searching the documents by the time interval using Dynamic Search Rules
    /// </summary>
    public class TimeCondition : BaseCondition
    {
        /// <inheritdoc/>
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
