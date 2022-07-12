using System.Collections.Generic;

namespace Meilisearch
{
    /// <summary>
    /// Generic result class for ressources.
    /// When returning a list, Meilisearch stores the data in the "results" field, to allow better pagination.
    /// </summary>
    /// <typeparam name="T">Type of the Meilisearch server object. Ex: keys, indexes, ...</typeparam>
    public class ResourceResults<T> : Result<T>
    {
        /// <summary>
        /// Gets or sets offset size.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets total size.
        /// </summary>
        public int Total { get; set; }
    }
}
