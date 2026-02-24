namespace Meilisearch
{
    /// <summary>
    /// Generic result class for resources.
    /// When returning a list, Meilisearch stores the data in the "results" field, to allow better pagination.
    /// </summary>
    /// <typeparam name="T">Type of the Meilisearch server object. Ex: keys, indexes, ...</typeparam>
    public class ChunkedResults<T> : Result<T>
    {
        /// <summary>
        /// Constructor for ChunkedResults.
        /// </summary>
        /// <param name="results">Results</param>
        /// <param name="limit">Results limit</param>
        /// <param name="from">Uid of the first item returned</param>
        /// <param name="next">Value passed to from to view the next “page” of results. When the value of next is null, there are no more items to view</param>
        /// <param name="total">Total number of items matching the filter or query</param>
        public ChunkedResults(T results, int? limit, int? from, int? next, int? total)
            : base(results, limit)
        {
            From = from;
            Next = next;
            Total = total;
        }

        /// <summary>
        /// Gets from size.
        /// </summary>
        public int? From { get; }

        /// <summary>
        /// Gets next size.
        /// </summary>
        public int? Next { get; }

        /// <summary>
        /// Gets total number of results.
        /// </summary>
        public int? Total { get; }
    }
}
