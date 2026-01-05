using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Search result for similar documents.
    /// </summary>
    public class SimilarDocumentsResult<T>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SimilarDocumentsResult{T}"/> class.
        /// </summary>
        /// <param name="hits"></param>
        /// <param name="id"></param>
        /// <param name="processingTimeMs"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="estimatedTotalHits"></param>
        public SimilarDocumentsResult(
            IReadOnlyCollection<T> hits,
            string id,
            int processingTimeMs,
            int offset,
            int limit,
            int estimatedTotalHits)
        {
            Hits = hits;
            Id = id;
            ProcessingTimeMs = processingTimeMs;
            Offset = offset;
            Limit = limit;
            EstimatedTotalHits = estimatedTotalHits;
        }

        /// <summary>
        /// Gets the hits.
        /// </summary>
        [JsonPropertyName("hits")]
        public IReadOnlyCollection<T> Hits { get; }

        /// <summary>
        /// Gets the id.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; }

        /// <summary>
        /// Gets the processing time in milliseconds.
        /// </summary>
        [JsonPropertyName("processingTimeMs")]
        public int ProcessingTimeMs { get; }

        /// <summary>
        /// Gets the offset.
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; }

        /// <summary>
        /// Gets the limit.
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; }

        /// <summary>
        /// Gets the estimated total hits.
        /// </summary>
        [JsonPropertyName("estimatedTotalHits")]
        public int EstimatedTotalHits { get; }
    }
}
