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

    /// <summary>
    /// Same documents as <see cref="VectorMovie"/>, shaped for a search made with
    /// <c>retrieveVectors</c>: Meilisearch returns <c>_vectors</c> as an embedder object rather than
    /// the bare array the documents were indexed with.
    /// </summary>
    public class VectorMovieWithEmbeddings
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("release_year")]
        public int ReleaseYear { get; set; }

        [JsonPropertyName("_vectors")]
        public Dictionary<string, RetrievedEmbedder> Vectors { get; set; }
    }

    public class RetrievedEmbedder
    {
        [JsonPropertyName("embeddings")]
        public double[][] Embeddings { get; set; }

        [JsonPropertyName("regenerate")]
        public bool Regenerate { get; set; }
    }
}
