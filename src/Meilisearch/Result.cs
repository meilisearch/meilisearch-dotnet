namespace Meilisearch
{
    /// <summary>
    /// Generic result class.
    /// When returning a list, MeiliSearch stores the data in the "results" field, to allow better pagination.
    /// </summary>
    /// <typeparam name="T">Type of the MeiliSearch server object. Ex: keys, tasks, ...</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Gets or sets the "results" field.
        /// </summary>
        public T Results { get; set; }
    }
}
