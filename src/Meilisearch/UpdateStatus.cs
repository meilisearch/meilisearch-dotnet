namespace Meilisearch
{
    /// <summary>
    /// Update Status of the actions done.
    /// </summary>
    public class UpdateStatus
    {
        /// <summary>
        /// Gets or sets update ID for the operation.
        /// </summary>
        public int UpdateId { get; set; }

        /// <summary>
        /// Gets or sets state of the operation.
        /// </summary>
        public string Status { get; set; }
    }
}
