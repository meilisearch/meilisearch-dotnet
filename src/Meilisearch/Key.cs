namespace Meilisearch
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Information regarding an API key for the MeiliSearch server.
    /// </summary>
    public class Key
    {
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
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets the date when the API key was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date when the API key was updated.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
