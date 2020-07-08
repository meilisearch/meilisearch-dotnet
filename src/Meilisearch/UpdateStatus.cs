namespace Meilisearch
{
    /// <summary>
    /// Update Status of the actions done.
    /// </summary>
    public class UpdateStatus
    {
        /// <summary>
        /// Update ID for the operation.
        /// </summary>
        public int UpdateId { get; set; }

        /// <summary>
        /// State of the operation.
        /// </summary>
        public string Status { get; set; }


    }
}
