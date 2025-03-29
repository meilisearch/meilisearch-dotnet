using System.Collections.Generic;
using System.Text.Json.Serialization;

using Meilisearch;


/// <summary>
/// A class that handles the creation of a query string for similar Documents.
/// </summary>
public class SimilarDocumentsQuery
{
    /// <summary>
    /// Identifier of the target document
    /// </summary>
    [JsonPropertyName("id")]
    public object Id { get; set; }

    /// <summary>
    /// Embedder name to use when computing recommendations
    /// </summary>
    [JsonPropertyName("embedder")]
    public string Embedder { get; set; }

    /// <summary>
    /// Attributes to display in the returned documents
    /// </summary>
    [JsonPropertyName("attributesToRetrieve")]
    public IEnumerable<string> AttributesToRetrieve { get; set; }

    /// <summary>
    /// Gets or sets the offset.
    /// </summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>
    /// Gets or sets the limit.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// Filter queries by an attribute's value
    /// </summary>
    [JsonPropertyName("filter")]
    public string Filter { get; set; }

    /// <summary>
    /// Display the global ranking score of a document
    /// </summary>
    [JsonPropertyName("showRankingScore")]
    public bool? ShowRankingScore { get; set; }

    /// <summary>
    /// Display detailed ranking score information
    /// </summary>
    [JsonPropertyName("showRankingScoreDetails")]
    public bool? ShowRankingScoreDetails { get; set; }

    /// <summary>
    /// Exclude results with low ranking scores
    /// </summary>
    [JsonPropertyName("rankingScoreThreshold")]
    public float? RankingScoreThreshold { get; set; }

    /// <summary>
    /// Return document vector data
    /// </summary>
    [JsonPropertyName("retrieveVectors")]
    public bool? RetrieveVectors { get; set; }
}
