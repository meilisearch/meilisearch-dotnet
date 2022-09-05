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
        /// Gets or sets unique identifier of the API key.
        /// </summary>
        [JsonPropertyName("key")]
        public string KeyUid { get; set; }

        /// <summary>
        /// Gets or sets unique identifier of the API key.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the name of the API key.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the API key.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets list of actions available for the API key.
        /// </summary>
        [JsonPropertyName("actions")]
        public IEnumerable<string> Actions { get; set; }

        /// <summary>
        /// Gets or sets the list of indexes the API key can access.
        /// </summary>
        [JsonPropertyName("indexes")]
        public IEnumerable<string> Indexes { get; set; }

        /// <summary>
        /// Gets or sets the date when the API key expires.
        /// </summary>
        [JsonPropertyName("expiresAt")]
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets the date when the API key was created.
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date when the API key was updated.
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}
