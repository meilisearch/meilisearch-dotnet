using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch.QueryParameters
{
    /// <summary>
    /// A class that handles the creation of a query string for Batches.
    /// </summary>
    public class BatchesQuery
    {
        /// <summary>
        /// Gets or sets the list of UIds to filter on.
        /// </summary>
        [JsonPropertyName("uids")]
        public List<int> UIds { get; set; }

        /// <summary>
        /// Gets or sets the list of Batch UIds to filter on.
        /// </summary>
        [JsonPropertyName("batchUids")]
        public List<int> BatchUIds { get; set; }

        /// <summary>
        /// Gets or sets the list of Index UIds to filter on.
        /// </summary>
        [JsonPropertyName("indexUids")]
        public List<int> IndexUIds { get; set; }

        /// <summary>
        /// Gets or sets the list of statuses to filter on.
        /// </summary>
        [JsonPropertyName("statuses")]
        public List<TaskInfoStatus> Statuses { get; set; }

        /// <summary>
        /// Gets or sets the list of types to filter on.
        /// </summary>
        [JsonPropertyName("types")]
        public List<TaskInfoType> Types { get; set; }

        /// <summary>
        /// Gets or sets the Number of tasks to return.
        /// </summary>
        [JsonPropertyName("limit")]
        public int? Limit { get; set; } = 20;

        /// <summary>
        /// Gets or sets the uid of the first task returned.
        /// </summary>
        [JsonPropertyName("from")]
        public int? From { get; set; }

        /// <summary>
        /// Gets or set the order of the returned tasks.
        /// </summary>
        [JsonPropertyName("reverse")]
        public bool Reverse { get; set; } = false;

        /// <summary>
        /// Gets or sets the date before the task is enqueued to filter.
        /// </summary>
        [JsonPropertyName("beforeEnqueuedAt")]
        public DateTime? BeforeEnqueuedAt { get; set; }

        /// <summary>
        /// Gets or sets the date before the task is started to filter.
        /// </summary>
        [JsonPropertyName("beforeStartedAt")]
        public DateTime? BeforeStartedAt { get; set; }

        /// <summary>
        /// Gets or sets the date before the task is finished to filter.
        /// </summary>
        [JsonPropertyName("beforeFinishedAt")]
        public DateTime? BeforeFinishedAt { get; set; }

        /// <summary>
        /// Gets or sets the date after the task is enqueued to filter.
        /// </summary>
        [JsonPropertyName("afterEnqueuedAt")]
        public DateTime? AfterEnqueuedAt { get; set; }

        /// <summary>
        /// Gets or sets the date after the task is started to filter.
        /// </summary>
        [JsonPropertyName("afterStartedAt")]
        public DateTime? AfterStartedAt { get; set; }

        /// <summary>
        /// Gets or sets the date after the task is finished to filter.
        /// </summary>
        [JsonPropertyName("afterFinishedAt")]
        public DateTime? AfterFinishedAt { get; set; }
    }
}
