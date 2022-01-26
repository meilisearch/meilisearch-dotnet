namespace Meilisearch
{
    /// <summary>
    /// Document Query for meilisearch class.
    /// </summary>
    public class DocumentQuery
    {
        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        public int? Offset { get; set; }

        /// <summary>
        /// Gets or sets the limit.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Gets or sets the attributes to retrieve.
        /// </summary>
        public string AttributesToRetrieve { get; set; }
    }
}
