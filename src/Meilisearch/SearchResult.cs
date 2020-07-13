using System.Collections.Generic;

namespace Meilisearch
{
    /// <summary>
    /// Wrapper for Search Results
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SearchResult<T>
    {
        /// <summary>
        /// Total count of search results.
        /// </summary>
        public IEnumerable<T> Hits { get; set; }

        /// <summary>
        /// Offset of the initial search.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Limit of the initial search.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Query sent.
        /// </summary>
        public string Query { get; set; }
    }
}
