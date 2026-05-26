using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch.Converters
{
    public class BaseObjectWithTypesConverter<TBase, TType>: JsonConverter<TBase> where TType : struct, Enum
    {
        private readonly string _typePropertyName;
        private readonly Dictionary<TType, Type> _mapper;

        public BaseObjectWithTypesConverter(string typePropertyName, Dictionary<TType, Type> mapper)
        {
            _typePropertyName = typePropertyName;
            _mapper = mapper;
        }

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

        public override void Write(Utf8JsonWriter writer, TBase value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }


    public class DynamicSearchRuleActionConverter : BaseObjectWithTypesConverter<BaseAction, ActionType>
    {
        public DynamicSearchRuleActionConverter() : base(BaseAction.TypePropertyName,
            new Dictionary<ActionType, Type> { [ActionType.Pin] = typeof(PinAction) })
        {
        }
    }

    public class DynamicSearchRuleConditionConverter : BaseObjectWithTypesConverter<BaseCondition, ConditionType>
    {
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
