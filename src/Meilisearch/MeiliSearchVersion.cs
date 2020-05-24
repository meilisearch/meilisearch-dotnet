using Newtonsoft.Json;

namespace Meilisearch
{
    /// <summary>
    /// Deserialized response of the Meilisearch verion
    /// </summary>
    public class MeiliSearchVersion
    {
        /// <summary>
        /// Commit Sha for Meilisearch
        /// </summary>
        [JsonProperty(PropertyName = "commitSha")]
        public string CommitSha { get; set; }
        
        /// <summary>
        /// Build Date for current version
        /// </summary>
        [JsonProperty(PropertyName = "buildDate")]
        public string BuildDate { get; set; }
        
        /// <summary>
        /// Version information for Meilisearch.
        /// </summary>
        [JsonProperty(PropertyName = "pkgVersion")]
        public string Version { get; set; }
    }
}