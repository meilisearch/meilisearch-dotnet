using System.Collections.Generic;
using System.Text.Json.Serialization;

using Meilisearch.Converters;

namespace Meilisearch
{
    /// <summary>
    /// Configure at least one embedder to use AI-powered search.
    /// </summary>
    public class Embedder
    {
        /// <summary>
        /// Use source to configure an embedder's source.
        /// This field is mandatory.
        /// </summary>
        [JsonPropertyName("source")]
        public EmbedderSource Source { get; set; }

        /// <summary>
        /// Meilisearch queries url to generate vector embeddings for queries and documents.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// Authentication token Meilisearch should send with each request to the embedder.
        /// </summary>
        [JsonPropertyName("apiKey")]
        public string ApiKey { get; set; }

        /// <summary>
        /// The model your embedder uses when generating vectors. 
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// documentTemplate is a string containing a Liquid template.
        /// </summary>
        [JsonPropertyName("documentTemplate")]
        public string DocumentTemplate { get; set; }

        /// <summary>
        /// The maximum size of a rendered document template. Longer texts are truncated to fit the configured limit.
        /// </summary>
        [JsonPropertyName("documentTemplateMaxBytes")]
        public int? DocumentTemplateMaxBytes { get; set; }

        /// <summary>
        /// Number of dimensions in the chosen model. If not supplied, Meilisearch tries to infer this value.
        /// </summary>
        [JsonPropertyName("dimensions")]
        public int? Dimensions { get; set; }

        /// <summary>
        /// Use this field to use a specific revision of a model.
        /// </summary>
        [JsonPropertyName("revision")]
        public string Revision { get; set; }

        /// <summary>
        /// Use distribution when configuring an embedder to correct the returned
        /// _rankingScores of the semantic hits with an affine transformation
        /// </summary>
        [JsonPropertyName("distribution")]
        public Distribution Distribution { get; set; }

        ///// <summary>
        ///// request must be a JSON object with the same structure
        ///// and data of the request you must send to your rest embedder.
        ///// </summary>
        //[JsonPropertyName("request")]
        //public object Request { get; set; }

        ///// <summary>
        ///// response must be a JSON object with the same structure
        ///// and data of the response you expect to receive from your rest embedder.
        ///// </summary>
        //[JsonPropertyName("response")]
        //public object Response { get; set; }

        /// <summary>
        /// When set to true, compresses vectors by representing each dimension with 1-bit values. 
        /// </summary>
        [JsonPropertyName("binaryQuantized")]
        public bool? BinaryQuantized { get; set; }
    }

    /// <summary>
    /// Configuring distribution requires a certain amount of trial and error,
    /// in which you must perform semantic searches and monitor the results.
    /// Based on their rankingScores and relevancy, add the observed mean and sigma values for that index.
    /// </summary>
    public class Distribution
    {
        /// <summary>
        ///  a number between 0 and 1 indicating the semantic score of "somewhat relevant"
        ///  hits before using the distribution setting.
        /// </summary>
        [JsonPropertyName("mean")]
        public float? Mean { get; set; }

        /// <summary>
        /// a number between 0 and 1 indicating the average absolute difference in
        /// _rankingScores between "very relevant" hits and "somewhat relevant" hits,
        /// and "somewhat relevant" hits and "irrelevant hits".
        /// </summary>
        [JsonPropertyName("sigma")]
        public float? Sigma { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [JsonConverter(typeof(EmbedderSourceConverter))]
    public enum EmbedderSource
    {
        /// <summary>
        /// empty source
        /// </summary>
        Empty,
        /// <summary>
        /// openAi source
        /// </summary>
        OpenAi,
        /// <summary>
        /// guggingFace source
        /// </summary>
        HuggingFace,
        /// <summary>
        /// ollama source
        /// </summary>
        Ollama,
        /// <summary>
        /// use rest to auto-generate embeddings with any embedder offering a REST API.
        /// </summary>
        Rest,
        /// <summary>
        /// You may also configure a userProvided embedder.
        /// In this case, you must manually include vector data in your documents' _vectors field.
        /// </summary>
        UserProvided
    }
}
