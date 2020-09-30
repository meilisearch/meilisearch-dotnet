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
        /// Gets or sets the total count of search results.
        /// </summary>
        public IEnumerable<T> Hits { get; set; }

        /// <summary>
        /// Gets or sets the offset of the initial search.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets the limit of the initial search.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Gets or sets the query sent.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Gets or sets the facets distribution.
        /// </summary>
        public Dictionary<string, IEnumerable<string>> FacetsDistribution { get; set; }

        /// <summary>
        /// Gets or sets the ExhaustiveFacetsCount boolean.
        /// </summary>
        public bool ExhaustiveFacetsCount { get; set; }
    }
}
