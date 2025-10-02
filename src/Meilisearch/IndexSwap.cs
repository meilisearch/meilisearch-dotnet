using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meilisearch
{
    /// <summary>
    /// Model for index swaps requests.
    /// </summary>
    public class IndexSwap
    {
        [JsonPropertyName("indexes")]
        public List<string> Indexes { get; private set; }

        [JsonPropertyName("rename")]
        public bool Rename { get; set; } = false;

        public IndexSwap(string indexA, string indexB, bool rename = false)
        {
            this.Indexes = new List<string> { indexA, indexB };
            this.Rename = rename;
        }
    }
}
