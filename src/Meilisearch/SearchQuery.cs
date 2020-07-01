namespace Meilisearch
{
    /// <summary>
    /// Search Query for MeiliSearch class
    /// </summary>
    public class SearchQuery
    {
        /// <summary>
        /// Offset for the Query.
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// Limit the number of results.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Filters to search the query.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// Attributes to retrieve.
        /// </summary>
        public string AttributesToRetrieve { get; set; }

        /// <summary>
        /// Attributes to crop.
        /// </summary>
        public string attributesToCrop { get; set; }

        /// <summary>
        /// Length used to crop field values
        /// </summary>
        public int? cropLength { get; set; }

        /// <summary>
        /// Attributes whose values will contain highlighted matching terms
        /// </summary>
        public string AttributesToHighlight { get; set; }

        /// <summary>
        /// Defines whether an object that contains information about the matches should be returned or not
        /// </summary>
        public string Matches { get; set; }
    }
}
