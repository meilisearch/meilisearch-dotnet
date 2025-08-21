using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch.QueryParameters
{
    /// <summary>
    /// A class that handles the creation of a query string for Documents.
    /// </summary>
    public class DocumentsQuery
    {
        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        [JsonPropertyName("limit")]
        public int? Limit { get; set; }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        [JsonPropertyName("offset")]
        public int? Offset { get; set; }

        /// <summary>
        /// Gets or sets the attributes to retrieve.
        /// </summary>
        [JsonPropertyName("fields")]
        public List<string> Fields { get; set; }

        /// <summary>
        /// An optional filter to apply
        /// </summary>
        [JsonPropertyName("filter")]
        public object Filter { get; set; }

        /// <summary>
        /// Return document vector data with search result
        /// </summary>
        [JsonPropertyName("retrieveVectors")]
        public bool? RetrieveVectors { get; set; }
    }
}
