using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Information of the regarding a task.
    /// </summary>
    public class TaskResource
    {
        public TaskResource(int uid, string? indexUid, TaskInfoStatus status, TaskInfoType type,
            Dictionary<string, object> details, Dictionary<string, string> error, string duration, DateTime enqueuedAt,
            DateTime? startedAt, DateTime? finishedAt)
        {
            Uid = uid;
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
        public int Uid { get; }

        /// <summary>
        /// The unique index identifier.
        /// </summary>
        public string? IndexUid { get; }

        /// <summary>
        /// The status of the task. Possible values are enqueued, processing, succeeded, failed.
        /// </summary>
        public TaskInfoStatus Status { get; }

        /// <summary>
        /// The type of task.
        /// </summary>
        public TaskInfoType Type { get; }

        /// <summary>
        /// Detailed information on the task payload.
        /// </summary>
        public Dictionary<string, dynamic> Details { get; }

        /// <summary>
        /// Error details and context. Only present when a task has the failed status.
        /// </summary>
        public Dictionary<string, string> Error { get; }

        /// <summary>
        /// The total elapsed time the task spent in the processing state, in ISO 8601 format.
        /// </summary>
        public string Duration { get; }

        /// <summary>
        /// The date and time when the task was first enqueued, in RFC 3339 format.
        /// </summary>
        public DateTime EnqueuedAt { get; }

        /// <summary>
        /// The date and time when the task began processing, in RFC 3339 format.
        /// </summary>
        public DateTime? StartedAt { get; }

        /// <summary>
        /// The date and time when the task finished processing, whether failed or succeeded, in RFC 3339 format.
        /// </summary>
        public DateTime? FinishedAt { get; }
    }
}
