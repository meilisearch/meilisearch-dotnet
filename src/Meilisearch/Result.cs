using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Generic result class.
    /// When returning a list, Meilisearch stores the data in the "results" field, to allow better pagination.
    /// </summary>
    /// <typeparam name="T">Type of the Meilisearch server object. Ex: keys, tasks, ...</typeparam>
    public class Result<T>
    {
        public Result(T results, int? limit)
        {
            Results = results;
            Limit = limit;
        }

        /// <summary>
        /// Gets or sets the "results" field.
        /// </summary>
        [JsonPropertyName("results")]
        public T Results { get; }

        /// <summary>
        /// Gets or sets limit size.
        /// </summary>
        [JsonPropertyName("limit")]
        public int? Limit { get; }
    }
}
