namespace Meilisearch
{
    /// <summary>
    /// Federation options in federated multi-index search
    /// </summary>
    public class MultiSearchFederationOptions
    {
        /// <summary>
        /// Number of documents to skip
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Maximum number of documents returned
        /// </summary>
        public int Limit { get; set; }
    }
}
