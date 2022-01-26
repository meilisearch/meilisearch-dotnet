namespace Meilisearch
{
    using System;

    /// <summary>
    /// Error sent when trying to connecting to meilisearch.
    /// </summary>
    public class MeilisearchCommunicationError : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchCommunicationError"/> class.
        /// Handler Exception for MeilisearchCommunicationError with message and inner exception.
        /// </summary>
        /// <param name="message">Custom error message.</param>
        /// <param name="innerException">Inner exception.</param>
        public MeilisearchCommunicationError(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
