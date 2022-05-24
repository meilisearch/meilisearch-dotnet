using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meilisearch.Tests
{
    internal static class Datasets
    {
        private static readonly string BasePath = Path.Combine(Directory.GetCurrentDirectory(), "Datasets");
        public static readonly string MoviesJson = Path.Combine(BasePath, "movies.json");
        public static readonly string NestedMoviesJson = Path.Combine(BasePath, "nested_movies.json");
        public static readonly string SmallMoviesJson = Path.Combine(BasePath, "small_movies.json");
    }

    public class DatasetMovie
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Poster { get; set; }
        public string Overview { get; set; }
        [JsonPropertyName("release_date")]
        [JsonConverter(typeof(UnixEpochDateTimeConverter))]
        public DateTime ReleaseDate { get; set; }
    }

    public class DatasetSmallMovie
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Poster { get; set; }
        public string Overview { get; set; }
        [JsonPropertyName("release_date")]
        [JsonConverter(typeof(UnixEpochDateTimeConverter))]
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; }
    }

    sealed class UnixEpochDateTimeConverter : JsonConverter<DateTime>
    {
        static readonly DateTime s_epoch = new DateTime(1970, 1, 1, 0, 0, 0);

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            var unixTime = reader.GetInt64();
            return s_epoch.AddMilliseconds(unixTime);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var unixTime = Convert.ToInt64((value - s_epoch).TotalMilliseconds);
            writer.WriteNumberValue(unixTime);
        }
    }
}
