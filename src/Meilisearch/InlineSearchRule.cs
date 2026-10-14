using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Dynamic search rule to preview inline in a search request, without persisting it.
    /// Inherits the optional rule fields so omitted values are not serialized.
    /// </summary>
    public class InlineSearchRule : PatchDynamicSearchRule
    {
        /// <summary>
        /// Identifier of the rule being previewed.
        /// </summary>
        [JsonPropertyName("uid")]
        public string Uid { get; set; }
    }
}
