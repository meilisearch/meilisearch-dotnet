namespace Meilisearch
{
    /// <summary>
    /// Update Status of the actions done.
    /// </summary>
    public class UpdateStatus
    {
        /// <summary>
        /// Update ID for the actions.
        /// </summary>
        public int UpdateId { get; set; }

        /// <summary>
        /// Actions done for the update.
        /// </summary>
        public string Status { get; set; }


    }
}
