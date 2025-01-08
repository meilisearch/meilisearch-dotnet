using System.Collections.Generic;
using System.Text.Json.Serialization;

using Meilisearch.Converters;

namespace Meilisearch
{
    /// <summary>
    /// Faceting configuration.
    /// </summary>
    public class Faceting
    {
        /// <summary>
        /// Gets or sets maxValuesPerFacet.
        /// </summary>
        [JsonPropertyName("maxValuesPerFacet")]
        public int MaxValuesPerFacet { get; set; }

        /// <summary>
        /// Gets or sets sortFacetValuesBy.
        /// </summary>
        [JsonPropertyName("sortFacetValuesBy")]
        public Dictionary<string, SortFacetValuesByType> SortFacetValuesBy { get; set; }
    }

    [JsonConverter(typeof(SortFacetValuesConverter))]
    public enum SortFacetValuesByType
    {
        /// <summary>
        /// Sort by alphanumerical value.
        /// </summary>
        Alpha,

        /// <summary>
        /// Sort by count value.
        /// </summary>
        Count
    }
}
