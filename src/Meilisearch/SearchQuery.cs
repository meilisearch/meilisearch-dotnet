using System.Collections.Generic;

namespace Meilisearch
{
    /// <summary>
    /// Search Query for MeiliSearch class
    /// </summary>
    public class SearchQuery
    {
        /// <summary>
        /// Query string
        /// </summary>
        public string Q { get; set; }

        /// <summary>
        /// Offset for the Query.
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// Limits the number of results.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Filters to apply to the query.
        /// </summary>
        public IEnumerable<string> Filters { get; set; }

        /// <summary>
        /// Attributes to retrieve.
        /// </summary>
        public IEnumerable<string> AttributesToRetrieve { get; set; }

        /// <summary>
        /// Attributes to crop.
        /// </summary>
        public IEnumerable<string> AttributesToCrop { get; set; }

        /// <summary>
        /// Length used to crop field values.
        /// </summary>
        public int? CropLength { get; set; }

        /// <summary>
        /// Attributes to highlight.
        /// </summary>
        public IEnumerable<string> AttributesToHighlight { get; set; }

        /// <summary>
        /// Defines whether an object that contains information about the matches should be returned or not.
        /// </summary>
        public bool? Matches { get; set; }
    }
}
