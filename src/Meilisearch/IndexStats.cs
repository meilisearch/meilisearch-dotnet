using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Wrapper for index stats.
    /// </summary>
    public class IndexStats
    {
        public IndexStats(int numberOfDocuments, bool isIndexing, IReadOnlyDictionary<string, int> fieldDistribution)
        {
            NumberOfDocuments = numberOfDocuments;
            IsIndexing = isIndexing;
            FieldDistribution = fieldDistribution;
        }

        /// <summary>
        /// Gets or sets the total number of documents.
        /// </summary>
        [JsonPropertyName("numberOfDocuments")]
        public int NumberOfDocuments { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the index is currently indexing.
        /// </summary>
        [JsonPropertyName("isIndexing")]
        public bool IsIndexing { get; }

        /// <summary>
        /// Gets or sets field distribution.
        /// </summary>
        [JsonPropertyName("fieldDistribution")]
        public IReadOnlyDictionary<string, int> FieldDistribution { get; }
    }
}
