using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Batch Object
    /// </summary>
    public class BatchResult
    {
        /// <summary>
        /// The unique sequential identifier of the batch.
        /// </summary>
        [JsonPropertyName("uid")]
        public int Uid { get; }

        /// <summary>
        /// Detailed information on the batch progress.
        /// </summary>
        [JsonPropertyName("progress")]
        public IReadOnlyCollection<IReadOnlyDictionary<string, dynamic>> Progress { get; }

        /// <summary>
        /// Detailed information on the batch.
        /// </summary>
        [JsonPropertyName("details")]
        public IReadOnlyDictionary<string, dynamic> Details { get; }

        /// <summary>
        /// Detailed information on the stats.
        /// </summary>
        [JsonPropertyName("stats")]
        public IReadOnlyDictionary<string, dynamic> Stats { get; }


        /// <summary>
        /// The total elapsed time the task spent in the processing state, in ISO 8601 format.
        /// </summary>
        [JsonPropertyName("duration")]

        public string Duration { get; }
        /// <summary>
        /// The date and time when the task began processing, in RFC 3339 format.
        /// </summary>
        [JsonPropertyName("startedAt")]
        public DateTime? StartedAt { get; }

        /// <summary>
        /// The date and time when the task finished processing, whether failed or succeeded, in RFC 3339 format.
        /// </summary>
        [JsonPropertyName("finishedAt")]
        public DateTime? FinishedAt { get; }

        /// <summary>
        /// A string describing the logic behind the creation of the batch. Can contain useful information when diagnosing indexing performance issues.
        /// </summary>
        [JsonPropertyName("batchStrategy")]
        public string BatchStrategy { get; }
    }
}
