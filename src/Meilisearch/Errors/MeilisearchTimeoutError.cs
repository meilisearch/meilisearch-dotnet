using System;

namespace Meilisearch
{
    /// <summary>
    /// Error sent when request not processed in expected time.
    /// </summary>
    public class MeilisearchTimeoutError : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchTimeoutError"/> class.
        /// Handler Exception for MeilisearchTimeoutError with message.
        /// </summary>
        /// <param name="message">Custom error message.</param>
        public MeilisearchTimeoutError(string message)
            : base(message)
        {
        }
    }
}
