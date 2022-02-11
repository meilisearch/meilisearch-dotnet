using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Error sent by Meilisearch API.
    /// </summary>
    public class MeilisearchApiErrorContent
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the link.
        /// </summary>
        [JsonPropertyName("link")]
        public string Link { get; set; }
    }
}
