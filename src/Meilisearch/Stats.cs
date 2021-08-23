namespace Meilisearch
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Wrapper for index stats.
    /// </summary>
    public class Stats
    {
        /// <summary>
        /// Gets or sets database size.
        /// </summary>
        public int DatabaseSize { get; set; }

        /// <summary>
        /// Gets or sets last update timestamp.
        /// </summary>
        public DateTime? LastUpdate { get; set; }

        /// <summary>
        /// Gets or sets index stats.
        /// </summary>
        public IDictionary<string, IndexStats> Indexes { get; set; }
    }
}
