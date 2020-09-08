namespace Meilisearch
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Deserialized response of the MeiliSearch version.
    /// </summary>
    public class MeiliSearchVersion
    {
        /// <summary>
        /// Gets or sets commit SHA for MeiliSearch.
        /// </summary>
        [JsonPropertyName("commitSha")]
        public string CommitSha { get; set; }

        /// <summary>
        /// Gets or sets build date of the current version.
        /// </summary>
        [JsonPropertyName("buildDate")]
        public string BuildDate { get; set; }

        /// <summary>
        /// Gets or sets information about MeiliSearch version.
        /// </summary>
        [JsonPropertyName("pkgVersion")]
        public string Version { get; set; }
    }
}
