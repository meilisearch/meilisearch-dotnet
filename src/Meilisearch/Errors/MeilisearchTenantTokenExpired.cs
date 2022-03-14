using System;

namespace Meilisearch
{
    /// <summary>
    /// Represents an exception thrown when the provided expiration date is invalid or in the past.
    /// </summary>
    public class MeilisearchTenantTokenExpired : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchTenantTokenExpired"/> class.
        /// </summary>
        public MeilisearchTenantTokenExpired()
            : base("Provide a valid UTC DateTime in the future.")
        {
        }
    }
}
