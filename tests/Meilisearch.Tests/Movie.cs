namespace Meilisearch.Tests
{
    public class Movie
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Genre { get; set; }
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

#pragma warning disable SA1300
        public Movie _Formatted { get; set; }
    }
}
