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
}
