namespace Meilisearch.Tests
{
    public class Movie
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
}
