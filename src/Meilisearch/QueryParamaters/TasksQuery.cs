using System.Collections.Generic;

namespace Meilisearch.QueryParameters
{
    /// <summary>
    /// Tasks Query for Meilisearch class.
    /// </summary>
    public class TasksQuery
    {
        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Gets or sets offset size.
        /// </summary>
        public int? From { get; set; }

        /// <summary>
        /// Gets or sets offset size.
        /// </summary>
        public List<string> IndexUid { get; set; }

        /// <summary>
        /// Gets or sets offset size.
        /// </summary>
        public List<string> Status { get; set; }

        /// <summary>
        /// Gets or sets offset size.
        /// </summary>
        public List<string> Types { get; set; }
    }
}
