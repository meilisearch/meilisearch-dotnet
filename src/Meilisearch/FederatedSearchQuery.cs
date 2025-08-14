using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Search query for federated multi-index search
    /// </summary>
    public class FederatedSearchQuery : SearchQueryBase
    {
        /// <summary>
        /// Federated search options
        /// </summary>
        [JsonPropertyName("federationOptions")]
        public MultiSearchFederationOptions FederationOptions { get; set; }
    }
}
