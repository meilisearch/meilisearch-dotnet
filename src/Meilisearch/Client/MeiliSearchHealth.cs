namespace Meilisearch
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Deserialized response of the MeiliSearch health.
    /// </summary>
    public class MeiliSearchHealth
    {
        /// <summary>
        /// Gets or sets health of MeiliSearch server.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
