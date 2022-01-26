namespace Meilisearch
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Deserialized response of the Meilisearch health.
    /// </summary>
    public class MeiliSearchHealth
    {
        /// <summary>
        /// Gets or sets health of Meilisearch server.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
