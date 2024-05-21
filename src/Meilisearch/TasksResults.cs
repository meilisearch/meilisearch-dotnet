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
        public TasksResults(T results, int? limit, int? from, int? next, int? total)
            : base(results, limit)
        {
            From = from;
            Next = next;
            Total = total;
        }

        /// <summary>
        /// Gets from size.
        /// </summary>
        public int? From { get; }

        /// <summary>
        /// Gets next size.
        /// </summary>
        public int? Next { get; }

        /// <summary>
        /// Gets total number of tasks.
        /// </summary>
        public int? Total { get; }
    }
}
