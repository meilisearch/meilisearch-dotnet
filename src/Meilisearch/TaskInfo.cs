namespace Meilisearch
{
    using System.Collections.Generic;

    /// <summary>
    /// Update Status of the actions done.
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
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets type of the task.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the error raised.
        /// </summary>
        public Dictionary<string, string> Error { get; set; }
    }
}
