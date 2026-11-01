using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Wrapper for index stats.
    /// </summary>
    public class IndexStats
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexStats"/> class.
        /// </summary>
        /// <paraaram name="usedIndexSize">The used size of the index on disk (in bytes).</param>
        public IndexStats(int numberOfDocuments, bool isIndexing, IReadOnlyDictionary<string, int> fieldDistribution, long rawDocumentDbSize, long avgDocumentSize, int numberOfEmbeddedDocuments, int numberOfEmbeddings, long indexSize, long usedIndexSize)
        {
            NumberOfDocuments = numberOfDocuments;
            IsIndexing = isIndexing;
            FieldDistribution = fieldDistribution;
            RawDocumentDbSize = rawDocumentDbSize;
            AvgDocumentSize = avgDocumentSize;
            NumberOfEmbeddedDocuments = numberOfEmbeddedDocuments;
            NumberOfEmbeddings = numberOfEmbeddings;
            IndexSize = indexSize;
            UsedIndexSize = usedIndexSize;
        }

        /// <summary>
        /// Gets the total number of documents.
        /// </summary>
        [JsonPropertyName("numberOfDocuments")]
        public int NumberOfDocuments { get; }

        /// <summary>
        /// Gets a value indicating whether the index is currently indexing.
        /// </summary>
        [JsonPropertyName("isIndexing")]
        public bool IsIndexing { get; }

        /// <summary>
        /// Gets field distribution.
        /// </summary>
        [JsonPropertyName("fieldDistribution")]
        public IReadOnlyDictionary<string, int> FieldDistribution { get; }

        /// <summary>
        /// Get the total size of the documents stored in Meilisearch
        /// </summary>
        [JsonPropertyName("rawDocumentDbSize")]
        public long RawDocumentDbSize { get; }

        /// <summary>
        /// Get the total size of the documents stored in Meilisearch divided by the number of documents
        /// </summary>
        [JsonPropertyName("avgDocumentSize")]
        public long AvgDocumentSize { get; }

        /// <summary>
        /// Get the number of document in index that contains at least one embedded representation
        /// </summary>
        [JsonPropertyName("numberOfEmbeddedDocuments")]
        public int NumberOfEmbeddedDocuments { get; }


        /// <summary>
        /// Get the total number of embeddings representation that exists in that indexes
        /// </summary>
        [JsonPropertyName("numberOfEmbeddings")]
        public int NumberOfEmbeddings { get; }

        /// <summary>
        /// Gets the total size of the index on disk (in bytes).
        /// </summary>
        [JsonPropertyName("indexSize")]
        public long IndexSize { get; }

        /// <summary>
        /// Gets the used size of the index on disk (in bytes).
        /// </summary>
        [JsonPropertyName("usedIndexSize")]
        public long UsedIndexSize { get; }
    }
}
