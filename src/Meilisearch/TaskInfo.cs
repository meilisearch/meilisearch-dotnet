using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Information of the regarding a task.
    /// </summary>
    public class TaskInfo
    {
        /// <summary>
        /// Gets or sets Uid for the task.
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// Gets or sets the index UID related to the task.
        /// </summary>
        public string IndexUid { get; set; }

        /// <summary>
        /// Gets or sets state of the task.
        /// </summary>
        public TaskInfoStatus Status { get; set; }

        /// <summary>
        /// Gets or sets type of the task.
        /// </summary>
        public TaskInfoType Type { get; set; }

        /// <summary>
        /// Detailed information on the task payload.
        /// </summary>
        public Dictionary<string, object> Details { get; set; }

        /// <summary>
        /// Gets or sets the error raised.
        /// </summary>
        public Dictionary<string, string> Error { get; set; }

        /// <summary>
        /// The total elapsed time the task spent in the processing state, in ISO 8601 format.
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// The date and time when the task was first enqueued, in RFC 3339 format.
        /// </summary>
        public DateTime EnqueuedAt { get; set; }

        /// <summary>
        /// The date and time when the task began processing, in RFC 3339 format.
        /// </summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// The date and time when the task finished processing, whether failed or succeeded, in RFC 3339 format.
        /// </summary>
        public DateTime? FinishedAt { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TaskInfoStatus
    {
        Enqueued,
        Processing,
        Succeeded,
        Failed
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TaskInfoType
    {
        IndexCreation,
        IndexUpdate,
        IndexDeletion,
        DocumentAddition,
        DocumentPartial,
        DocumentDeletion,
        SettingsUpdate,
        ClearAll
    }
}
