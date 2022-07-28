using System;

namespace Meilisearch
{
    /// <summary>
    /// Represents an exception thrown when `apiKey` is not present
    /// to sign correctly the Tenant Token generation.
    /// </summary>
    public class MeilisearchTenantTokenApiKeyUidInvalid : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeilisearchTenantTokenApiKeyUidInvalid"/> class.
        /// </summary>
        public MeilisearchTenantTokenApiKeyUidInvalid()
            : base("Cannot generate a signed token without a valid apiKeyUid. Provide one in the method params.")
        {
        }
    }
}
