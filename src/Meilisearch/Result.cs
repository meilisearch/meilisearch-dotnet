namespace Meilisearch
{
    using System.Collections.Generic;
    /// <summary>
    /// Generic result class.
    /// When returning a list, MeiliSearch stores the data in the "results" field, to allow better pagination.
    /// </summary>
    public class Result<T>
    {
        /// <summary>
        /// Gets or sets the "results" field
        /// </summary>
        public T Results { get; set;}
    }
}
