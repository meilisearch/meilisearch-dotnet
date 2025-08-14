using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch.Tests.Models
{
    public class VectorMovie
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("release_year")]
        public int ReleaseYear { get; set; }

        [JsonPropertyName("_vectors")]
        public Dictionary<string, double[]> Vectors { get; set; }
    }
}
