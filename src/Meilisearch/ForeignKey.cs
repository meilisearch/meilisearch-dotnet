using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Represents a foreign key entry.
    /// </summary>
    public class ForeignKey
    {
        /// <summary>
        /// The uid of the foreign (related) index this entity points to.
        /// </summary>
        [JsonPropertyName("foreignIndexUid")]
        public string ForeignIndexUid { get; set; }

        /// <summary>
        /// The name of the field in the current index that holds the foreign key value.
        /// </summary>
        [JsonPropertyName("fieldName")]
        public string FieldName { get; set; }
    }
}
