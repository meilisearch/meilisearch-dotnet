using System;

namespace Meilisearch
{
    /// <summary>
    /// Represents an exception thrown when `apiKey` is not present
    /// to sign correctly the Tenant Token generation.
    /// </summary>
    public class MeilisearchTenantTokenApiKeyInvalid : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchTenantTokenApiKeyInvalid"/> class.
        /// </summary>
        public MeilisearchTenantTokenApiKeyInvalid()
            : base("Cannot generate a signed token without a valid apiKey. Provide one in the MeilisearchClient instance or in the method params.")
        {
        }
    }
}
