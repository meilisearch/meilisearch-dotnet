using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Error sent by Meilisearch API.
    /// </summary>
    public class MeilisearchApiErrorContent
    {
        public MeilisearchApiErrorContent(string message, string code, string type, string link)
        {
            Message = message;
            Code = code;
            Type = type;
            Link = link;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; }

        /// <summary>
        /// Gets or sets the link.
        /// </summary>
        [JsonPropertyName("link")]
        public string Link { get; }
    }
}
