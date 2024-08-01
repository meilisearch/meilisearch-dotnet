using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using Meilisearch.Converters;

namespace Meilisearch
{
    /// <summary>
    /// Information of the regarding a task.
    /// </summary>
    public class TaskInfo
    {
        public TaskInfo(int taskUid, string indexUid, TaskInfoStatus status, TaskInfoType type,
            IReadOnlyDictionary<string, object> details, IReadOnlyDictionary<string, string> error, string duration, DateTime enqueuedAt,
            DateTime? startedAt, DateTime? finishedAt)
        {
            TaskUid = taskUid;
            IndexUid = indexUid;
            Status = status;
            Type = type;
            Details = details;
            Error = error;
            Duration = duration;
            EnqueuedAt = enqueuedAt;
            StartedAt = startedAt;
            FinishedAt = finishedAt;
        }

        /// <summary>
        /// The unique sequential identifier of the task.
        /// </summary>
        [JsonPropertyName("taskUid")]
        public int TaskUid { get; }

        /// <summary>
        /// The unique index identifier.
        /// </summary>
        [JsonPropertyName("indexUid")]
        public string IndexUid { get; }

        /// <summary>
        /// The status of the task. Possible values are enqueued, processing, succeeded, failed.
        /// </summary>
        [JsonPropertyName("status")]
        public TaskInfoStatus Status { get; }

        /// <summary>
        /// The type of task.
        /// </summary>
        [JsonPropertyName("type")]
        public TaskInfoType Type { get; }

        /// <summary>
        /// Detailed information on the task payload.
        /// </summary>
        [JsonPropertyName("details")]
        public IReadOnlyDictionary<string, object> Details { get; }

        /// <summary>
        /// Error details and context. Only present when a task has the failed status.
        /// </summary>
        [JsonPropertyName("error")]
        public IReadOnlyDictionary<string, string> Error { get; }

        /// <summary>
        /// The total elapsed time the task spent in the processing state, in ISO 8601 format.
        /// </summary>
        [JsonPropertyName("duration")]
        public string Duration { get; }

        /// <summary>
        /// The date and time when the task was first enqueued, in RFC 3339 format.
        /// </summary>
        [JsonPropertyName("enqueuedAt")]
        public DateTime EnqueuedAt { get; }

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
        /// A taskUid who canceled the current task.
        /// </summary>
        [JsonPropertyName("canceledBy")]
        public int? CanceledBy { get; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TaskInfoStatus
    {
        Enqueued,
        Processing,
        Succeeded,
        Failed,
        Canceled
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TaskInfoType
    {
        IndexCreation,
        IndexUpdate,
        IndexDeletion,
        DocumentAdditionOrUpdate,
        DocumentDeletion,
        SettingsUpdate,
        DumpCreation,
        TaskCancelation,
        SnapshotCreation,
        TaskDeletion,
        IndexSwap,
        Unknown
    }
}
