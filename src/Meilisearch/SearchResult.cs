using System.Collections.Generic;

namespace Meilisearch
{
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
        /// Gets or sets the facet distribution.
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> FacetDistribution { get; set; }

        /// <summary>
        /// Gets or sets the estimated total number of hits returned by the search.
        /// </summary>
        public int EstimatedTotalHits { get; set; }
    }
}
