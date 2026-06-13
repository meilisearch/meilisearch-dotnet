using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch.Converters
{
    /// <summary>
    /// Base JSON converter for polymorphic objects identified by a discriminator property
    /// </summary>
    /// <typeparam name="TBase">Base type that contains the discriminator property</typeparam>
    /// <typeparam name="TType">Enum used to determine the concrete derived type</typeparam>
    public abstract class BaseObjectWithTypesConverter<TBase, TType>: JsonConverter<TBase> where TType : struct, Enum
    {
        private readonly string _typePropertyName;
        private readonly Dictionary<TType, Type> _mapper;

        /// <summary>
        /// Default constructor to define converter behavior
        /// </summary>
        /// <param name="typePropertyName">Name of the discriminator property</param>
        /// <param name="mapper">Mapping between discriminator values and their corresponding derived types</param>
        protected BaseObjectWithTypesConverter(string typePropertyName, Dictionary<TType, Type> mapper)
        {
            _typePropertyName = typePropertyName;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public override TBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (var doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;
                if (!root.TryGetProperty(_typePropertyName, out var typeElement))
                    throw new JsonException($"Expected {_typePropertyName} property");

                var typeStr = typeElement.GetString();
                if (!Enum.TryParse(typeStr, true, out TType type))
                    throw new JsonException($"Unexpected value for {_typePropertyName} property found: {typeStr}");

                if (!_mapper.TryGetValue(type, out var resultType))
                    throw new JsonException($"Unimplemented {_typePropertyName} type: {type}");

                return (TBase) JsonSerializer.Deserialize(root.GetRawText(), resultType, options);
            }
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, TBase value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }

    /// <summary>
    /// JSON converter for <see cref="BaseAction"/> implementations.
    /// </summary>
    public class DynamicSearchRuleActionConverter : BaseObjectWithTypesConverter<BaseAction, ActionType>
    {
        /// <summary>
        /// <inheritdoc/>
        /// Supports the following mappings:
        /// <list type="bullet">
        /// <item>
        /// <description><see cref="ActionType.Pin"/> -&gt; <see cref="PinAction"/></description>
        /// </item>
        /// </list>
        /// </summary>
        public DynamicSearchRuleActionConverter() : base(BaseAction.TypePropertyName,
            new Dictionary<ActionType, Type> { [ActionType.Pin] = typeof(PinAction) })
        {
        }
    }

    /// <summary>
    /// Defines converter for BaseCondition implementations
    /// </summary>
    public class DynamicSearchRuleConditionConverter : BaseObjectWithTypesConverter<BaseCondition, ConditionType>
    {
        /// <summary>
        /// <inheritdoc/>
        /// Supports the following mappings:
        /// <list type="bullet">
        /// <item>
        /// <description><see cref="ConditionType.Query"/> -&gt; <see cref="QueryCondition"/></description>
        /// </item>
        /// <item>
        /// <description><see cref="ConditionType.Time"/> -&gt; <see cref="TimeCondition"/></description>
        /// </item>
        /// </list>
        /// </summary>
        public DynamicSearchRuleConditionConverter() : base(BaseCondition.ScopePropertyName,
            new Dictionary<ConditionType, Type>
            {
                [ConditionType.Query] = typeof(QueryCondition),
                [ConditionType.Time] = typeof(TimeCondition)
            })
        {
        }
    }
}
