using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Information regarding an API key for the Meilisearch server.
    /// </summary>
    public class Key
    {
        /// <summary>
        /// Create a key whith an expiry date. Pass null if it's not expiring.
        /// </summary>
        /// <param name="expiresAt">Expiry date.</param>
        public Key(DateTime? expiresAt)
        {
            ExpiresAt = expiresAt;
        }

        /// <summary>
        /// Gets or sets unique identifier of the API key.
        /// </summary>
        [JsonPropertyName("key")]
        public string KeyUid { get; set; }

        /// <summary>
        /// Gets or sets the description of the API key.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets list of actions available for the API key.
        /// </summary>
        public IEnumerable<string> Actions { get; set; }

        /// <summary>
        /// Gets or sets the list of indexes the API key can access.
        /// </summary>
        public IEnumerable<string> Indexes { get; set; }

        /// <summary>
        /// Gets or sets the date when the API key expires.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public DateTime? ExpiresAt { get; private set; }

        /// <summary>
        /// Gets or sets the date when the API key was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date when the API key was updated.
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Update the key expiry date to a new one. Pass null if you want it to never expires.
        /// </summary>
        /// <param name="expiresAt">New expiry date.</param>
        public void SetExpiresAt(DateTime? expiresAt)
        {
            ExpiresAt = expiresAt;
        }
    }
}
