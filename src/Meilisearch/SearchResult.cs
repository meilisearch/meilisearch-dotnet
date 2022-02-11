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
        /// Gets or sets the facets distribution.
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> FacetsDistribution { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the facets distribution is exhaustive or not.
        /// </summary>
        public bool ExhaustiveFacetsCount { get; set; }

        /// <summary>
        /// Gets or sets the nbHits returned by the search.
        /// </summary>
        public int NbHits { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the nbHits number returned by the search is exhaustive or not.
        /// </summary>
        public bool ExhaustiveNbHits { get; set; }
    }
}
