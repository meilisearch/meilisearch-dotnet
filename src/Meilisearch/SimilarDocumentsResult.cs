
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    public class SimilarDocumentsResult<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hits"></param>
        /// <param name="id"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="estimatedTotalHits"></param>
        /// <param name="processingTimeMs"></param>
        /// <param name="indexUid"></param>
        public SimilarDocumentsResult(
            IReadOnlyCollection<T> hits, string id, int offset,
            int limit, int estimatedTotalHits,int processingTimeMs, string indexUid)
        {
            Id = id;
            Hits = hits;
            Offset = offset;
            Limit = limit;
            EstimatedTotalHits = estimatedTotalHits;
            ProcessingTimeMs = processingTimeMs;
            IndexUid = indexUid;
        }

        [JsonPropertyName("id")]
        public string Id { get; }

        /// <inheritdoc/>
        [JsonPropertyName("hits")]
        public IReadOnlyCollection<T> Hits { get; }

        /// <summary>
        /// Number of documents skipped.
        /// </summary>
        [JsonPropertyName("offset")]
        public int Offset { get; }

        /// <summary>
        /// Number of documents to take.
        /// </summary>
        [JsonPropertyName("limit")]
        public int Limit { get; }

        /// <summary>
        /// Gets the estimated total number of hits returned by the search.
        /// </summary>
        [JsonPropertyName("estimatedTotalHits")]
        public int EstimatedTotalHits { get; }

        /// <inheritdoc/>
        [JsonPropertyName("processingTimeMs")]
        public int ProcessingTimeMs { get; }

        /// <inheritdoc/>
        [JsonPropertyName("indexUid")]
        public string IndexUid { get; }
    }
}
