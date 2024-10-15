using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Search Query for federated Multisearch
    /// </summary>
    public class FederatedSearchQuery : SearchQueryBase
    {
        /// <summary>
        /// Federation Options
        /// </summary>
        [JsonPropertyName("federationOptions")]
        public MultiSearchFederationOptions FederationOptions { get; set; }
    }
}
