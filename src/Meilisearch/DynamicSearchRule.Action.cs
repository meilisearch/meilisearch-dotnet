using System;
using System.Text.Json.Serialization;

using Meilisearch.Converters;

namespace Meilisearch
{
    /// <summary>
    /// Enum that indicates action type for <see cref="BaseAction"/> objects
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
        /// Creates a document selector.
        /// </summary>
        /// <param name="indexUid">Index containing the selected document.</param>
        /// <param name="id">Identifier of the selected document.</param>
        public DSRASelector(string indexUid, string id)
        {
            if (string.IsNullOrEmpty(indexUid))
            {
                throw new ArgumentNullException(nameof(indexUid));
            }

            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            IndexUid = indexUid;
            Id = id;
        }

        /// <summary>
        /// Gets indexUid
        /// </summary>
        [JsonPropertyName("indexUid")]
        public string IndexUid { get; }

        /// <summary>
        /// Gets id
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; }
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
    /// Action that pins matching documents to a specific position
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
