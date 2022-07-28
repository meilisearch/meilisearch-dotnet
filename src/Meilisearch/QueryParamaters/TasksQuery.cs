using System.Collections.Generic;

namespace Meilisearch.QueryParameters
{
    /// <summary>
    /// A class that handles the creation of a query string for Tasks.
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
