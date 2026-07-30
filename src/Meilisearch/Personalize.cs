using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Personalization settings of a search query.
    /// </summary>
    public class Personalize
    {
        /// <summary>
        /// Gets or sets the description of the user performing the search, used to rerank the results.
        /// </summary>
        [JsonPropertyName("userContext")]
        public string UserContext { get; set; }
    }
}
