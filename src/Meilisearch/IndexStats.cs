using System.Collections.Generic;

namespace Meilisearch
{
    /// <summary>
    /// Wrapper for index stats.
    /// </summary>
    public class IndexStats
    {
        /// <summary>
        /// Gets or sets the total number of documents.
        /// </summary>
        public int NumberOfDocuments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the index is currently indexing.
        /// </summary>
        public bool IsIndexing { get; set; }

        /// <summary>
        /// Gets or sets field distribution.
        /// </summary>
        public IDictionary<string, int> FieldDistribution { get; set; }
    }
}