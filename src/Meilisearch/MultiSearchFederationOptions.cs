using System.Text.Json;
using System.Text.Json.Serialization;

using Meilisearch.Converters;

namespace Meilisearch
{
    /// <summary>
    /// Federation options in federated multi-index search
    /// </summary>
    public class MultiSearchFederationOptions
    {
        /// <summary>
        /// Number of documents to skip
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        /// <summary>
        /// Maximum number of documents returned
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
    }
}
