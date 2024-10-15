using System.Collections.Generic;
using System.Text.Json.Serialization;

using Meilisearch.Converters;

namespace Meilisearch
{
    /// <summary>
    ///  Search query used in federated multi-index search
    /// </summary>
    public class FederatedMultiSearchQuery
    {

        /// <summary>
        /// Default Constructor that ensures FederationOptions are always set
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
        /// If present and not null, returns a single list merging all search results across all specified queries
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("federation")]
        [JsonConverter(typeof(MultiSearchFederationOptionsConverter))]
        public MultiSearchFederationOptions FederationOptions { get; set; }
    }
}
