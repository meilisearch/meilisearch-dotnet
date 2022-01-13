namespace Meilisearch
{
    using System;

    /// <summary>
    /// Task information of the actions done.
    /// </summary>
    public class TaskInfo
    {
        /// <summary>
        /// Gets or sets Uid for the operation.
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// Gets or sets state of the operation.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets task error.
        /// </summary>
        public TaskError Error { get; set; }

        /// <summary>
        /// Gets or sets the time the Task was enqueued at.
        /// </summary>
        public DateTime EnqeuedAt { get; set; }

        /// <summary>
        /// Gets or sets the time the Task was started at.
        /// </summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// Gets or sets the time the Task was finished at.
        /// </summary>
        public DateTime? FinishedAt { get; set; }
    }
}
