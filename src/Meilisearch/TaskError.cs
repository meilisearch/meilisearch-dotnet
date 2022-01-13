namespace Meilisearch
{
    /// <summary>
    /// Task error details.
    /// </summary>
    public class TaskError
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the link to error definition.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the error type.
        /// </summary>
        public string Type { get; set; }
    }
}
