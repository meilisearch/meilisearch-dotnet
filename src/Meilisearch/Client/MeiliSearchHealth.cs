namespace Meilisearch
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Deserialized response of the meilisearch health.
    /// </summary>
    public class MeiliSearchHealth
    {
        /// <summary>
        /// Gets or sets health of meilisearch server.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
