using System.Collections.Generic;

namespace Meilisearch
{
    /// <summary>
    /// Wrapper for Search Results.
    /// </summary>
    /// <typeparam name="T">Hit type.</typeparam>
    public class SearchResult<T>
    {
        public SearchResult(IReadOnlyCollection<T> hits, int offset, int limit, int nbHits, bool exhaustiveNbHits,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> facetsDistribution, bool exhaustiveFacetsCount,
            int processingTimeMs, string query)
        {
            Hits = hits;
            Offset = offset;
            Limit = limit;
            NbHits = nbHits;
            ExhaustiveNbHits = exhaustiveNbHits;
            FacetsDistribution = facetsDistribution;
            ExhaustiveFacetsCount = exhaustiveFacetsCount;
            ProcessingTimeMs = processingTimeMs;
            Query = query;
        }

        /// <summary>
        /// Results of the query.
        /// </summary>
        public IReadOnlyCollection<T> Hits { get; }

        /// <summary>
        /// Number of documents skipped.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Number of documents to take.
        /// </summary>
        public int Limit { get; }

        /// <summary>
        /// Total number of matches.
        /// </summary>
        public int NbHits { get; }

        /// <summary>
        /// Whether nbHits is exhaustive.
        /// </summary>
        public bool ExhaustiveNbHits { get; }

        /// <summary>
        /// Returns the number of documents matching the current search query for each given facet.
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> FacetsDistribution { get; }

        /// <summary>
        /// Whether facetsDistribution is exhaustive.
        /// </summary>
        public bool ExhaustiveFacetsCount { get; }

        /// <summary>
        /// Processing time of the query.
        /// </summary>
        public int ProcessingTimeMs { get; }

        /// <summary>
        /// Query originating the response.
        /// </summary>
        public string Query { get; }
    }
}
