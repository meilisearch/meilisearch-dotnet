namespace Meilisearch
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Deserialized response of the meilisearch version.
    /// </summary>
    public class MeiliSearchVersion
    {
        /// <summary>
        /// Gets or sets commit SHA for meilisearch.
        /// </summary>
        [JsonPropertyName("commitSha")]
        public string CommitSha { get; set; }

        /// <summary>
        /// Gets or sets build date of the current version.
        /// </summary>
        [JsonPropertyName("commitDate")]
        public string CommitDate { get; set; }

        /// <summary>
        /// Gets or sets information about meilisearch version.
        /// </summary>
        [JsonPropertyName("pkgVersion")]
        public string Version { get; set; }
    }
}
