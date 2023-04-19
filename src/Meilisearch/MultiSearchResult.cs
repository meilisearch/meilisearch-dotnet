using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Search result used in multi-index search
    /// </summary>
    public class MultiSearchResult
    {
        /// <summary>
        /// The search results
        /// </summary>
        [JsonPropertyName("results")]
        public List<ISearchable<JsonDocument>> Results { get; set; }
    }
}
