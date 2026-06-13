using System.Text.Json.Serialization;

using Meilisearch.Converters;

namespace Meilisearch
{
    /// <summary>
    /// Enum that indicates action type for BaseAction objects
    /// </summary>
    [JsonConverter(typeof(EnumToCamelCaseConverter<ActionType>))]
    public enum ActionType
    {
        /// <summary>
        /// Pin action
        /// </summary>
        Pin
    }

    /// <summary>
    /// Wrapper for Selector - Action pairs
    /// </summary>
    public class DSRAction
    {
        /// <summary>
        /// Target document selector for this action
        /// </summary>
        [JsonPropertyName("selector")]
        public DSRASelector Selector { get; set; }

        /// <summary>
        /// Action payload to apply to the selected document
        /// </summary>
        [JsonPropertyName("action")]
        public BaseAction Action { get; set; }
    }

    /// <summary>
    /// Target document selector descriptor
    /// </summary>
    public class DSRASelector
    {
        /// <summary>
        /// Gets or sets indexUid
        /// </summary>
        [JsonPropertyName("indexUid")]
        public string IndexUid { get; set; }

        /// <summary>
        /// Gets or sets id
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }


    /// <summary>
    /// Base class of Actions for Dynamic Search Rules
    /// </summary>
    [JsonConverter(typeof(DynamicSearchRuleActionConverter))]
    public abstract class BaseAction
    {
        /// <summary>
        /// Property name that defines type of BaseAction object
        /// </summary>
        public const string TypePropertyName = "type";

        /// <summary>
        /// Describes action type
        /// </summary>
        [JsonPropertyName(TypePropertyName)]
        public abstract ActionType Type { get; }
    }

    /// <summary>
    /// Pin action for found documents using Dynamic Search Rules
    /// </summary>
    public class PinAction : BaseAction
    {
        /// <inheritdoc/>
        /// <value>ActionType.Pin</value>
        public override ActionType Type => ActionType.Pin;

        /// <summary>
        /// Gets or sets position
        /// </summary>
        [JsonPropertyName("position")]
        public int Position { get; set; }
    }
}
