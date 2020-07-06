using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Deserialized response of the Meilisearch verion
    /// </summary>
    public class MeiliSearchVersion
    {
        /// <summary>
        /// Commit Sha for MeiliSearch
        /// </summary>
        [JsonPropertyName("commitSha")]
        public string CommitSha { get; set; }

        /// <summary>
        /// Build Date for current version
        /// </summary>
        [JsonPropertyName("buildDate")]
        public string BuildDate { get; set; }

        /// <summary>
        /// Version information for MeiliSearch.
        /// </summary>
        [JsonPropertyName("pkgVersion")]
        public string Version { get; set; }
    }
}
