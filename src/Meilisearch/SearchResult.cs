namespace Meilisearch
{
    using System.Collections.Generic;

    /// <summary>
    /// Wrapper for Search Results.
    /// </summary>
    /// <typeparam name="T">Hit type.</typeparam>
    public class SearchResult<T>
    {
        /// <summary>
        /// Gets or sets total count of search results.
        /// </summary>
        public IEnumerable<T> Hits { get; set; }

        /// <summary>
        /// Gets or sets offset of the initial search.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets limit of the initial search.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Gets or sets query sent.
        /// </summary>
        public string Query { get; set; }
    }
}
