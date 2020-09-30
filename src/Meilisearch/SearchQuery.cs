namespace Meilisearch
{
    using System.Collections.Generic;

    /// <summary>
    /// Search Query for MeiliSearch class.
    /// </summary>
    public class SearchQuery
    {
        /// <summary>
        /// Gets or sets query string.
        /// </summary>
        public string Q { get; set; }

        /// <summary>
        /// Gets or sets offset for the Query.
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// Gets or sets limits the number of results.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Gets or sets filters to apply to the query.
        /// </summary>
        public IEnumerable<string> Filters { get; set; }

        /// <summary>
        /// Gets or sets attributes to retrieve.
        /// </summary>
        public IEnumerable<string> AttributesToRetrieve { get; set; }

        /// <summary>
        /// Gets or sets attributes to crop.
        /// </summary>
        public IEnumerable<string> AttributesToCrop { get; set; }

        /// <summary>
        /// Gets or sets length used to crop field values.
        /// </summary>
        public int? CropLength { get; set; }

        /// <summary>
        /// Gets or sets attributes to highlight.
        /// </summary>
        public IEnumerable<string> AttributesToHighlight { get; set; }

        /// <summary>
        /// Gets or sets the faceted search to apply to the query.
        /// </summary>
        public IEnumerable<dynamic> FacetFilters { get; set; }

        /// <summary>
        /// Gets or sets the facets distribution for the query.
        /// </summary>
        public IEnumerable<string> FacetsDistribution { get; set; }

        /// <summary>
        /// Gets or sets matches. It defines whether an object that contains information about the matches should be returned or not.
        /// </summary>
        public bool? Matches { get; set; }
    }
}
