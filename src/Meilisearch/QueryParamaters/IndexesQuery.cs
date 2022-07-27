namespace Meilisearch.QueryParameters
{
    /// <summary>
    /// A class that handles the creation of a query string for Indexes.
    /// </summary>
    public class IndexesQuery
    {
        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        public int? Offset { get; set; }
    }
}
