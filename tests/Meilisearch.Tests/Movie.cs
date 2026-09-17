using System.Collections.Generic;
using System.Text.Json;

namespace Meilisearch.Tests
{
    public class Movie
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Genre { get; set; }
    }

    public struct MovieStruct
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Genre { get; set; }
    }
    public class MovieInfo
    {
        public string Comment { get; set; }

        public int ReviewNb { get; set; }
    }

    public class MovieWithInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public MovieInfo Info { get; set; }
    }


    public class MovieWithIntId
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Genre { get; set; }
    }

    public class FormattedMovie
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Genre { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Naming convention used to match meilisearch.")]
        public Movie _Formatted { get; set; }
    }

    public class MovieWithRankingScore
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Genre { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Naming convention used to match meilisearch.")]
        public double? _RankingScore { get; set; }
    }

    public class MovieWithRankingScoreDetails
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Genre { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Naming convention used to match meilisearch.")]
        public IDictionary<string, JsonElement> _RankingScoreDetails { get; set; }
    }

    /// <summary>
    /// A movie hit from a federated multi-search, carrying the `_federation` metadata Meilisearch
    /// adds to every hit.
    /// </summary>
    public class FederatedMovie
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Genre { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("_federation")]
        public FederationMetadata Federation { get; set; }
    }

    public class FederationMetadata
    {
        [System.Text.Json.Serialization.JsonPropertyName("indexUid")]
        public string IndexUid { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("queriesPosition")]
        public int QueriesPosition { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("weightedRankingScore")]
        public double WeightedRankingScore { get; set; }
    }
}
