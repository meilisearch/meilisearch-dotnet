using System;
using System.Collections.Generic;

namespace Meilisearch.QueryParameters
{
    /// <summary>
    /// A class that handles the creation of a query string for Tasks.
    /// </summary>
    public class TasksQuery
    {
        /// <summary>
        /// Gets or sets the Number of tasks to return.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Gets or sets the uid of the first task returned.
        /// </summary>
        public int? From { get; set; }

        /// <summary>
        /// Gets or sets the lists of indexUid to filter on. Case-sensitive.
        /// </summary>
        public List<string> IndexUids { get; set; }

        /// <summary>
        /// Gets or sets the lists of uid to filter on. Case-sensitive.
        /// </summary>
        public List<int> Uids { get; set; }

        /// <summary>
        /// Gets or sets the list of statuses to filter on.
        /// </summary>
        public List<TaskInfoStatus> Statuses { get; set; }

        /// <summary>
        /// Gets or sets the list of types to filter on.
        /// </summary>
        public List<TaskInfoType> Types { get; set; }

        /// <summary>
        /// Gets or sets the list of canceledBy uids to filter on. Case-sensitive.
        /// </summary>
        public List<string> CanceledBy { get; set; }

        /// <summary>
        /// Gets or sets the date before the task is enqueued to filter.
        /// </summary>
        public DateTime? BeforeEnqueuedAt { get; set; }

        /// <summary>
        /// Gets or sets the date after the task is enqueued to filter.
        /// </summary>
        public DateTime? AfterEnqueuedAt { get; set; }

        /// <summary>
        /// Gets or sets the date before the task was started to filter.
        /// </summary>
        public DateTime? BeforeStartedAt { get; set; }

        /// <summary>
        /// Gets or sets the date after the task was started to filter.
        /// </summary>
        public DateTime? AfterStartedAt { get; set; }

        /// <summary>
        /// Gets or sets the date before the task was finished to filter.
        /// </summary>
        public DateTime? BeforeFinishedAt { get; set; }

        /// <summary>
        /// Gets or sets the date after the task was finished to filter.
        /// </summary>
        public DateTime? AfterFinishedAt { get; set; }
    }
}
