using System.Collections.Generic;

namespace Meilisearch.QueryParameters
{
    /// <summary>
    /// Documents Query for Meilisearch class.
    /// </summary>
    public class DocumentsQuery
    {
        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// Gets or sets the attributes to retrieve.
        /// </summary>
        public List<string> Fields { get; set; }
    }
}
