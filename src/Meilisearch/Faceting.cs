using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Faceting configuration.
    /// </summary>
    public class Faceting
    {
        [JsonPropertyName("maxValuesPerFacet")]
        public int MaxValuesPerFacet { get; set; }
    }
}
