using System.Collections.Generic;
using System.Text.Json.Serialization;

using Meilisearch.Converters;

namespace Meilisearch
{
    [JsonConverter(typeof(EnumToCamelCaseConverter<ActionType>))]
    public enum ActionType
    {
        Pin
    }

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

    public class DSRASelector
    {
        /// <summary>
        /// Gets or sets indexUid
        /// </summary>
        [JsonPropertyName("indexUid")]
        public string IndexUid { get; set; }

        /// <summary>
        /// Gets ir sets id
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }


    [JsonConverter(typeof(DynamicSearchRuleActionConverter))]
    public abstract class BaseAction
    {
        public const string TypePropertyName = "type";

        /// <summary>
        /// Describes action type
        /// </summary>
        [JsonPropertyName(TypePropertyName)]
        public abstract ActionType Type { get; }
    }

    public class PinAction : BaseAction
    {
        public override ActionType Type => ActionType.Pin;

        /// <summary>
        /// Gets or sets position
        /// </summary>
        [JsonPropertyName("position")]
        public int Position { get; set; }
    }
}
