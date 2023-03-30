using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Wrapper for Facet Stats.
    /// </summary>
    public class FacetStat
    {
        /// <summary>
        /// Minimum value returned by FacetDistribution per facet
        /// </summary>
        [JsonPropertyName("min")]
        public float Min { get; set; }

        /// <summary>
        /// Maximum value returned by FacetDistribution per facet
        /// </summary>
        [JsonPropertyName("max")]
        public float Max { get; set; }
    }
}
