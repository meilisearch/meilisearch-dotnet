using Newtonsoft.Json;

namespace Meilisearch
{
    /// <summary>
    /// Meilisearch Index for Search and managing document. 
    /// </summary>
    public class Index
    {
        /// <summary>
        /// Initializes with the default unique identifier and Primary Key.
        /// </summary>
        /// <param name="uid">Unique Identifier</param>
        /// <param name="primaryKey"></param>
        public Index(string uid,string primaryKey=default)
        {
            this.Uid = uid;
            this.PrimaryKey = primaryKey;
        }
        
        /// <summary>
        /// Unique Identifier for the Index. 
        /// </summary>
        [JsonProperty(PropertyName = "uid")] public string Uid { get; private set; }

        /// <summary>
        /// Primary key of the document.
        /// </summary>
        [JsonProperty(PropertyName = "primaryKey")] public string PrimaryKey { get; private set; }
    }
}