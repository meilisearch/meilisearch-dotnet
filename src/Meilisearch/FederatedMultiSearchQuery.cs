using System.Collections.Generic;
using System.Text.Json.Serialization;

using Meilisearch.Converters;

namespace Meilisearch
{
    /// <summary>
    ///  Search query used in federated multi-index search
    /// </summary>
    [JsonConverter(typeof(MultiSearchFederationOptionsConverter))]
    public class FederatedMultiSearchQuery
    {
        /// <summary>
        /// Default constructor that ensures FederationOptions are always set
        /// </summary>
        public FederatedMultiSearchQuery()
        {
            FederationOptions = new MultiSearchFederationOptions();
        }

        /// <summary>
        /// The queries
        /// </summary>
        [JsonPropertyName("queries")]
        public List<FederatedSearchQuery> Queries { get; set; }

        /// <summary>
        /// The federated search query options
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("federation")]
        [JsonConverter(typeof(MultiSearchFederationOptionsConverter))]
        public MultiSearchFederationOptions FederationOptions { get; set; }
    }
}
