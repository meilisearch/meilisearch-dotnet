using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Pagination configuration.
    /// </summary>
    public class Pagination
    {
        /// <summary>
        /// Max total hits in each page
        /// </summary>
        [JsonPropertyName("maxTotalHits")]
        public int MaxTotalHits { get; set; }
    }
}
