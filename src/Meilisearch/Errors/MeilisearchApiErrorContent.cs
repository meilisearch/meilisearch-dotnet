namespace Meilisearch
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Error sent by MeiliSearch API.
    /// </summary>
    public class MeilisearchApiErrorContent
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the errorCode.
        /// </summary>
        [JsonPropertyName("errorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the errorType.
        /// </summary>
        [JsonPropertyName("errorType")]
        public string ErrorType { get; set; }

        /// <summary>
        /// Gets or sets the errorLink.
        /// </summary>
        [JsonPropertyName("errorLink")]
        public string ErrorLink { get; set; }
    }
}
