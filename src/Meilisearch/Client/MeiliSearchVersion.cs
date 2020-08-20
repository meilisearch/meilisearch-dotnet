using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Deserialized response of the MeiliSearch version.
    /// </summary>
    public class MeiliSearchVersion
    {
        /// <summary>
        /// Commit SHA for MeiliSearch.
        /// </summary>
        [JsonPropertyName("commitSha")]
        public string CommitSha { get; set; }

        /// <summary>
        /// Build date of the current version.
        /// </summary>
        [JsonPropertyName("buildDate")]
        public string BuildDate { get; set; }

        /// <summary>
        /// Information about MeiliSearch version.
        /// </summary>
        [JsonPropertyName("pkgVersion")]
        public string Version { get; set; }
    }
}
