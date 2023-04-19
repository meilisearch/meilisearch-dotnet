using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Search query used in multi-index search
    /// </summary>
    public class MultiSearchQuery
    {
        /// <summary>
        /// The queries
        /// </summary>
        [JsonPropertyName("queries")]
        public List<SearchQuery> Queries { get; set; }
    }
}
