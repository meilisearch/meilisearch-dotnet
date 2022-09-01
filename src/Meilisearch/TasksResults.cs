using System.Collections.Generic;

namespace Meilisearch
{
    /// <summary>
    /// Generic result class for resources.
    /// When returning a list, Meilisearch stores the data in the "results" field, to allow better pagination.
    /// </summary>
    /// <typeparam name="T">Type of the Meilisearch server object. Ex: keys, indexes, ...</typeparam>
    public class TasksResults<T> : Result<T>
    {
        public TasksResults(T results, int? limit, int? from, int? next)
            : base(results, limit)
        {
            From = from;
            Next = next;
        }

        /// <summary>
        /// Gets or sets from size.
        /// </summary>
        public int? From { get; }

        /// <summary>
        /// Gets or sets next size.
        /// </summary>
        public int? Next { get; }
    }
}
